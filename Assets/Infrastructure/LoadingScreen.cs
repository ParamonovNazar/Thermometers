using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.General.Fill;
using UnityEngine;

namespace Infrastructure
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private SmoothFillBase _smoothFill;

        private CancellationTokenSource _cancellationTokenSource;

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
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
            await _smoothFill.FillTo(1f, _cancellationTokenSource.Token).SuppressCancellationThrow();
        }
    }
}