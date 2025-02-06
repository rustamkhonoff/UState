using Cysharp.Threading.Tasks;

namespace UState.Tests.Editor
{
    public class StateB : ModelState<StateB.StateModel>
    {
        public record StateModel(int Value);

        public override UniTask Enter()
        {
            return UniTask.CompletedTask;
        }
    }
}