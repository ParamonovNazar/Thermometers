using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.General.Fill
{
    public class SmoothFill : SmoothFillBase
    {
        [SerializeField] private Image _fill;
        [SerializeField] private float _fillSpeed = 1f;

        public override void SetProgress(float progress)
        {
            ChangeFill(progress);
        }

        public override async UniTask FillTo(float targetProgress, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var delta = Mathf.Abs(targetProgress - _fill.fillAmount);
                var step = Time.deltaTime * _fillSpeed;

                if (delta < step)
                {
                    ChangeFill(targetProgress);
                    return;
                }

                ChangeFill(Mathf.MoveTowards(_fill.fillAmount, targetProgress, step));

                await UniTask.Yield();
            }
        }

        private void ChangeFill(float progress)
        {
            _fill.fillAmount = progress;
            RaiseChange(progress);
        }
    }
}