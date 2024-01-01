using GameWarriors.StateMachineDomain.Abstraction;
using GameWarriors.StateMachineDomain.Data;
using System;
using System.Collections.Generic;

namespace GameWarriors.StateMachineDomain.Core
{
    /// <summary>
    /// The main Implementation of system which is States container, update the current state and check its transitions condition to switch to transition target state.
    /// </summary>
    public class StateMachineSystem : IStateMachine
    {
        public event Action<IState, IState> OnStateChanged;
        private readonly Dictionary<string, StateDataItem> _dataTable;

        public IState CurrentState { get; private set; }
        public IList<ITransition> CurrentStateTransitions { get; private set; }

#if UNITY_2018_4_OR_NEWER
        [UnityEngine.Scripting.Preserve]
#endif
        public StateMachineSystem()
        {
            _dataTable = new Dictionary<string, StateDataItem>();
        }

        public void AddState(IState state, params ITransition[] transitions)
        {
            if (CurrentState == null)
            {
                CurrentState = state;
                CurrentStateTransitions = transitions;
                ActiveTransitions(transitions);
                state.OnEnterState(null);
                OnStateChanged?.Invoke(null, state);
            }
            StateDataItem dataItem = new StateDataItem(state, new List<ITransition>(transitions));
            _dataTable.Add(state.Id, dataItem);
        }

        public void UpdateMachine()
        {
            if (CurrentState != null)
            {
                CurrentState.OnStateUpdate();
                int length = CurrentStateTransitions.Count;
                for (int i = 0; i < length; ++i)
                {
                    ITransition transition = CurrentStateTransitions[i];
                    if (transition.TransitionUpdate())
                    {
                        IState newState = transition.TargetState;
                        if (newState != null)
                        {
                            DeactiveTransitions(CurrentStateTransitions);
                            ChangeState(CurrentState, newState);
                            return;
                        }
                    }
                }
            }
        }

        private void ActiveTransitions(IList<ITransition> transitions)
        {
            CurrentStateTransitions = transitions;
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
                DeactiveTransitions(CurrentStateTransitions);
                ChangeState(CurrentState, newState);
            }
        }

        public void ClearStateMachine()
        {
            _dataTable.Clear();
            CurrentStateTransitions.Clear();
            CurrentState = null;
        }

        private void ChangeState(IState oldState, IState newState)
        {
            IList<ITransition> transitions = _dataTable[newState.Id].Transitions;
            ActiveTransitions(transitions);
            CurrentState?.OnExitState(newState);
            CurrentState = newState;
            newState.OnEnterState(oldState);
            OnStateChanged?.Invoke(oldState, newState);
        }

        private void DeactiveTransitions(IList<ITransition> transitions)
        {
            int length = transitions.Count;
            for (int i = 0;i< length;++i)
            {
                transitions[i].OnTransitionDeactivate();
            }
        }

        public bool RemoveState(IState state)
        {
            if (state == null)
                throw new ArgumentNullException("the input state is invalid");

            bool isRemoved = _dataTable.Remove(state.Id);
            if ( isRemoved)
            {
                foreach (var item in _dataTable.Values)
                {
                    int length = item.Transitions?.Count ?? 0;
                    for (int i = 0; i < length; ++i)
                    {
                        if (item.Transitions[i].TargetState == state)
                        {
                            item.Transitions.RemoveAt(i);
                            --i;
                        }
                    }
                }

                if (state == CurrentState)
                {
                    foreach (var item in _dataTable.Values)
                    {
                        ChangeState(CurrentState, item.State);
                        break;
                    }
                }
                return true;
            }
            return false;
        }
    }
}