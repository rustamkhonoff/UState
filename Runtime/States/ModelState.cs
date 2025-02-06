using Cysharp.Threading.Tasks;

namespace UState
{
    public abstract class ModelState<TModel> : ExitableState
    {
        public TModel Model { internal set; get; }
        public abstract UniTask Enter();
    }
}