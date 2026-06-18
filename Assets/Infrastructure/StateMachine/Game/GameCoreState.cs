using Core.Level;
using Cysharp.Threading.Tasks;
using Infrastructure.Configs;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Infrastructure.StateMachine.Game
{
    public class GameCoreState : IGameState, ITickable
    {
        private const string SCENE_NAME = "CoreScene";

        private readonly GameStateMachine _gameStateMachine;
        private readonly LevelService _levelService;
        private readonly GameConfigManager _gameConfigManager;

        public LevelGameConfig CurrentLevelConfig { get; set; }

        public bool IsActive { get; private set; }

        public GameCoreState(GameStateMachine gameStateMachine, LevelService levelService, GameConfigManager gameConfigManager)
        {
            _gameStateMachine = gameStateMachine;
            _levelService = levelService;
            _gameConfigManager = gameConfigManager;
        }

        public void Enter()
        {
            Load().Forget(Debug.LogException);
        }

        private async UniTask Load()
        {
            await SceneManager.LoadSceneAsync(SCENE_NAME).ToUniTask();
            CurrentLevelConfig = _levelService.GetCurrentLevelConfig();
            LoadLevel(CurrentLevelConfig);

            if (LoadingScreen.Instance.IsActive)
            {
                await LoadingScreen.Instance.Complete();
            }

            if (TransitionScreen.Instance.IsActive)
            {
                await TransitionScreen.Instance.Hide();
            }

            IsActive = true;
        }

        private void LoadLevel(LevelGameConfig currentLevelConfig)
        {
        }

        private void Win()
        {
            _levelService.CompleteLevel();
            TransitionScreen.Instance.Show().Forget(Debug.LogException);
            _gameStateMachine.Enter<GameCoreEndState>();
        }

        public void Exit()
        {
            IsActive = false;
        }

        public void Tick()
        {
            if (!IsActive)
            {
                return;
            }
            
            if (Input.GetKeyDown(KeyCode.E)) 
                Win();
        }
    }
}