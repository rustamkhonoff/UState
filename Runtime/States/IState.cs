using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public interface IState : IDisposable
    {
        UniTask Enter();
        UniTask Exit();
        void Tick(float delta);
        void FixedTick(float delta);
        CancellationToken LifetimeToken { get; }
    }
}