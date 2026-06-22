using Infrastructure.Player;
using Infrastructure.Services.Haptic;
using Infrastructure.StateMachine.Game;
using VContainer.Unity;

namespace Infrastructure.Scopes
{
    public class GameEntryPoint: IStartable
    {
        private readonly GameStateMachine _gameStateMachine;

        public GameEntryPoint(GameStateMachine gameStateMachine, PlayerDataManager playerDataManager)
        {
            _gameStateMachine = gameStateMachine;
            HapticService.Initialize(playerDataManager);
        }
        
        public void Start()
        {
            _gameStateMachine.Enter<GameStartState>();
        }
    }
}