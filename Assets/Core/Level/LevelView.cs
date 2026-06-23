using System.Collections.Generic;
using Core.Level.Thermometer;
using Cysharp.Threading.Tasks;
using Infrastructure.Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private ConstraintView _rowConstraintPrefab;
        [SerializeField] private ConstraintView _columnConstraintPrefab;
        [SerializeField] private FieldSizeView _fieldSizeViewPrefab;
        [SerializeField] private ThermometerView _thermometerViewPrefab;
        [SerializeField] private Transform _thermometerViewRoot;
        [SerializeField] private RectTransform _inputOverlay;
        [SerializeField] private RectTransform _playableArea;
        [SerializeField] private float _spacingRatio = 0.125f;

        private GameConfig _gameConfig;
        private LevelModel _model;
        private readonly Dictionary<ThermometerData, ThermometerView> _thermometerViews = new();
        
        public CellView[,] Cells { get; private set; }
        public ConstraintView[] RowConstraints { get; private set; }
        public ConstraintView[] ColumnConstraints { get; private set; }
        public List<ThermometerView> Thermometers { get; private set; } = new();
        
        public ThermometerView GetThermometerView(ThermometerData data)
        {
            return _thermometerViews.TryGetValue(data, out var view) ? view : null;
        }

        public void Initialize(LevelModel model, GameConfig gameConfig)
        {
            _model = model;
            _gameConfig = gameConfig;
            
            // Clear existing
            foreach (Transform child in _gridLayout.transform) Destroy(child.gameObject);
            foreach (Transform child in _thermometerViewRoot) Destroy(child.gameObject);
            Thermometers.Clear();
            _thermometerViews.Clear();

            _model.OnThermometerFillChanged += HandleThermometerFillChanged;
            _model.OnThermometerCrossChanged += HandleThermometerCrossChanged;

            // Calculate layout
            Rect rect = _playableArea.rect;
            int gridWidthCount = model.Width + 1;
            int gridHeightCount = model.Height + 1;

            float spacingX = gridWidthCount > 1 ? (rect.width * _spacingRatio) / (gridWidthCount - 1) : 0;
            float spacingY = gridHeightCount > 1 ? (rect.height * _spacingRatio) / (gridHeightCount - 1) : 0;
            float spacing = Mathf.Min(spacingX, spacingY);
            _gridLayout.spacing = new Vector2(spacing, spacing);

            float totalSpacingX = gridWidthCount > 1 ? spacing * (gridWidthCount - 1) : 0;
            float totalSpacingY = gridHeightCount > 1 ? spacing * (gridHeightCount - 1) : 0;

            float cellWidth = (rect.width - totalSpacingX) / gridWidthCount;
            float cellHeight = (rect.height - totalSpacingY) / gridHeightCount;
            float cellSize = Mathf.Min(cellWidth, cellHeight);

            _gridLayout.cellSize = new Vector2(cellSize, cellSize);
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = gridWidthCount;

            float gridWidth = cellSize * gridWidthCount + _gridLayout.spacing.x * (gridWidthCount - 1);
            float gridHeight = cellSize * gridHeightCount + _gridLayout.spacing.y * (gridHeightCount - 1);

            var gridRectTransform = _gridLayout.GetComponent<RectTransform>();
            gridRectTransform.sizeDelta = new Vector2(gridWidth, gridHeight);

            Cells = new CellView[model.Width, model.Height];
            RowConstraints = new ConstraintView[model.Height];
            ColumnConstraints = new ConstraintView[model.Width];

            // Create grid elements
            // Top row: grid info, then Column Constraints
            var fieldSizeView = Instantiate(_fieldSizeViewPrefab, _gridLayout.transform);
            fieldSizeView.Setup(model.Width, model.Height);
            
            for (int x = 0; x < model.Width; x++)
            {
                var constraintView = Instantiate(_columnConstraintPrefab, _gridLayout.transform);
                constraintView.Setup(0, model.ColumnConstraints[x]);
                ColumnConstraints[x] = constraintView;
            }

            // Subsequent rows: Row Constraint, then Level Cells
            for (int y = model.Height - 1; y >= 0; y--)
            {
                // Row constraint at the start of the row
                var rowConstraintView = Instantiate(_rowConstraintPrefab, _gridLayout.transform);
                rowConstraintView.Setup(0, model.RowConstraints[y]);
                RowConstraints[y] = rowConstraintView;

                for (int x = 0; x < model.Width; x++)
                {
                    var cellView = Instantiate(_cellPrefab, _gridLayout.transform);
                    Cells[x, y] = cellView;
                }
            }

            // Input overlay alignment
            if (_inputOverlay != null)
            {
                _inputOverlay.sizeDelta = gridRectTransform.sizeDelta;
                _inputOverlay.anchorMin = gridRectTransform.anchorMin;
                _inputOverlay.anchorMax = gridRectTransform.anchorMax;
                _inputOverlay.pivot = gridRectTransform.pivot;
                _inputOverlay.anchoredPosition = gridRectTransform.anchoredPosition;
            }

            // Thermometers
            LayoutRebuilder.ForceRebuildLayoutImmediate(gridRectTransform);
            _thermometerViewRoot.position = Cells[0, 0].transform.position;
            
            float stepSize = cellSize + _gridLayout.spacing.x;
            foreach (var thermometerData in model.Thermometers)
            {
                var thermometerView = Instantiate(_thermometerViewPrefab, _thermometerViewRoot);
                thermometerView.Initialize(thermometerData, stepSize, _gameConfig);
                thermometerView.UpdateCrosses(model);
                
                Thermometers.Add(thermometerView);
                _thermometerViews[thermometerData] = thermometerView;
            }
        }

        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.OnThermometerFillChanged -= HandleThermometerFillChanged;
                _model.OnThermometerCrossChanged -= HandleThermometerCrossChanged;
            }
        }

        private void HandleThermometerFillChanged(ThermometerData thermometer, int length)
        {
            if (_thermometerViews.TryGetValue(thermometer, out var view))
            {
                view.Fill(length).Forget(Debug.LogException);
            }

            UpdateConstraints();
        }

        private void HandleThermometerCrossChanged(ThermometerData thermometer, int index)
        {
            if (_thermometerViews.TryGetValue(thermometer, out var view))
            {
                view.UpdateCrosses(_model);
            }
        }

        private void UpdateConstraints()
        {
            for (int y = 0; y < _model.Height; y++)
            {
                int filledCount = 0;
                for (int x = 0; x < _model.Width; x++)
                {
                    if (_model.GetCellState(new Vector2Int(x, y)) == CellState.Filled) filledCount++;
                }
                RowConstraints[y].SetCurrentValue(filledCount);
            }

            for (int x = 0; x < _model.Width; x++)
            {
                int filledCount = 0;
                for (int y = 0; y < _model.Height; y++)
                {
                    if (_model.GetCellState(new Vector2Int(x, y)) == CellState.Filled) filledCount++;
                }
                ColumnConstraints[x].SetCurrentValue(filledCount);
            }
        }
    }
}