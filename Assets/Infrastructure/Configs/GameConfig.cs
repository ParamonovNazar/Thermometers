using System;
using System.Collections.Generic;
using Core.Level;
using Infrastructure.General.Reward;
using UnityEngine;

namespace Infrastructure.Configs
{
    public class GameConfig
    {
        public GameSettings Settings { get; set; }
        public List<RewardInfo> Rewards { get; set; } = new List<RewardInfo>();
        public List<LevelGameConfig> Levels { get; set; } = new List<LevelGameConfig>();
        public List<LevelGameConfig> LoopedLevels { get; set; } = new List<LevelGameConfig>();

        
        public RewardDto GetRewardForLevel(int levelIndex)
        {
            foreach (var gameConfigLevel in Levels)
            {
                if (gameConfigLevel.LevelIndex == levelIndex)
                {
                    return gameConfigLevel.Reward;
                }
            }

            Debug.LogError($"Cant find reward for level {levelIndex}");
            return new RewardDto(new List<RewardItemDto>
                {
                    new RewardItemDto
                    {
                        Amount = 100,
                        Payload = RewardItemDto.REWARD_1,
                        RewardType = RewardType.Reward1
                    }
                }
            );
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
        public int LevelIndex { get; set; }
        public RewardDto Reward { get; set; }
        public string ConfigId { get; set; }
        public LevelConfig LevelConfig { get; set; }
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