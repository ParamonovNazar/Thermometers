using Core.Level;
using Cysharp.Threading.Tasks;
using Infrastructure.Configs;
using Infrastructure.TransitionScreen;
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
        private readonly GameConfig _gameConfig;

        private LevelContext _levelContext;

        public LevelGameConfig CurrentLevelConfig { get; set; }

        public bool IsActive { get; private set; }

        public GameCoreState(GameStateMachine gameStateMachine, LevelService levelService, GameConfig gameConfig)
        {
            _gameStateMachine = gameStateMachine;
            _levelService = levelService;
            _gameConfig = gameConfig;
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

            if (TransitionView.Instance.IsActive)
            {
                await TransitionView.Instance.Hide();
            }

            IsActive = true;
        }

        private void LoadLevel(LevelGameConfig currentLevelConfig)
        {
            _levelContext = Object.FindAnyObjectByType<LevelContext>();
            if (_levelContext != null)
            {
                var model = _levelService.CurrentLevelModel;
                _levelContext.RebuildLayout();
                _levelContext.LevelView.Initialize(model, _gameConfig);
                _levelContext.InputController.Initialize(model);
                _levelContext.InputController.IsActive = true;

                model.OnLevelSolved += Win;
            }
        }

        private void Win()
        {
            _levelContext.InputController.IsActive = false;
            if (_levelService.CurrentLevelModel != null)
            {
                _levelService.CurrentLevelModel.OnLevelSolved -= Win;
            }

            _levelService.CompleteLevel();
            ShowVictoryScreen().Forget(Debug.LogException);
        }

        private async UniTask ShowVictoryScreen()
        {
            await _levelContext.VictoryScreen.Show();
            await TransitionView.Instance.Show();
            _gameStateMachine.Enter<GameCoreState>();
        }

        private async UniTask TransitToMeta()
        {
            await TransitionView.Instance.Show();
            _gameStateMachine.Enter<GameMetaState>();
        }

        public void Exit()
        {
            IsActive = false;
        }

        public void ReturnToMeta()
        {
            TransitToMeta().Forget(Debug.LogException);
        }
    }
}