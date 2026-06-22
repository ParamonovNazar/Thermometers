using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.StateMachine.Game
{
    public class GameStartState : IGameState
    {
        public const string START_SCENE = "MetaScene";
        private readonly GameStateMachine _gameStateMachine;

        public GameStartState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            InitializeAnalytics().Forget(Debug.LogException);
        }

        private async UniTask InitializeAnalytics()
        {
            await RunGDPR();
            StartAnalytics();
            _gameStateMachine.Enter<GameLoadingState>();
        }

        private UniTask RunGDPR()
        {
            return UniTask.CompletedTask;
        }

        private void StartAnalytics()
        {
 
        }

        public void Exit()
        {
        }
    }
}