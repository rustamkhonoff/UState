using System.Threading;
using Cysharp.Threading.Tasks;

namespace UState
{
    public abstract class ExitableState : IState
    {
        public virtual UniTask Exit() => UniTask.CompletedTask;
        public virtual void Tick(float delta) { }
        public virtual void FixedTick(float delta) { }
        public IStateMachine StateMachine { internal set; get; }
        internal CancellationTokenSource StateLifetimeTokenSource { set; get; }
        public CancellationToken LifetimeToken => StateLifetimeTokenSource.Token;
    }
}