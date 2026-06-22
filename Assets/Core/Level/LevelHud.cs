using Core.Level.Input;
using General.Switch;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level
{
    public class LevelHud : MonoBehaviour
    {
        [SerializeField] private DoubleSwitch _modeSwitch;
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private InputController _inputController;

        private void Awake()
        {
            _modeSwitch.InitializeFirstOption(false);
            _modeSwitch.OnOptionChanged += ChangeMode;
            _hintButton.onClick.AddListener(Hint);
            _undoButton.onClick.AddListener(Undo);
        }

        private void ChangeMode(bool cross)
        {
            _inputController.ChangeState(cross ? FillType.Cross : FillType.Fill);
        }

        private void Hint()
        {
            _inputController.Hint();
        }
        
        private void Undo()
        {
            //not ready yet
        }

        private void OnDestroy()
        {
            _modeSwitch.OnOptionChanged -= ChangeMode;
            _hintButton.onClick.RemoveListener(Hint);
            _undoButton.onClick.RemoveListener(Undo);
        }
    }
}