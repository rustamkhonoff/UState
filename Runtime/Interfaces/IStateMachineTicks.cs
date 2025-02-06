namespace UState
{
    public interface IStateMachineTicks
    {
        void Tick(float delta);
        void FixedTick(float delta);
    }
}