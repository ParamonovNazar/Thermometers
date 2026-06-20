using System.Collections.Generic;
using Core.Level.Thermometer;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private Transform _rowConstraintsParent;
        [SerializeField] private Transform _columnConstraintsParent;
        [SerializeField] private ThermometerView _thermometerViewPrefab;
        [SerializeField] private Transform _thermometerViewRoot;
        
        private LevelModel _model;
        private readonly Dictionary<ThermometerData, ThermometerView> _thermometerViews = new();
        
        public CellView[,] Cells { get; private set; }
        public ConstraintView[] RowConstraints { get; private set; }
        public ConstraintView[] ColumnConstraints { get; private set; }
        public List<ThermometerView> Thermometers { get; private set; } = new();

        public void Initialize(LevelModel model)
        {
            _model = model;
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = model.Width;

            Cells = new CellView[model.Width, model.Height];

            // Clear existing
            foreach (Transform child in _gridLayout.transform) Destroy(child.gameObject);
            foreach (Transform child in _rowConstraintsParent) Destroy(child.gameObject);
            foreach (Transform child in _columnConstraintsParent) Destroy(child.gameObject);
            foreach (Transform child in _thermometerViewRoot) Destroy(child.gameObject);
            Thermometers.Clear();
            _thermometerViews.Clear();

            _model.OnThermometerFillChanged += HandleThermometerFillChanged;

            // Create cells (Top-to-bottom for GridLayout default usually, 
            // but we need to match our (0,0) bottom-left coordinate system)
            // GridLayout usually fills row by row from top-left.
            for (int y = model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < model.Width; x++)
                {
                    var cellView = Instantiate(_cellPrefab, _gridLayout.transform);
                    Cells[x, y] = cellView;
                }
            }

            // Constraints
            RowConstraints = new ConstraintView[model.Height];
            for (int y = model.Height - 1; y >= 0; y--)
            {
                var constraintView = Instantiate(_rowConstraintPrefab, _rowConstraintsParent);
                constraintView.Setup(0, model.RowConstraints[y]);
                RowConstraints[y] = constraintView;
            }

            ColumnConstraints = new ConstraintView[model.Width];
            for (int x = 0; x < model.Width; x++)
            {
                var constraintView = Instantiate(_columnConstraintPrefab, _columnConstraintsParent);
                constraintView.Setup(0, model.ColumnConstraints[x]);
                ColumnConstraints[x] = constraintView;
            }

            // Thermometers
            float cellSize = _gridLayout.cellSize.x + _gridLayout.spacing.x;
            foreach (var thermometerData in model.Thermometers)
            {
                var thermometerView = Instantiate(_thermometerViewPrefab, _thermometerViewRoot);
                thermometerView.Initialize(thermometerData, cellSize);
                Thermometers.Add(thermometerView);
                _thermometerViews[thermometerData] = thermometerView;
            }
        }

        private void OnDestroy()
        {
            if (_model != null)
            {
                _model.OnThermometerFillChanged -= HandleThermometerFillChanged;
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