using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.General.Fill
{
    public class SmoothFillRect : SmoothFillBase
    {
        [SerializeField] private RectTransform _fill;
        [SerializeField] private float _fillSpeed = 1f;
        [SerializeField] private bool _horizontal = true;
        [SerializeField] private float _maxSize;

        public override void SetProgress(float progress)
        {
            ChangeFill(progress);
        }

        public override async UniTask FillTo(float targetProgress, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var current = (_horizontal ? _fill.sizeDelta.x : _fill.sizeDelta.y) / _maxSize;
                
                var delta = Mathf.Abs(targetProgress - current);
                var step = Time.deltaTime * _fillSpeed;

                if (delta < step)
                {
                    ChangeFill(targetProgress);
                    return;
                }

                ChangeFill(Mathf.MoveTowards(current, targetProgress, step));

                await UniTask.Yield();
            }
        }

        private void ChangeFill(float progress)
        {
            _fill.sizeDelta = new Vector2(_horizontal ? _maxSize * progress : _fill.sizeDelta.x,
                _horizontal ? _fill.sizeDelta.y : _maxSize * progress);

           RaiseChange(progress);
        }
    }
}