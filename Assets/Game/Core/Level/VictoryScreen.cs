using Cysharp.Threading.Tasks;
using Infrastructure.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.Level
{
    public class VictoryScreen : MonoBehaviour
    {
        private static readonly int IsShowing = Animator.StringToHash("IsShowing");
        
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _description;
        [SerializeField] private Animator _animator;
        [SerializeField] private Button _continueButton;
        
        [Inject]
        public void Constructor(GameConfig config)
        {
            _header.text = config.VictoryScreenHeaders[Random.Range(0, config.VictoryScreenHeaders.Count)];
            _description.text =
                config.VictoryScreenDescriptions[Random.Range(0, config.VictoryScreenDescriptions.Count)];
        }
        
        public async UniTask Show()
        {
            _animator.SetBool(IsShowing, true);
            await _continueButton.OnClickAsync();
        }
    }
}