using System;
using System.Collections.Generic;
using Core.Level;
using Infrastructure.General.Reward;
using UnityEngine;

namespace Infrastructure.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Config/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        [field: SerializeField] public GameSettings Settings { get; set; }
        public List<RewardInfo> Rewards { get; set; } = new List<RewardInfo>();
        [field: SerializeField] public List<LevelGameConfig> Levels { get; set; } = new List<LevelGameConfig>();
        [field: SerializeField] public List<LevelGameConfig> LoopedLevels { get; set; } = new List<LevelGameConfig>();


        public RewardDto GetRewardForLevel(int levelIndex)
        {
            return null;
            
            // foreach (var gameConfigLevel in Levels)
            // {
            //     if (gameConfigLevel.LevelIndex == levelIndex)
            //     {
            //         return gameConfigLevel.Reward;
            //     }
            // }
            //
            // Debug.LogError($"Cant find reward for level {levelIndex}");
            // return new RewardDto(new List<RewardItemDto>
            //     {
            //         new RewardItemDto
            //         {
            //             Amount = 100,
            //             Payload = RewardItemDto.REWARD_1,
            //             RewardType = RewardType.Reward1
            //         }
            //     }
            // );
        }

        public bool IsRewardIdValid(string rewardId)
        {
            foreach (var rewardInfo in Rewards)
            {
                if (rewardInfo.Id == rewardId)
                {
                    return true;
                }
            }

            return false;
        }

        public RewardDto GetReward(string rewardId)
        {
            foreach (var rewardInfo in Rewards)
            {
                if (rewardInfo.Id == rewardId)
                {
                    return rewardInfo.Rewards;
                }
            }

            Debug.LogError($"Can't find reward {rewardId}, returned 1 hint reward");

            return new RewardDto(new List<RewardItemDto>
            {
                new()
                {
                    RewardType = RewardType.Reward1,
                    Amount = 1,
                    Payload = RewardItemDto.REWARD_1
                }
            });
        }
    }

    [Serializable]
    public class LevelGameConfig
    {
        [field: SerializeField] public int LevelIndex { get; set; }
        [field: SerializeField] public LevelConfig LevelConfig { get; set; }
        // [field: SerializeField] public RewardDto Reward { get; set; }
        // [field: SerializeField] public string ConfigId { get; set; }
    }

    [Serializable]
    public class GameSettings
    {
    }

    [Serializable]
    public class RewardInfo
    {
        public string Id { get; set; }
        public RewardDto Rewards { get; set; }
    }
}