namespace GameWarriors.StateMachineDomain.Abstraction
{
    public interface ITransition
    {
        IState TargetState { get; }
        void ActiveTransition();
        bool TransitionUpdate();
    }
}
