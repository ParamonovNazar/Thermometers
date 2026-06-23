using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Level.Thermometer
{
    public abstract class ThermometerPartBase : MonoBehaviour
    {
        [SerializeField] private GameObject _cross;
        [SerializeField] private AnimationCurve _crossShowCurve;
        [SerializeField] private AnimationCurve _crossHideCurve;
        [SerializeField] private float _crossAnimationDuration = 0.2f;

        private CancellationTokenSource _crossCts;

        public float CurrentFillProgress { get; protected set; }

        public abstract void Setup(Color color);

        public virtual void SetFill(float progress)
        {
            CurrentFillProgress = progress;
        }

        public abstract void SetSize(float cellSize);

        public abstract void SetScale(float scale);

        public abstract void SetBorderColor(Color color);

        public void SetActiveCross(bool active)
        {
            _crossCts?.Cancel();
            _crossCts?.Dispose();
            _crossCts = new CancellationTokenSource();

            AnimateCross(active, _crossCts.Token).Forget(Debug.LogException);
        }

        private async UniTask AnimateCross(bool active, CancellationToken token)
        {
            if (active == _cross.activeSelf)
            {
                return;
            }
            
            float startScale = Mathf.Clamp01(_cross.transform.localScale.x);
            float targetScale = active ? 1f : 0f;

            if (active)
            {
                _cross.SetActive(true);
            }
            else if (startScale <= 0.001f) // Already practically hidden
            {
                _cross.SetActive(false);
                return;
            }

            float elapsed = 0f;
            var curve = active ? _crossShowCurve : _crossHideCurve;

            while (elapsed < _crossAnimationDuration && !token.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _crossAnimationDuration);
                float curveValue = curve.Evaluate(t);
                float currentScale = Mathf.LerpUnclamped(startScale, targetScale, curveValue);
                _cross.transform.localScale = Vector3.one * currentScale;
                await UniTask.Yield(PlayerLoopTiming.Update, token).SuppressCancellationThrow();
            }

            if (token.IsCancellationRequested)
                return;

            _cross.transform.localScale = Vector3.one * targetScale;

            if (!active)
            {
                _cross.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            _crossCts?.Cancel();
            _crossCts?.Dispose();
        }
    }
}