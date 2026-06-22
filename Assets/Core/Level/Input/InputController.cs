using System;
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

        private LevelModel _levelModel;
        private ThermometerData _currentThermometer;
        private HapticService _hapticService;

        [field: SerializeField] public FillType FillType { get; set; } = FillType.Fill;
        [field: SerializeField] public DrawType DrawType { get; set; } = DrawType.Add;

        private IInputState _currentInputState;
        private FillState _fillState;
        private CrossState _crossState;
        private LifetimeScope _scope;
        private InteractionHelper _interactionHelper;

        [Inject]
        public void Constructor(HapticService hapticService)
        {
            _hapticService = hapticService;
        }

        public void Initialize(LevelModel levelModel)
        {
            _levelModel = levelModel;
            _interactionHelper = new InteractionHelper(_rectTransform, _levelModel, _hapticService);
            _fillState = new FillState(_levelModel, _interactionHelper);
            _crossState = new CrossState(_levelModel, _interactionHelper);
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
            _currentInputState.OnPointerDown(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _currentInputState.OnPointerUp(eventData);
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            _currentInputState.OnPointerMove(eventData);
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