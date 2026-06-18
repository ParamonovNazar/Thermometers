namespace Infrastructure.StateMachine.Game
{
    public class GameStateMachine: StateMachine<IGameState>
    {
        public GameStateMachine(IStateFactory stateFactory) : base(stateFactory)
        {
        }
    }
}