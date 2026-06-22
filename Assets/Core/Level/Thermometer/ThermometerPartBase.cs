using UnityEngine;

namespace Core.Level.Thermometer
{
    public abstract class ThermometerPartBase : MonoBehaviour
    {
        [SerializeField] private GameObject _cross;
        
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
            _cross.SetActive(active);
        }
    }
}