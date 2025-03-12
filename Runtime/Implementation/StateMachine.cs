using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UState
{
    public class StateMachine : IStateMachine, IStateMachineEvents, IStateMachineTicks, IDisposable
    {
        public event Action<Type> StateChanged;
        public event Action<Type, Type> StateChangedFrom;

        private readonly IStateFactory m_stateFactory;
        public ExitableState CurrentState { get; private set; }
        private ExitableState m_tickState;

        public StateMachine(IStateFactory stateFactory)
        {
            m_stateFactory = stateFactory;
        }

        public async UniTaskVoid Enter<TState>() where TState : State
        {
            m_tickState = null;

            Type newStateType = typeof(TState);
            Type lastStateType = CurrentState?.GetType();
            ExitableState previousState = CurrentState;

            State newState = (State)m_stateFactory.Create(newStateType);
            newState.StateMachine = this;
            newState.StateLifetimeTokenSource = new CancellationTokenSource();

            try
            {
                if (previousState != null)
                {
                    previousState.StateLifetimeTokenSource.Cancel();
                    await previousState.Exit();
                }

                UniTask enterTask = newState.Enter();
                CurrentState = newState;
                await enterTask;
                m_tickState = CurrentState;

                StateChanged?.Invoke(newStateType);
                if (lastStateType != null) StateChangedFrom?.Invoke(lastStateType, newStateType);
            }
            catch (ObjectDisposedException exception)
            {
                Debug.LogException(exception);
            }
            catch (OperationCanceledException canceledException)
            {
                Debug.LogException(canceledException);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async UniTaskVoid Enter<TState, TModel>(TModel model) where TState : ModelState<TModel>
        {
            m_tickState = null;
            Type newStateType = typeof(TState);
            Type lastStateType = CurrentState?.GetType();
            ExitableState previousState = CurrentState;

            ModelState<TModel> newState = (ModelState<TModel>)m_stateFactory.Create(newStateType);
            newState.Model = model;
            newState.StateMachine = this;
            newState.StateLifetimeTokenSource = new CancellationTokenSource();

            try
            {
                if (previousState != null)
                {
                    previousState.StateLifetimeTokenSource.Cancel();
                    await previousState.Exit();
                }

                UniTask enterTask = newState.Enter();
                CurrentState = newState;
                await enterTask;
                m_tickState = CurrentState;

                StateChanged?.Invoke(newStateType);
                if (lastStateType != null) StateChangedFrom?.Invoke(lastStateType, newStateType);
            }
            catch (ObjectDisposedException exception)
            {
                Debug.LogException(exception);
            }
            catch (OperationCanceledException canceledException)
            {
                Debug.LogException(canceledException);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public void Tick(float delta)
        {
            m_tickState?.Tick(delta);
        }

        public void FixedTick(float delta)
        {
            m_tickState?.FixedTick(delta);
        }

        public void Dispose()
        {
            CurrentState?.StateLifetimeTokenSource?.Cancel();
        }
    }
}