using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public abstract class BaseState : IState
    {
        public IStateMachine StateMachine { internal set; get; }
        public CancellationToken LifetimeToken => StateLifetimeTokenSource.Token;
        public abstract UniTask Enter();
        public virtual UniTask Exit() => UniTask.CompletedTask;
        public virtual void Tick(float delta) { }
        public virtual void FixedTick(float delta) { }
        internal CancellationTokenSource StateLifetimeTokenSource { set; get; }

        public void Dispose()
        {
            StateLifetimeTokenSource?.Cancel();
            StateLifetimeTokenSource?.Dispose();
            StateLifetimeTokenSource = null;
        }
    }
}