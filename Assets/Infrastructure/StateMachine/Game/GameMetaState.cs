using Cysharp.Threading.Tasks;
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

        public MetaHud  MetaHud { get; private set; }
        
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
            await SceneManager.LoadSceneAsync(SCENE_NAME).ToUniTask();
            
            MetaHud = Object.FindFirstObjectByType<MetaHud>();

            MetaHud.OnPlayClicked += HandlePlayClick;
            
            if (LoadingScreen.Instance.IsActive)
            {
               await LoadingScreen.Instance.Complete();
            }
            
            if (TransitionScreen.Instance.IsActive)
            {
                await TransitionScreen.Instance.Hide();
            }
            
            IsInIdle = true;
        }

        private void HandlePlayClick()
        {
            TransitionScreen.Instance.Show().Forget(Debug.LogException);
            _gameStateMachine.Enter<GameCoreState>();
        }

        public void Exit()
        {
            MetaHud.OnPlayClicked -= HandlePlayClick;
            IsFirstStateEnter = false;
        }
    }
}