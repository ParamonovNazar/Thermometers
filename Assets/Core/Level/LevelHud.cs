using Core.Level.Input;
using General.Switch;
using UnityEngine;

namespace Core.Level
{
    public class LevelHud : MonoBehaviour
    {
        [SerializeField] private DoubleSwitch _modeSwitch;
        [SerializeField] private InputController _inputController;

        private void Awake()
        {
            _modeSwitch.InitializeFirstOption(false);
            _modeSwitch.OnOptionChanged += ChangeMode;
        }

        private void ChangeMode(bool cross)
        {
            _inputController.ChangeState(cross ? FillType.Cross : FillType.Fill);
        }

        private void OnDestroy()
        {
            _modeSwitch.OnOptionChanged -= ChangeMode;
        }
    }
}