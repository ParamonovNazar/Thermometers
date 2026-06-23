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
        [SerializeField] private RectTransform _connectionLine;
        [SerializeField] private RectTransform _layout;
        [SerializeField] private RectTransform _canvasRect;

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
            LayoutRebuilder.ForceRebuildLayoutImmediate(_layout);
            var currentLevelNumber = _levelService.GetCurrentLevelNumber();
            _playButtonLabel.text = $"Level {currentLevelNumber}";

            for (var index = 0; index < _upcomingLevelLabels.Count; index++)
            {
                _upcomingLevelLabels[index].text = $"{currentLevelNumber + index + 1}";
            }

            UpdateConnectionLine();
        }

        public void UpdateConnectionLine()
        {
            if (_upcomingLevelLabels.Count == 0) return;

            var startPoint = _playButton.GetComponent<RectTransform>().position;
            var endPoint = _upcomingLevelLabels[^1].rectTransform.position;

            var parent = _connectionLine.parent as RectTransform;
            if (parent == null) return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(null, startPoint), null, out var localStart);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, RectTransformUtility.WorldToScreenPoint(null, endPoint), null, out var localEnd);

            var direction = localEnd - localStart;
            var distance = direction.magnitude;

            _connectionLine.localPosition = localStart;
            _connectionLine.sizeDelta = new Vector2(_connectionLine.sizeDelta.x, distance);
            _connectionLine.up = _connectionLine.parent.TransformDirection(direction.normalized);
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