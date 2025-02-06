using Cysharp.Threading.Tasks;

namespace UState
{
    public interface IStateMachine
    {
        ExitableState CurrentState { get; }
        UniTaskVoid Enter<TState>() where TState : State;
        UniTaskVoid Enter<TState, TModel>(TModel model) where TState : ModelState<TModel>;
    }
}