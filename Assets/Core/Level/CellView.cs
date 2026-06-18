using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level
{
    public class CellView : MonoBehaviour
    {
        [SerializeField] private Image _background;
        [SerializeField] private Image _stateIcon;
        [SerializeField] private Button _button;

        public Button Button => _button;

        public void SetState(CellState state)
        {
            _stateIcon.enabled = state != CellState.Empty;
            if (state == CellState.Filled)
            {
                _stateIcon.color = Color.black; // Or some other color for filled
                // In a real game we'd use sprites for bulb/body/tip
            }
            else if (state == CellState.CrossedOut)
            {
                _stateIcon.color = Color.red; // X mark
            }
        }
    }
}
