using UnityEngine;

namespace Core.Level.Thermometer
{
    public abstract class ThermometerPartBase : MonoBehaviour
    {
        public float CurrentFillProgress { get; protected set; }

        public abstract void Setup(Color color);

        public virtual void SetFill(float progress)
        {
            CurrentFillProgress = progress;
        }

        public abstract void SetSize(float cellSize);

        public abstract void SetScale(float scale);

        public void SetActiveCross(bool active)
        {
            
        }
    }
}