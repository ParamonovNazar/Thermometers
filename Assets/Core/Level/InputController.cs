using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.Level
{
    public class InputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
    {
        [SerializeField] private LevelView _levelView;
        [SerializeField] private RectTransform _rectTransform;

        private LevelModel _levelModel;

        [field: SerializeField] public FillType FillType { get; set; } = FillType.Standard;
        [field: SerializeField] public DrawType DrawType { get; set; } = DrawType.Fill;

        public void Initialize(LevelModel levelModel)
        {
            _levelModel = levelModel;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (TryFindCell(eventData, out var cellPosition))
            {
                if (_levelModel.TryGetThermometer(cellPosition, out var thermometer))
                {
                    bool isFilled = _levelModel.GetCellState(cellPosition) == CellState.Filled;
                    DrawType = isFilled ? DrawType.Remove : DrawType.Fill;

                    var drawModifier = DrawType == DrawType.Remove ? 0 : 1;

                    var length = Mathf.Max(0, thermometer.GetLengthToCell(cellPosition) + drawModifier);

                    _levelModel.SetThermometerFill(thermometer, length);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            // if (eventData.dragging)
            // {
            //     if (TryFindCell(eventData, out var cellPosition))
            //     {
            //         if (_levelModel.TryGetThermometer(cellPosition, out var thermometer))
            //         {
            //             var length = thermometer.GetLengthToCell(cellPosition);
            //
            //             _levelModel.SetThermometerFill(thermometer, length);
            //         }
            //     }
            // }
        }

        private bool TryFindCell(PointerEventData eventData, out Vector2Int cellPosition)
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
    }

    public enum FillType
    {
        Standard = 0,
        Cross = 1
    }

    public enum DrawType
    {
        Fill = 0,
        Remove = 1
    }
}