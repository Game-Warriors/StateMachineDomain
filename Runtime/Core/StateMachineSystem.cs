using GameWarriors.StateMachineDomain.Abstraction;
using GameWarriors.StateMachineDomain.Data;
using System.Collections.Generic;

namespace GameWarriors.StateMachineDomain.Core
{
    /// <summary>
    /// The main Implementation of system which is States container, update the current state and check its transitions condition to switch to transition target state.
    /// </summary>
    public class StateMachineSystem : IStateMachine
    {
        public readonly Dictionary<string, StateDataItem> _dataTable;

        public IState CurrnetState { get; private set; }
        public IList<ITransition> StateTransitions { get; private set; }

#if UNITY_2018_4_OR_NEWER
        [UnityEngine.Scripting.Preserve]
#endif
        public StateMachineSystem()
        {
            _dataTable = new Dictionary<string, StateDataItem>();
        }

        public void AddState(IState state, params ITransition[] transitions)
        {
            if (CurrnetState == null)
            {
                CurrnetState = state;
                StateTransitions = transitions;
            }
            StateDataItem dataItem = new StateDataItem(state, new List<ITransition>(transitions));
            _dataTable.TryAdd(state.Id, dataItem);
        }

        public void UpdateMachine()
        {
            if (CurrnetState != null)
            {
                CurrnetState.OnStateUpdate();
                int length = StateTransitions.Count;
                for (int i = 0; i < length; ++i)
                {
                    ITransition transition = StateTransitions[i];
                    if (transition.TransitionUpdate())
                    {
                        IState state = transition.TargetState;
                        if (state != null)
                        {
                            IList<ITransition> transitions = _dataTable[state.Id].Transitions;
                            ActiveTransitions(transitions);
                            CurrnetState.OnExitState(state);
                            state.OnEnterState(CurrnetState);
                            CurrnetState = state;
                            return;
                        }
                    }
                }
            }
        }

        private void ActiveTransitions(IList<ITransition> transitions)
        {
            StateTransitions = transitions;
            int length = transitions.Count;
            for (int i = 0; i < length; ++i)
            {
                transitions[i].OnTransitionActivate();
            }
        }

        public void ForceChangeState(string id)
        {
            if (_dataTable.TryGetValue(id, out StateDataItem item))
            {
                IState newState = item.State;
                IState oldState = CurrnetState;
                if (CurrnetState != null)
                    CurrnetState.OnExitState(newState);

                CurrnetState = newState;
                CurrnetState.OnEnterState(oldState);
            }
        }
    }
}