using GameWarriors.StateMachineDomain.Abstraction;
using System.Collections.Generic;

namespace GameWarriors.StateMachineDomain.Data
{
    /// <summary>
    /// This class store state and corresponding transistions which quit from state.
    /// </summary>
    public class StateDataItem
    {
        public IState State { get; set; }
        public IList<ITransition> Transitions { get; private set; }

        public StateDataItem(IState state, IList<ITransition> transitions)
        {
            Transitions = transitions;
            State = state;
        }
    }
}