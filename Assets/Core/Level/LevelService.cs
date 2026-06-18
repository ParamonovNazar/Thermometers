using System;
using Infrastructure.Configs;
using Infrastructure.Player;

namespace Core.Level
{
     public class LevelService
    {
        private readonly LevelStorage _levelStorage;
        private readonly PlayerDataManager _playerDataManager;
        private readonly GameConfig _gameConfig;

        public LevelModel CurrentLevelModel { get; private set; }

        public event Action PreLevelComplete;
        public event Action OnLevelComplete;

        public LevelService(LevelStorage levelStorage, PlayerDataManager playerDataManager,
            GameConfig gameConfig)
        {
            _levelStorage = levelStorage;
            _playerDataManager = playerDataManager;
            _gameConfig = gameConfig;
        }

        public LevelGameConfig GetCurrentLevelConfig()
        {
            if (_playerDataManager.PlayerData.IsMainLevels)
            {
                //mod used just for safe
                return FindConfigs(_gameConfig.Levels[
                    _playerDataManager.CurrentLevel % _gameConfig.Levels.Count]);
            }

            return FindConfigs(_gameConfig.LoopedLevels[_playerDataManager.CurrentLoopedLevel % _gameConfig.LoopedLevels.Count]);
        }

        public LevelGameConfig FindConfigs(LevelGameConfig gameConfigLevel)
        {
            // if (gameConfigLevel.LevelConfig == null)
            // {
            //     gameConfigLevel.LevelConfig = _levelStorage.GetConfig(gameConfigLevel.ConfigId);
            // }
            
            CurrentLevelModel = new LevelModel(gameConfigLevel.LevelConfig);
            return gameConfigLevel;
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