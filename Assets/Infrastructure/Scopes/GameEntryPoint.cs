using Infrastructure.Player;
using Infrastructure.Services.Haptic;
using Infrastructure.StateMachine.Game;
using VContainer.Unity;

namespace Infrastructure.Scopes
{
    public class GameEntryPoint: IStartable
    {
        private readonly GameStateMachine _gameStateMachine;

        public GameEntryPoint(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }
        
        public void Start()
        {
            _gameStateMachine.Enter<GameStartState>();
        }
    }
}