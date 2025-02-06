using System;

namespace UState
{
    public interface IStateFactory
    {
        object Create(Type type);
    }
}