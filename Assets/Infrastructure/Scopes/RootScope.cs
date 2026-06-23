using Core.Level;
using Infrastructure.Configs;
using Infrastructure.Player;
using Infrastructure.Services.Haptic;
using Infrastructure.StateMachine.Game;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scopes
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private GameConfig _gameConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerDataManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<LevelService>(Lifetime.Singleton);
            builder.Register(resolver => resolver.Resolve<LevelService>().CurrentLevelModel, Lifetime.Singleton).AsSelf();
            builder.RegisterInstance(_gameConfig);

            ConfigureStates(builder);

            builder.RegisterEntryPoint<GameEntryPoint>();
        }

        private void ConfigureStates(IContainerBuilder builder)
        {
            builder.Register<GameStateFactory>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameStartState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameLoadingState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameMetaState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameCoreState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameCoreEndState>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<GameStateMachine>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}