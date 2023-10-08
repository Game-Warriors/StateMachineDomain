namespace GameWarriors.StateMachineDomain.Abstraction
{
    /// <summary>
    /// The base abstraction for accessing the state machine system features like adding or force changing states.
    /// </summary>
    public interface IStateMachine
    {
        /// <summary>
        /// Adding new state to state container. the related transitions from this state to another state should pass when adding.
        /// </summary>
        /// <param name="state">source state</param>
        /// <param name="transitions">all transition which quit from source state</param>
        void AddState(IState state, params ITransition[] transitions);
        /// <summary>
        /// Force the system to change current state to requested state by state id, regardless of transitions. do noting when couldn't find target state state.
        /// </summary>
        /// <param name="id">target state Id</param>
        void ForceChangeState(string id);
        /// <summary>
        /// The main update loop of system. all system operations and checking will process in this method and the method should called by program core update loop.
        /// </summary>
        void UpdateMachine();
    }
}