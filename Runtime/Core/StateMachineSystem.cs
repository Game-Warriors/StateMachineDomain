using GameWarriors.StateMachineDomain.Abstraction;
using System.Collections.Generic;

namespace GameWarriors.StateMachineDomain.Core
{
    public class StateMachineSystem : IStateMachine
    {
        public readonly Dictionary<IState, IList<ITransition>> _dataTable;

        public IState CurrnetState { get; private set; }
        public IList<ITransition> StateTransitions { get; private set; }

#if UNITY_2018_4_OR_NEWER
        [UnityEngine.Scripting.Preserve]
#endif
        public StateMachineSystem()
        {
            _dataTable = new Dictionary<IState, IList<ITransition>>();
        }

        public void AddState(IState state, params ITransition[] transitions)
        {
            if (CurrnetState == null)
            {
                CurrnetState = state;
                StateTransitions = transitions;
            }
            _dataTable.TryAdd(state, transitions);
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
                            ActiveTransitions(_dataTable[state]);
                            CurrnetState.OnExitState(state);
                            state.OnEnterState(CurrnetState);
                            CurrnetState = state;
                            return;
                        }
                    }
                }
            }
        }

        //public void ChangeState(int stateID)
        //{
        //    if (_currentState != null)
        //        _currentState.ExitAction?.Invoke();

        //    _currentState = _states[stateID];
        //    _currentState.EnterAction?.Invoke();
        //}

        private void ActiveTransitions(IList<ITransition> transitions)
        {
            StateTransitions = transitions;
            int length = transitions.Count;
            for (int i = 0; i < length; ++i)
            {
                transitions[i].ActiveTransition();
            }
        }
    }
}