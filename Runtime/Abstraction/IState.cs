namespace GameWarriors.StateMachineDomain.Abstraction
{
    /// <summary>
    /// The base abstraction which state has ti implelment, the state machine system could work with the state
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// The unique id of the state in state machine system.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Calling when the system switch to this state each time.
        /// </summary>
        /// <param name="pastState"></param>
        void OnEnterState(IState pastState);
        /// <summary>
        /// Calling on each system update call when the state is active and is as current state in system.
        /// </summary>
        void OnStateUpdate();
        /// <summary>
        /// Calling when the system switch from this state to another state.
        /// </summary>
        /// <param name="newState"></param>
        void OnExitState(IState newState);
    }
}