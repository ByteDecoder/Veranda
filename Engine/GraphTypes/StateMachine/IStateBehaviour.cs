namespace RedOwl.Sleipnir.Graphs.StateMachine
{
    public interface IStateBehaviour
    {
        bool OnEnter();
        bool OnUpdate();
        void OnExit();
    }
}