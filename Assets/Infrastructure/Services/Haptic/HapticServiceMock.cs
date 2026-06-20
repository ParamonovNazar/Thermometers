using UnityEngine;

namespace Infrastructure.Services.Haptic
{
    public class HapticServiceMock: IHapticService
    {
        public void Play(HapticType type)
        {
            Debug.Log($"HapticServiceMock: Play {type}");
        }
    }
}