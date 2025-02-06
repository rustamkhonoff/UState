using Cysharp.Threading.Tasks;

namespace UState.Tests.Editor
{
    public class StateA : State
    {
        public override UniTask Enter()
        {
            return UniTask.CompletedTask;
        }
    }
}