using UnityEngine;
using UnityEngine.UI;

namespace Core.Level.Thermometer
{
    public class ThermometerPartBase : MonoBehaviour
    {
        [SerializeField] private Image _fill;

        public float CurrentFillProgress { get; private set; }

        public virtual void Setup(Color color) => _fill.color = color;
        
        public virtual void SetFill(float progress)
        {
            CurrentFillProgress= progress;
            _fill.fillAmount = progress;
        }
    }
}