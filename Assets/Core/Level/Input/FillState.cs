using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Level.Input
{
    public class FillState : IInputState
    {
        private readonly LevelModel _levelModel;
        private readonly InteractionHelper _interactionHelper;
        private ThermometerData _currentThermometer;

        public DrawType DrawType { get; set; } = DrawType.Add;
        
        public FillState(LevelModel levelModel, InteractionHelper interactionHelper)
        {
            _levelModel = levelModel;
            _interactionHelper = interactionHelper;
        }

        public void Activate()
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_interactionHelper.TryFindCell(eventData, out var cellPosition))
            {
                if (_levelModel.TryGetThermometer(cellPosition, out var thermometer))
                {
                    _currentThermometer = thermometer;
                    bool isFilled = _levelModel.GetCellState(cellPosition) == CellState.Filled;
                    DrawType = isFilled ? DrawType.Remove : DrawType.Add;

                    _interactionHelper.UpdateThermometerFill(thermometer, cellPosition, DrawType);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _currentThermometer = null;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_currentThermometer == null) return;

            if (_interactionHelper.TryFindCell(eventData, out var cellPosition))
            {
                if (_currentThermometer.Cells.Contains(cellPosition))
                {
                    _interactionHelper.UpdateThermometerFill(_currentThermometer, cellPosition, DrawType);
                }
            }
        }

        public void Deactivate()
        {
            _currentThermometer = null;
        }
    }
}