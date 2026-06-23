using Core.Level.Input;
using General.Switch;
using Infrastructure.StateMachine.Game;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core.Level
{
    public class LevelHud : MonoBehaviour
    {
        [SerializeField] private DoubleSwitch _modeSwitch;
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private InputController _inputController;
        [SerializeField] private Button _backButton;
        
        private GameCoreState _gameCoreState;

        [Inject]
        public void Constructor(GameCoreState gameCoreState)
        {
            _gameCoreState = gameCoreState;
        }
        
        private void Awake()
        {
            _modeSwitch.InitializeFirstOption(false);
            _modeSwitch.OnOptionChanged += ChangeMode;
            _hintButton.onClick.AddListener(Hint);
            _undoButton.onClick.AddListener(Undo);
            _backButton.onClick.AddListener(ReturnToMeta);
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
        
        private void ReturnToMeta()
        {
            //confirmation dialog?
            _gameCoreState.ReturnToMeta();
        }

        private void OnDestroy()
        {
            _modeSwitch.OnOptionChanged -= ChangeMode;
            _hintButton.onClick.RemoveListener(Hint);
            _undoButton.onClick.RemoveListener(Undo);
            _backButton.onClick.RemoveListener(ReturnToMeta);
        }
    }
}