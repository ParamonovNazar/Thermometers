using System;
using System.Collections.Generic;
using Tools.DataSave;
using Tools.Logger;
using UnityEngine;
using VContainer.Unity;
using Newtonsoft.Json;

namespace Infrastructure.Player
{
    public class PlayerDataManager : ITickable
    {
        private const string PLAYER_DATA = "playerSave";

        private const string MUTE = "MuteAudio";
        private const string SOUNDS_MUTE = "MuteSounds";
        private const string VIBARTIONS_MUTE = "MuteVibrations";

        private readonly PersistentDataController _persistentDataController;
        public PlayerData PlayerData { get; private set; }

        public event Action OnPlayerDataSetuped;

        private bool _saveRequested;

        public int CurrentLevel
        {
            get
            {
                if (PlayerData == null)
                {
                    Debug.LogError("Tried to get level from not loaded playerData");
                    return 0;
                }

                return PlayerData.CurrentLevel;
            }
        }

        public int CurrentLoopedLevel
        {
            get
            {
                if (PlayerData == null)
                {
                    Debug.LogError("Tried to get level from not loaded playerData");
                    return 0;
                }

                return PlayerData.CurrentLoopedLevel;
            }
        }

        public int LocationInProgress
        {
            get
            {
                if (PlayerData == null)
                {
                    Debug.LogError("Tried to get location from not loaded playerData");
                    return 0;
                }

                return PlayerData.LocationInProgress;
            }
        }

        public bool IsMuted
        {
            get => PlayerPrefs.GetInt(MUTE, 0) == 1;
            set => PlayerPrefs.SetInt(MUTE, value ? 1 : 0);
        }

        public bool IsSoundsMuted
        {
            get => PlayerPrefs.GetInt(SOUNDS_MUTE, 0) == 1;
            set => PlayerPrefs.SetInt(SOUNDS_MUTE, value ? 1 : 0);
        }
        
        public bool IsVibrationsMuted
        {
            get => PlayerPrefs.GetInt(VIBARTIONS_MUTE, 0) == 1;
            set => PlayerPrefs.SetInt(VIBARTIONS_MUTE, value ? 1 : 0);
        }

        public PlayerDataManager()
        {
            _persistentDataController = new PersistentDataController(Application.persistentDataPath, PLAYER_DATA,
                new UnityDebugLogger(), 1);
        }

        public void LoadData()
        {
            if (_persistentDataController.TryLoad(out var json))
            {
                try
                {
                    var playerData = JsonConvert.DeserializeObject<PlayerData>(json);
                    PlayerData = playerData;
                    OnPlayerDataSetuped?.Invoke();
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Can't parse player data");
                    Debug.LogException(e);
                }
            }
            
            PlayerData = CreateDefaultPlayerData();
            
            OnPlayerDataSetuped?.Invoke();
        }

        public void SetSkipTutorialData()
        {
            PlayerData = CreateDefaultPlayerData();
            SaveDataInternal();
        }

        public void SaveData(bool force = false)
        {
            if (force)
            {
                SaveDataInternal();
                _saveRequested = false;
                return;
            }

            _saveRequested = true;
        }

        public void UpdateCurrentLevel(int currentLevel)
        {
            PlayerData.CurrentLevel = currentLevel;
            SaveData();
        }

        public void UpdateCurrentLooperLevel(int currentLoopedLevel)
        {
            PlayerData.CurrentLoopedLevel = currentLoopedLevel;
            SaveData();
        }

        private PlayerData CreateDefaultPlayerData()
        {
            return new PlayerData
            {
                CurrentBuildCurrency = 100,
                LocationInProgress = 0,
                CurrentLevel = 0,
                IsMainLevels = true,
                IsMainLocations = true,
                CurrentLoopedLevel = 0,
            };
        }

        public void ClearPlayerData()
        {
            _persistentDataController.ClearAllData();
            PlayerPrefs.DeleteAll();
        }

        void ITickable.Tick()
        {
            if (_saveRequested)
            {
                _saveRequested = false;
                SaveDataInternal();
            }
        }

        private void SaveDataInternal()
        {
            if (!_persistentDataController.TrySaveData(JsonConvert.SerializeObject(PlayerData)))
            {
                Debug.LogError($"Can't save player data");
            }
        }
    }

    [Serializable]
    public class PlayerData
    {
        public int CurrentBuildCurrency;

        public bool IsMainLevels;
        public int CurrentLevel;
        public int CurrentLoopedLevel;

        public int LocationInProgress;
        public int LocationInProgressLooped;
        public bool IsMainLocations;
    }
}