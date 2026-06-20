using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.General.Reward
{
    [Serializable]
    public class RewardDto
    {
        [SerializeField] private List<RewardItemDto> _rewards;

        public IReadOnlyList<RewardItemDto> Rewards => _rewards;

        public RewardDto(List<RewardItemDto> rewards)
        {
            _rewards = rewards;
        }

        public RewardDto Merge(RewardDto reward)
        {
            var rewards = new List<RewardItemDto>();

            foreach (var rewardItemDto in Rewards)
            {
                rewards.Add(rewardItemDto);
            }

            foreach (var orderRewardReward in reward.Rewards)
            {
                rewards.Add(orderRewardReward);
            }

            return new RewardDto(rewards);
        }

        public RewardDto GetCopy()
        {
            var rewards = new List<RewardItemDto>();

            foreach (var rewardItemDto in Rewards)
            {
                rewards.Add(rewardItemDto);
            }

            return new RewardDto(rewards);
        }
    }
}