using System;
using System.Linq;
using Core.Level.Thermometer;
using Cysharp.Threading.Tasks;
using Infrastructure.Services.Haptic;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using VContainer.Unity;

namespace Core.Level.Input
{
    public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [SerializeField] private LevelView _levelView;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Color _hintColor = Color.yellow;

        private LevelModel _levelModel;
        private ThermometerData _currentThermometer;
        private IInputState _currentInputState;
        private FillState _fillState;
        private CrossState _crossState;
        private InteractionHelper _interactionHelper;
        private InputStateFactory _inputStateFactory;
        
        [field: SerializeField] public FillType FillType { get; set; } = FillType.Fill;
        [field: SerializeField] public DrawType DrawType { get; set; } = DrawType.Add;
        public bool IsActive { get; set; }


        [Inject]
        public void Construct(InputStateFactory inputStateFactory)
        {
            _inputStateFactory = inputStateFactory;
        }

        public void Initialize(LevelModel levelModel)
        {
            _levelModel = levelModel;
            _interactionHelper = _inputStateFactory.CreateInteractionHelper(_rectTransform, _levelModel);
            _fillState = _inputStateFactory.CreateFillState(_levelModel, _interactionHelper);
            _crossState = _inputStateFactory.CreateCrossState(_levelModel, _interactionHelper);
            ChangeState(FillType);
        }

        public void ChangeState(FillType fillType)
        {
            _currentInputState?.Deactivate();

            switch (fillType)
            {
                case FillType.Fill:
                    _currentInputState = _fillState;
                    break;
                case FillType.Cross:
                    _currentInputState = _crossState;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fillType), fillType, null);
            }

            _currentInputState.Activate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsActive) return;

            _currentInputState.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsActive) return;

            _currentInputState.OnPointerUp(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!IsActive) return;

            _currentInputState.OnPointerMove(eventData);
        }

        public void Hint()
        {
            if(!IsActive) return;
            
            var incorrectThermometers = _levelModel.Thermometers
                .Where(t => _levelModel.GetThermometerFill(t) != t.SolutionFill)
                .ToList();

            if (incorrectThermometers.Count == 0) return;

            var randomThermometer = incorrectThermometers[UnityEngine.Random.Range(0, incorrectThermometers.Count)];

            _levelModel.SetThermometerFill(randomThermometer, randomThermometer.SolutionFill);
            _levelModel.BlockThermometer(randomThermometer);

            ThermometerView view = _levelView.GetThermometerView(randomThermometer);
            if (view != null)
            {
                view.IsBlocked = true;
                // view.Fill is called by HandleThermometerFillChanged in LevelView
                view.Highlight(_hintColor, 0.5f).Forget();
            }
        }
    }

    public enum FillType
    {
        Fill = 0,
        Cross = 1
    }

    public enum DrawType
    {
        Add = 0,
        Remove = 1
    }
}