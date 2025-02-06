namespace UState
{
    public abstract class ModelState<TModel> : State
    {
        public TModel Model { internal set; get; }
    }
}