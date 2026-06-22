using System;
using System.Collections.Generic;
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

        [field: SerializeField] public FillType FillType { get; set; } = FillType.Fill;
        [field: SerializeField] public DrawType DrawType { get; set; } = DrawType.Add;

        private IInputState _currentInputState;
        private FillState _fillState;
        private CrossState _crossState;
        private LifetimeScope _scope;
        private InteractionHelper _interactionHelper;


        public void Initialize(LevelModel levelModel)
        {
            _levelModel = levelModel;
            _interactionHelper = new InteractionHelper(_rectTransform, _levelModel);
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

        public void Hint()
        {
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