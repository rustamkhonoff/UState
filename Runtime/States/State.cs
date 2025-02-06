using Cysharp.Threading.Tasks;

namespace UState
{
    public abstract class State : ExitableState
    {
        public abstract UniTask Enter();
    }
}