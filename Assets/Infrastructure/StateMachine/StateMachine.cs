using System;
using UnityEngine;

namespace Infrastructure.StateMachine
{
    public abstract class StateMachine<TBaseState> : IStateMachine<TBaseState>
    {
        private readonly IStateFactory _stateFactory;
        private IState _currentState;
        public event Action OnStateChanged;
        public Type ActiveStateType => _currentState.GetType();
        public IState ActiveState => _currentState;

        protected StateMachine(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public TState Enter<TState>() where TState : class, TBaseState, IState
        {
            _currentState?.Exit();
            var state = _stateFactory.GetState<TState>();
            _currentState = state;
            _currentState.Enter();
            OnStateChanged?.Invoke();
            
            return state;
        }
    }
}