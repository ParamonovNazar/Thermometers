using System;
using System.Collections.Generic;
using Core.Level;
using UnityEngine;

namespace Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public GameSettings Settings { get; set; }
        [field: SerializeField] public List<LevelGameConfig> Levels { get; set; } = new List<LevelGameConfig>();
        [field: SerializeField] public List<LevelGameConfig> LoopedLevels { get; set; } = new List<LevelGameConfig>();
    }

    [Serializable]
    public class LevelGameConfig
    {
        [field: SerializeField] public LevelConfig LevelConfig { get; set; }
        //rewards etc
    }

    [Serializable]
    public class GameSettings
    {
        //parameters
    }
}