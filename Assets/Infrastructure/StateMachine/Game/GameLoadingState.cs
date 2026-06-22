using Cysharp.Threading.Tasks;
using Infrastructure.Player;
using Meta;
using UnityEngine;

namespace Infrastructure.StateMachine.Game
{
    public class GameLoadingState : IGameState
    {
        private readonly GameStateMachine _gameStateMachine;
        private readonly PlayerDataManager _playerDataManager;
        private readonly GameMetaState _gameMetaState;

        public GameLoadingState(GameStateMachine gameStateMachine, PlayerDataManager playerDataManager, GameMetaState gameMetaState)
        {
            _gameStateMachine = gameStateMachine;
            _playerDataManager = playerDataManager;
            _gameMetaState = gameMetaState;
        }

        public void Enter()
        {
            LoadAndStart().Forget(Debug.LogException);
        }

        private async UniTask LoadAndStart()
        {
            _gameMetaState.ShouldAppear = true;
            var metaHud = Object.FindFirstObjectByType<MetaHud>();
            metaHud.StartLoading();
            
            _playerDataManager.LoadData();
            //initialize services, load data
            
            _gameStateMachine.Enter<GameMetaState>();
        }
        
        public void Exit()
        {
        }
    }
}