using System;
using System.Collections.Generic;
using Core.Level;
using Cysharp.Threading.Tasks;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Meta
{
    public class MetaHud : MonoBehaviour
    {
        private static readonly int IsShow = Animator.StringToHash("IsShow");
        private LevelService _levelService;
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _playButtonLabel;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _appearTime;

        [SerializeField] private List<TextMeshProUGUI> _upcomingLevelLabels;

        [field: SerializeField] public LoadingScreen LoadingScreen { get; private set; }

        public event Action OnPlayClicked;

        private void Awake()
        {
            _playButton.onClick.AddListener(HandlePlayClick);
        }

        [Inject]
        public void Constructor(LevelService levelService)
        {
            _levelService = levelService;
        }

        public void UpdateView()
        {
            var currentLevelNumber = _levelService.GetCurrentLevelNumber();
            _playButtonLabel.text = $"Level {currentLevelNumber}";

            for (var index = 0; index < _upcomingLevelLabels.Count; index++)
            {
                _upcomingLevelLabels[index].text = $"{currentLevelNumber + index + 1}";
            }
        }

        private void HandlePlayClick()
        {
            OnPlayClicked?.Invoke();
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(HandlePlayClick);
        }

        public void StartLoading()
        {
            LoadingScreen.SetProgress(0);
        }

        public async UniTask Appear(bool smooth)
        {
            if (smooth)
            {
                await LoadingScreen.Complete();
                _animator.SetBool(IsShow, true);
                await UniTask.Delay(TimeSpan.FromSeconds(_appearTime));
                return;
            }
         
            _animator.SetBool(IsShow, true);
            _animator.Play("Idle");
        }
    }
}