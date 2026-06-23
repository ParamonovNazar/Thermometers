using UnityEngine;
using UnityEngine.UI;

namespace Core.Level.Thermometer
{
    public class ThermometerPart : ThermometerPartBase
    {
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private Transform _scaleRoot;
        [SerializeField] private Image _fill;
        [SerializeField] private Image _border;
        [SerializeField] private bool _scaleWidth = true;
        [SerializeField] private bool _scaleHeight = true;

        public override void Setup(Color color)
        {
            _fill.color = color;
        }

        public override void SetFill(float progress)
        {
            base.SetFill(progress);
            _fill.fillAmount = progress;
        }

        public override void SetSize(float cellSize)
        {
            _rootRectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }

        public override void SetScale(float scale)
        {
            _scaleRoot.localScale = new Vector3(_scaleWidth ? scale : 1f, _scaleHeight ? scale : 1f, 1f);
        }

        public override void SetBorderColor(Color color)
        {
            if (_border != null)
            {
                _border.color = color;
            }
        }
    }
}