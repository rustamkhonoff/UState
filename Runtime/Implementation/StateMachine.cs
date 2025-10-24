using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

namespace UState
{
    public class StateMachine : IStateMachine, IStateMachineEvents, IStateMachineTicks, IAsyncDisposable
    {
        public event Action<IState> StateChanged;
        public event Action<IState, IState> StateChangedFrom;

        private readonly IStateFactory m_stateFactory;
        public IState CurrentState { get; private set; }

        public StateMachine(IStateFactory stateFactory)
        {
            m_stateFactory = stateFactory;
        }

        public async UniTask Enter<TState>() where TState : State
        {
            State newState = (State)m_stateFactory.Create(typeof(TState));
            newState.StateMachine = this;
            newState.StateLifetimeTokenSource = new CancellationTokenSource();

            await EnterInternal(newState);
        }

        public async UniTask Enter<TState, TModel>(TModel model) where TState : ModelState<TModel>
        {
            ModelState<TModel> newState = (ModelState<TModel>)m_stateFactory.Create(typeof(TState));
            newState.Model = model;
            newState.StateMachine = this;
            newState.StateLifetimeTokenSource = new CancellationTokenSource();

            await EnterInternal(newState);
        }

        private async UniTask EnterInternal(IState state)
        {
            IState lastState = CurrentState;
            try
            {
                if (lastState != null)
                {
                    lastState.Dispose();
                    await lastState.Exit();
                }

                CurrentState = state;

                await state.Enter();

                StateChanged?.Invoke(state);
                if (lastState != null) StateChangedFrom?.Invoke(lastState, CurrentState);
            }
            catch (ObjectDisposedException) { }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public void Tick(float delta) => CurrentState?.Tick(delta);
        public void FixedTick(float delta) => CurrentState?.FixedTick(delta);


        public async ValueTask DisposeAsync()
        {
            if (CurrentState == null) return;

            await CurrentState.Exit();
            CurrentState.Dispose();
        }
    }
}