using System;
using System.Collections;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace UState.Tests.Editor
{
    public class StateMachineTest
    {
        private IStateMachine m_stateMachine;
        private IStateMachineTicks m_stateMachineTicks;
        private IStateMachineEvents m_stateMachineEvents;

        [SetUp]
        public void Setup()
        {
            DefaultStateFactory stateFactory = new();
            StateMachine stateMachine = new(stateFactory);

            m_stateMachine = stateMachine;
            m_stateMachineTicks = stateMachine;
            m_stateMachineEvents = stateMachine;
        }

        [UnityTest]
        public IEnumerator ChangeStateAndCheckNewState()
        {
            m_stateMachine.Enter<StateA>();

            Assert.IsTrue(m_stateMachine.CurrentState is StateA, "m_stateMachine.CurrentState is StateA");

            yield break;
        }

        [UnityTest]
        public IEnumerator ChangeStateAndCheckModelIsNotNullAndValid()
        {
            StateB.StateModel setModel = new(10);

            m_stateMachine.Enter<StateB, StateB.StateModel>(setModel);

            Assert.IsTrue(((StateB)m_stateMachine.CurrentState).Model != null, "m_stateMachine.CurrentState.Model != null");

            StateB.StateModel getModel = ((StateB)m_stateMachine.CurrentState).Model;

            Assert.IsTrue(getModel.Value == 10, "getModel.Value == 10");

            yield break;
        }

        [UnityTest]
        public IEnumerator ChangeStateAndCheckTickIsWorks()
        {
            m_stateMachine.Enter<StateC>();

            StateC state = (StateC)m_stateMachine.CurrentState;

            Assert.IsTrue(state.Value == 0, "state.Value==0");

            m_stateMachineTicks.Tick(Time.deltaTime);

            Assert.IsTrue(state.Value == 1, "state.Value==1");

            m_stateMachineTicks.Tick(Time.deltaTime);
            m_stateMachineTicks.Tick(Time.deltaTime);

            Assert.IsTrue(state.Value == 3, "state.Value==3");

            yield break;
        }

        [UnityTest]
        public IEnumerator StateLifetimeTokenIsCanceledAfterStateChange()
        {
            m_stateMachine.Enter<StateA>();

            int value = 0;

            CancellationToken token = m_stateMachine.CurrentState.LifetimeToken;
            token.Register(() => value++);

            m_stateMachine.Enter<StateC>();

            Assert.IsTrue(token.IsCancellationRequested, "token.IsCancellationRequested");
            Assert.IsTrue(value == 1, "value == 1");

            yield break;
        }

        [UnityTest]
        public IEnumerator CheckStateEventsTo()
        {
            Type to = null;

            m_stateMachineEvents.StateChanged += ToDo;

            void ToDo(Type type)
            {
                to = type;
                m_stateMachineEvents.StateChanged -= ToDo;
            }

            m_stateMachine.Enter<StateA>();

            Assert.IsTrue(to == typeof(StateA), "to == typeof(StateA)");

            yield break;
        }

        [UnityTest]
        public IEnumerator CheckStateEventsFromAndTo()
        {
            Type from = null, to = null;

            m_stateMachine.Enter<StateA>();

            m_stateMachineEvents.StateChangedFrom += ToDo;

            void ToDo(Type f, Type t)
            {
                from = f;
                to = t;
                m_stateMachineEvents.StateChangedFrom -= ToDo;
            }

            m_stateMachine.Enter<StateC>();

            Assert.IsTrue(from == typeof(StateA), "from == typeof(StateA)");
            Assert.IsTrue(to == typeof(StateC), "to == typeof(StateC)");

            yield break;
        }
    }
}