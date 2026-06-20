using UnityEngine;
using UnityEngine.UI;

namespace Core.Level.Thermometer
{
    public class ThermometerPartBase : MonoBehaviour
    {
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private Image _fill;

        public float CurrentFillProgress { get; private set; }

        public virtual void Setup(Color color) => _fill.color = color;
        
        public virtual void SetFill(float progress)
        {
            CurrentFillProgress= progress;
            _fill.fillAmount = progress;
        }

        public void SetSize(float cellSize)
        {
            _rootRectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }
    }
}