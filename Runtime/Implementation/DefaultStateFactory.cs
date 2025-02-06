using System;

namespace UState
{
    public class DefaultStateFactory : IStateFactory
    {
        public object Create(Type type) => Activator.CreateInstance(type);
    }
}