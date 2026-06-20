using TMPro;
using UnityEngine;

namespace Core.Level
{
    public class ConstraintView : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI CurrentValueLabel { get; set; }
        [field: SerializeField] public TextMeshProUGUI TargetValueLabel { get; set; }

        [SerializeField] private Color _correctColor;
        [SerializeField] private Color _loverTargetColor;
        [SerializeField] private Color _higherTargetColor;

        public int CurrentValue { get; private set; }
        public int TargetValue { get; private set; }

        public void Setup(int currentValue, int targetValue)
        {
            SetCurrentValue(currentValue);
            SetTargetValue(targetValue);
        }

        public void SetCurrentValue(int currentValue)
        {
            CurrentValue = currentValue;
            CurrentValueLabel.text = currentValue.ToString();

            if (currentValue == TargetValue)
                CurrentValueLabel.color = _correctColor;

            if (currentValue > TargetValue)
                CurrentValueLabel.color = _higherTargetColor;

            if (currentValue < TargetValue)
                CurrentValueLabel.color = _loverTargetColor;
        }

        public void SetTargetValue(int targetValue)
        {
            TargetValue = targetValue;
            TargetValueLabel.text = targetValue.ToString();
        }
    }
}