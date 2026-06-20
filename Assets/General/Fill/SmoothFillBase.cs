using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.General.Fill
{
    public abstract class SmoothFillBase: MonoBehaviour
    {
        public event Action<float> OnChanged;

        public abstract void SetProgress(float progress);
        public abstract UniTask FillTo(float targetProgress, CancellationToken cancellationToken);

        protected void RaiseChange(float progress)
        {
            OnChanged?.Invoke(progress);
        }
    }
}