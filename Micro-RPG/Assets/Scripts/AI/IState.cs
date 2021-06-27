namespace AI
{
    public interface IState
    {
        void OnEnter();
        void Tick();
        void FixedTick();
        void OnExit();
    }
}
