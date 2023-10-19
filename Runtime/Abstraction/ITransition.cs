namespace GameWarriors.StateMachineDomain.Abstraction
{
    /// <summary>
    /// The base abstraction which state has ti implelment, the state machine system could work with the state
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// The target state which will move to when condition was met.
        /// </summary>
        IState TargetState { get; }
        /// <summary>
        /// Calling each time, when the host state selects as current state, and the state transitions consider in possible transition.
        /// </summary>
        void OnTransitionActivate();

        /// <summary>
        /// Calling each time, when the host state end or exit and current state changed.
        /// </summary>
        void OnTransitionDeactivate();
        /// <summary>
        /// Calling on each system update call when the transition bind to active state which state is current state in system.
        /// </summary>
        /// <returns>returning true if transition should be applied</returns>
        bool TransitionUpdate();
    }
}
