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
        [field: SerializeField] public List<IdColor> ColorPalette { get; set; } = new();
        [field: SerializeField] public Color FallbackThermometerColor { get; set; } = new();
        [field: SerializeField] public List<string> VictoryScreenHeaders { get; set; } = new List<string>();
        [field: SerializeField] public List<string> VictoryScreenDescriptions { get; set; } = new List<string>();
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

    [Serializable]
    public class IdColor
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public Color Color { get; set; }
    }
}