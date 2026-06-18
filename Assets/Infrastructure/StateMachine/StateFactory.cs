using System;
using System.Collections.Generic;
using VContainer;

namespace Infrastructure.StateMachine
{
    public abstract class StateFactory : IStateFactory
    {
        private readonly Dictionary<Type, Func<IState>> _states;

        protected StateFactory(IObjectResolver resolver)
        {
            _states = BuildStatesRegister(resolver);
        }

        protected abstract Dictionary<Type, Func<IState>> BuildStatesRegister(IObjectResolver resolver);

        public IState Create(Type type)
        {
            if (!_states.TryGetValue(type, out Func<IState> state))
                throw new Exception($"State for {type.Name} can't be created");

            return state();
        }

        public T GetState<T>() where T : class, IState =>
            Create(typeof(T)) as T;
    }
}