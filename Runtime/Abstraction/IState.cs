namespace GameWarriors.StateMachineDomain.Abstraction
{
    public interface IState
    {
        void OnEnterState(IState pastState);
        void OnStateUpdate();
        void OnExitState(IState newState);
    }
}