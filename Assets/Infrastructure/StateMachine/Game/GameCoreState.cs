using Core.Level;
using Cysharp.Threading.Tasks;
using Infrastructure.Configs;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Infrastructure.StateMachine.Game
{
    public class GameCoreState : IGameState
    {
        private const string SCENE_NAME = "CoreScene";

        private readonly GameStateMachine _gameStateMachine;
        private readonly LevelService _levelService;
        
        private LevelView _levelView;

        public LevelGameConfig CurrentLevelConfig { get; set; }

        public bool IsActive { get; private set; }

        public GameCoreState(GameStateMachine gameStateMachine, LevelService levelService)
        {
            _gameStateMachine = gameStateMachine;
            _levelService = levelService;
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
            _levelView = Object.FindAnyObjectByType<LevelView>();
            if (_levelView != null)
            {
                _levelView.Initialize(_levelService.CurrentLevelModel, OnCellClicked);
            }
        }

        private void OnCellClicked(Vector2Int coord)
        {
            if (!IsActive) return;
            
            _levelService.CurrentLevelModel.ToggleCell(coord);
            _levelView.UpdateCell(coord);

            if (_levelService.CurrentLevelModel.IsSolved())
            {
                Win();
            }
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
    }
}