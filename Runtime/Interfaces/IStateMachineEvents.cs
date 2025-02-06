using System;

namespace UState
{
    public interface IStateMachineEvents
    {
        event Action<Type> StateChanged;
        event Action<Type, Type> StateChangedFrom;
    }
}