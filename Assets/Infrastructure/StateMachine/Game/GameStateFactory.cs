using System;
using System.Collections.Generic;
using VContainer;

namespace Infrastructure.StateMachine.Game
{
    public class GameStateFactory : StateFactory
    {
        public GameStateFactory(IObjectResolver resolver) : base(resolver)
        {
        }

        protected override Dictionary<Type, Func<IState>> BuildStatesRegister(IObjectResolver resolver)
        {
            return new Dictionary<Type, Func<IState>>()
            {
                [typeof(GameStartState)] = () => resolver.Resolve<GameStartState>(),
                [typeof(GameLoadingState)] = () => resolver.Resolve<GameLoadingState>(),
                [typeof(GameCoreState)] = () => resolver.Resolve<GameCoreState>(),
                [typeof(GameCoreEndState)] = () => resolver.Resolve<GameCoreEndState>(),
                [typeof(GameMetaState)] = () => resolver.Resolve<GameMetaState>(),
            };
        }
    }
}