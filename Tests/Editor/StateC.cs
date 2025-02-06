using Cysharp.Threading.Tasks;

namespace UState.Tests.Editor
{
    public class StateC : State
    {
        public int Value;

        public override UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        public override void Tick(float delta)
        {
            Value += 1;
        }
    }
}