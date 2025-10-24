using Cysharp.Threading.Tasks;

namespace UState
{
    public interface IStateMachine
    {
        IState CurrentState { get; }
        UniTask Enter<TState>() where TState : State;
        UniTask Enter<TState, TModel>(TModel model) where TState : ModelState<TModel>;
    }
}