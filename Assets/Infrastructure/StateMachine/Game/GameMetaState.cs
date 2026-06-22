using Cysharp.Threading.Tasks;
using Infrastructure.TransitionScreen;
using JetBrains.Annotations;
using Meta;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Infrastructure.StateMachine.Game
{
    [UsedImplicitly]
    public class GameMetaState : IGameState
    {
        private const string SCENE_NAME = "MetaScene";

        private readonly GameStateMachine _gameStateMachine;

        public bool IsInIdle { get; private set; }
        private bool IsFirstStateEnter { get; set; } = true;

        public MetaHud MetaHud { get; private set; }
        public bool ShouldAppear { get; set; }

        public GameMetaState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            IsInIdle = false;

            Load().Forget(Debug.LogException);
        }

        private async UniTask Load()
        {
            if (SceneManager.GetActiveScene().name != SCENE_NAME)
            {
                await SceneManager.LoadSceneAsync(SCENE_NAME).ToUniTask();
            }
            
            MetaHud = Object.FindFirstObjectByType<MetaHud>();
            MetaHud.OnPlayClicked += HandlePlayClick;
            MetaHud.UpdateView();
            if (ShouldAppear)
            {
                ShouldAppear = false;
                await MetaHud.Appear(true);
            }
            else
            {
                await MetaHud.Appear(false);
            }

            if (TransitionView.Instance.IsActive)
            {
                await TransitionView.Instance.Hide();
            }

            IsInIdle = true;
        }

        private void HandlePlayClick()
        {
            TransitToCore().Forget(Debug.LogException);
        }

        private async UniTask TransitToCore()
        {
            await TransitionView.Instance.Show();
            _gameStateMachine.Enter<GameCoreState>();
        }

        public void Exit()
        {
            MetaHud.OnPlayClicked -= HandlePlayClick;
            IsFirstStateEnter = false;
        }
    }
}