using System;

namespace UState
{
    public interface IStateMachineEvents
    {
        event Action<IState> StateChanged;
        event Action<IState, IState> StateChangedFrom;
    }
}