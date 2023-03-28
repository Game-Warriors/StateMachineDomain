namespace GameWarriors.StateMachineDomain.Abstraction
{
    public interface IStateMachine
    {
        void AddState(IState state, params ITransition[] transitions);
        void UpdateMachine();
    }
}