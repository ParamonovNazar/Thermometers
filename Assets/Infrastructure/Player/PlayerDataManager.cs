using System;
using UnityEngine;
using VContainer.Unity;
using Newtonsoft.Json;

namespace Infrastructure.Player
{
    public class PlayerDataManager : ITickable
    {
        private const string PLAYER_DATA_KEY = "playerSave";

        private const string MUSIC_MUTE_KEY = "MuteAudio";
        private const string SOUNDS_MUTE_KEY = "MuteSounds";
        private const string VIBARTIONS_MUTE_KEY = "MuteVibrations";

        public PlayerData PlayerData { get; private set; }

        public event Action OnPlayerDataSetup;

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

        public bool IsMusicMuted
        {
            get => PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1;
            set => PlayerPrefs.SetInt(MUSIC_MUTE_KEY, value ? 1 : 0);
        }

        public bool IsSoundsMuted
        {
            get => PlayerPrefs.GetInt(SOUNDS_MUTE_KEY, 0) == 1;
            set => PlayerPrefs.SetInt(SOUNDS_MUTE_KEY, value ? 1 : 0);
        }
        
        public bool IsVibrationsMuted
        {
            get => PlayerPrefs.GetInt(VIBARTIONS_MUTE_KEY, 0) == 1;
            set => PlayerPrefs.SetInt(VIBARTIONS_MUTE_KEY, value ? 1 : 0);
        }

        public void LoadData()
        {
            if (PlayerPrefs.HasKey(PLAYER_DATA_KEY))
            {
                try
                {
                    var playerData = JsonConvert.DeserializeObject<PlayerData>(PlayerPrefs.GetString(PLAYER_DATA_KEY));
                    PlayerData = playerData;
                    OnPlayerDataSetup?.Invoke();
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Can't parse player data");
                    Debug.LogException(e);
                }
            }
           
            PlayerData = CreateDefaultPlayerData();
            
            OnPlayerDataSetup?.Invoke();
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
                CurrentLevel = 0,
                IsMainLevels = true,
                CurrentLoopedLevel = 0,
            };
        }

        public void ClearPlayerData()
        {
            PlayerPrefs.DeleteKey(PLAYER_DATA_KEY);
            PlayerPrefs.DeleteKey(MUSIC_MUTE_KEY);
            PlayerPrefs.DeleteKey(SOUNDS_MUTE_KEY);
            PlayerPrefs.DeleteKey(VIBARTIONS_MUTE_KEY);
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
            PlayerPrefs.SetString(PLAYER_DATA_KEY, JsonConvert.SerializeObject(PlayerData));
        }
    }

    [Serializable]
    public class PlayerData
    {
        public bool IsMainLevels;
        public int CurrentLevel;
        public int CurrentLoopedLevel;
    }
}