using System;

namespace Infrastructure.StateMachine
{
    public interface IStateMachine<TBaseState>
    {
        event Action OnStateChanged;
        Type ActiveStateType { get; }
        TState Enter<TState>() where TState : class, TBaseState, IState;
    }
}