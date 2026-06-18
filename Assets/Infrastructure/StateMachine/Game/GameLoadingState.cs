using Cysharp.Threading.Tasks;
using Infrastructure.Configs;
using Infrastructure.Player;
using UnityEngine;

namespace Infrastructure.StateMachine.Game
{
    public class GameLoadingState : IGameState
    {
        private const float LOAD_PROGRESS_VALUE = 0.8f;

        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerDataManager _playerDataManager;
        
        public GameLoadingState(GameStateMachine gameStateMachine, PlayerDataManager playerDataManager)
        {
            _gameStateMachine = gameStateMachine;
            _playerDataManager = playerDataManager;
        }

        public void Enter()
        {
            LoadAndStart().Forget(Debug.LogException);
        }

        private async UniTask LoadAndStart()
        {
            _playerDataManager.LoadData();

            await InitializeServices();
            // await _environmentLoader.LoadLocation();
            _gameStateMachine.Enter<GameMetaState>();
        }

        private async UniTask InitializeServices()
        {
        }

        private void UpdateLoadingWindow(float configLoadingProgress)
        {
            LoadingScreen.Instance.UpdateProgress(configLoadingProgress * LOAD_PROGRESS_VALUE);
        }

        public void Exit()
        {
        }
    }
}