using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Infrastructure.StateMachine.Game
{
    public class GameCoreEndState : IGameState
    {
        private readonly GameStateMachine _gameStateMachine;

        public GameCoreEndState(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        public void Enter()
        {
            TransitToNextState().Forget(Debug.LogException);
        }

        private async UniTask TransitToNextState()
        {
            var shouldStayInCore = true;

            if (shouldStayInCore)
            {
                _gameStateMachine.Enter<GameCoreState>();
            }
            else
            {
                _gameStateMachine.Enter<GameMetaState>();
            }
        }

        public void Exit()
        {
        }
    }
}