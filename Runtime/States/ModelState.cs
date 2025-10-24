namespace UState
{
    public abstract class ModelState<TModel> : BaseState
    {
        public TModel Model { internal set; get; }
    }
}