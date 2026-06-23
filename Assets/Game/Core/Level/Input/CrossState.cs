using UnityEngine.EventSystems;

namespace Core.Level.Input
{
    public class CrossState : IInputState
    {
        private readonly LevelModel _levelModel;
        private readonly InteractionHelper _interactionHelper;

        private bool _isDragging;

        public DrawType DrawType { get; set; } = DrawType.Add;

        public CrossState(LevelModel levelModel, InteractionHelper interactionHelper)
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
                var currentState = _levelModel.GetCellState(cellPosition);
                DrawType = (currentState == CellState.Empty || currentState == CellState.Filled)
                    ? DrawType.Add
                    : DrawType.Remove;

                _isDragging = true;
                _interactionHelper.UpdateCross(cellPosition, DrawType);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (!_isDragging) return;

            if (_interactionHelper.TryFindCell(eventData, out var cellPosition))
            {
                _interactionHelper.UpdateCross(cellPosition, DrawType);
            }
        }

        public void Deactivate()
        {
            _isDragging = false;
        }
    }
}