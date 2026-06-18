using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.General.Fill;
using UnityEngine;

namespace Infrastructure
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private SmoothFillBase _smoothFill;
        [SerializeField] private float _hideTime;

        private CancellationTokenSource _cancellationTokenSource;
        public bool IsActive { get; set; } = true;
        public static LoadingScreen Instance { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }

        public void SetProgress(float progress)
        {
            _cancellationTokenSource?.Cancel();
            _smoothFill.SetProgress(progress);
        }

        public UniTask UpdateProgress(float progress)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            return _smoothFill.FillTo(progress, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }

        public async UniTask Complete()
        {
            IsActive = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            await _smoothFill.FillTo(1f, _cancellationTokenSource.Token).SuppressCancellationThrow();
            // _animator.SetTrigger("Hide");
            await UniTask.Delay(TimeSpan.FromSeconds(_hideTime), cancellationToken: _cancellationTokenSource.Token)
                .SuppressCancellationThrow();
            gameObject.SetActive(false);
        }
    }
}