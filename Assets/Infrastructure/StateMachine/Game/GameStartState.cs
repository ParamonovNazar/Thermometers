using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.StateMachine.Game
{
    public class GameStartState : IGameState
    {
        public const string START_SCENE = "StartUpScene";
        private readonly GameStateMachine _gameStateMachine;

        public GameStartState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            LoadingScreen.Instance.SetProgress(0f);
            InitializeAnalytics().Forget(Debug.LogException);
        }

        private async UniTask InitializeAnalytics()
        {
            await RunGDPR();
            StartAnalytics();
            _gameStateMachine.Enter<GameLoadingState>();
        }

        private async UniTask RunGDPR()
        {
           return;
        }

        private void StartAnalytics()
        {
 
        }

        public void Exit()
        {
        }
    }
}