using Infrastructure.Services.Haptic;
using UnityEngine;

namespace General.CustomButton
{
    public class ButtonFeedback: MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(PlayFeedback);
        }

        private void PlayFeedback()
        {
            //sounds
            HapticService.Instance.Play(HapticType.Button);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(PlayFeedback);
        }
    }
}