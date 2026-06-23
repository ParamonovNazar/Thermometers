using System;
using Infrastructure.Configs;
using Infrastructure.Player;

namespace Core.Level
{
    public class LevelService
    {
        private readonly PlayerDataManager _playerDataManager;
        private readonly GameConfig _gameConfig;

        public event Action PreLevelComplete;
        public event Action OnLevelComplete;

        public LevelService(PlayerDataManager playerDataManager,
            GameConfig gameConfig)
        {
            _playerDataManager = playerDataManager;
            _gameConfig = gameConfig;
        }

        public LevelGameConfig GetCurrentLevelConfig()
        {
            if (_playerDataManager.PlayerData.IsMainLevels)
            {
                //mod used just for safe
                return _gameConfig.Levels[
                    _playerDataManager.CurrentLevel % _gameConfig.Levels.Count];
            }

            return _gameConfig.LoopedLevels[_playerDataManager.CurrentLoopedLevel % _gameConfig.LoopedLevels.Count];
        }

        public void CompleteLevel()
        {
            PreLevelComplete?.Invoke();

            if (_playerDataManager.PlayerData.IsMainLevels)
            {
                _playerDataManager.UpdateCurrentLevel(_playerDataManager.CurrentLevel + 1);

                if (_playerDataManager.CurrentLevel >= _gameConfig.Levels.Count)
                {
                    _playerDataManager.PlayerData.IsMainLevels = false;
                }
            }
            else
            {
                _playerDataManager.UpdateCurrentLooperLevel(_playerDataManager.CurrentLoopedLevel + 1);

                if (_playerDataManager.CurrentLevel < _gameConfig.Levels.Count)
                {
                    _playerDataManager.PlayerData.IsMainLevels = true;
                }
            }

            OnLevelComplete?.Invoke();
        }

        public int GetCurrentLevelNumber()
        {
            return _playerDataManager.CurrentLevel + _playerDataManager.CurrentLoopedLevel + 1;
        }

        public int GetLoop()
        {
            if (_playerDataManager.PlayerData.IsMainLevels)
            {
                return 1;
            }

            return 1 + _playerDataManager.CurrentLoopedLevel / _gameConfig.LoopedLevels.Count;
        }
    }
}