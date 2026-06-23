using Infrastructure.Services.Haptic;
using UnityEngine;
using VContainer;

namespace General.CustomButton
{
    public class ButtonFeedback: MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button _button;

        [Inject] private HapticService HapticService { get; set; }
        
        private void Awake()
        {
            _button.onClick.AddListener(PlayFeedback);
        }

        private void PlayFeedback()
        {
            //sounds
            HapticService.Play(HapticType.Button);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(PlayFeedback);
        }
    }
}