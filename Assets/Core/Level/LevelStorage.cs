using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [CreateAssetMenu(fileName = "LevelStorage", menuName = "Level/LevelStorage")]
    public class LevelStorage: ScriptableObject
    {
        public List<LevelConfig> Levels;

        public LevelConfig GetConfig(string id)
        {
            foreach (var levelConfig in Levels)
            {
                if (levelConfig.Id == id)
                {
                    return levelConfig;
                }
            }
            
            Debug.LogError($"Can't find level with id {id}");
            return Levels[0];
        }
    }
}