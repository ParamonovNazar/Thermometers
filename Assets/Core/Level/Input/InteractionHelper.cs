using Infrastructure.Services.Haptic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Level.Input
{
    public class InteractionHelper
    {
        private readonly RectTransform _rectTransform;
        private readonly LevelModel _levelModel;

        public InteractionHelper(RectTransform rectTransform, LevelModel levelModel)
        {
            _rectTransform = rectTransform;
            _levelModel = levelModel;
        }

        public bool TryFindCell(PointerEventData eventData, out Vector2Int cellPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position,
                    eventData.pressEventCamera, out var localPoint))
            {
                var rect = _rectTransform.rect;

                // Calculate normalized position (0 to 1) from bottom-left
                float normalizedX = (localPoint.x - rect.xMin) / rect.width;
                float normalizedY = (localPoint.y - rect.yMin) / rect.height;

                int gridWidth = _levelModel.Width + 1;
                int gridHeight = _levelModel.Height + 1;

                int xGrid = Mathf.FloorToInt(normalizedX * gridWidth);
                int yGrid = Mathf.FloorToInt(normalizedY * gridHeight);

                // xGrid: 0 is row constraint, 1..Width are level cells
                // yGrid: 0..Height-1 are level cells, Height is column constraint
                int x = xGrid - 1;
                int y = yGrid;

                // Debug.Log($"[DEBUG_LOG] Input at xGrid:{xGrid}, yGrid:{yGrid} -> x:{x}, y:{y}");

                if (x >= 0 && x < _levelModel.Width && y >= 0 && y < _levelModel.Height)
                {
                    // Debug.Log($"Cell touched: {x}, {y}");
                    cellPosition = new Vector2Int(x, y);
                    return true;
                }
            }

            cellPosition = default;
            return false;
        }
        
        public void UpdateThermometerFill(ThermometerData thermometer, Vector2Int cellPosition, DrawType drawType)
        {
            var drawModifier = drawType == DrawType.Remove ? 0 : 1;
            var length = Mathf.Max(0, thermometer.GetLengthToCell(cellPosition) + drawModifier);
            
            if (_levelModel.GetThermometerFill(thermometer) == length)
            {
                return;
            }

            _levelModel.SetThermometerFill(thermometer, length);
            
            HapticService.Instance.Play(HapticType.ThermometerInteraction);
        }

        public void UpdateCross(Vector2Int cellPosition, DrawType drawType)
        {
            if (_levelModel.TryGetThermometer(cellPosition, out var thermometer))
            {
                int index = thermometer.GetLengthToCell(cellPosition);
                if (drawType == DrawType.Add)
                {
                    _levelModel.SetThermometerCross(thermometer, index);
                }
                else
                {
                    _levelModel.ClearThermometerCross(thermometer, index);
                }
            }
            else
            {
                //is it ok?
                Debug.LogWarning($"Cell {cellPosition} is not part of any thermometer");
                var newState = drawType == DrawType.Add ? CellState.CrossedOut : CellState.Empty;
                if (_levelModel.GetCellState(cellPosition) != newState)
                {
                    _levelModel.SetCellState(cellPosition, newState);
                }
            }
            
            HapticService.Instance.Play(HapticType.ThermometerInteraction);
        }
    }
}