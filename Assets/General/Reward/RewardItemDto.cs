using System;
using UnityEngine;

namespace Infrastructure.General.Reward
{
    [Serializable]
    public class RewardItemDto
    {
        public const string REWARD_1 ="reward";
        
        [field: SerializeField] public RewardType RewardType { get; set; }
        [field: SerializeField] public int Amount { get; set; }
        [field: SerializeField] public string Payload { get; set; }

        public RewardItemDto GetCopy()
        {
            return new RewardItemDto
            {
                RewardType = this.RewardType,
                Amount = this.Amount,
                Payload = this.Payload
            };
        }
    }
}