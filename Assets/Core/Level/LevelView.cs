using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level
{
    public class LevelView : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup _gridLayout;
        [SerializeField] private CellView _cellPrefab;
        [SerializeField] private GameObject _rowConstraintPrefab;
        [SerializeField] private GameObject _columnConstraintPrefab;
        [SerializeField] private Transform _rowConstraintsParent;
        [SerializeField] private Transform _columnConstraintsParent;

        private LevelModel _model;
        private CellView[,] _cells;
        private TextMeshProUGUI[] _rowTexts;
        private TextMeshProUGUI[] _columnTexts;

        public void Initialize(LevelModel model, System.Action<Vector2Int> onCellClick)
        {
            _model = model;
            _gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _gridLayout.constraintCount = model.Width;

            _cells = new CellView[model.Width, model.Height];
            
            // Clear existing
            foreach (Transform child in _gridLayout.transform) Destroy(child.gameObject);
            foreach (Transform child in _rowConstraintsParent) Destroy(child.gameObject);
            foreach (Transform child in _columnConstraintsParent) Destroy(child.gameObject);

            // Create cells (Top-to-bottom for GridLayout default usually, 
            // but we need to match our (0,0) bottom-left coordinate system)
            // GridLayout usually fills row by row from top-left.
            for (int y = model.Height - 1; y >= 0; y--)
            {
                for (int x = 0; x < model.Width; x++)
                {
                    var cellView = Instantiate(_cellPrefab, _gridLayout.transform);
                    Vector2Int coord = new Vector2Int(x, y);
                    cellView.Button.onClick.AddListener(() => onCellClick?.Invoke(coord));
                    _cells[x, y] = cellView;
                }
            }

            // Constraints
            _rowTexts = new TextMeshProUGUI[model.Height];
            for (int y = model.Height - 1; y >= 0; y--)
            {
                var obj = Instantiate(_rowConstraintPrefab, _rowConstraintsParent);
                var text = obj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = model.RowConstraints[y].ToString();
                _rowTexts[y] = text;
            }

            _columnTexts = new TextMeshProUGUI[model.Width];
            for (int x = 0; x < model.Width; x++)
            {
                var obj = Instantiate(_columnConstraintPrefab, _columnConstraintsParent);
                var text = obj.GetComponentInChildren<TextMeshProUGUI>();
                text.text = model.ColumnConstraints[x].ToString();
                _columnTexts[x] = text;
            }

            UpdateAll();
        }

        public void UpdateCell(Vector2Int coord)
        {
            _cells[coord.x, coord.y].SetState(_model.GetCellState(coord));
            UpdateConstraints();
        }

        public void UpdateAll()
        {
            for (int x = 0; x < _model.Width; x++)
            {
                for (int y = 0; y < _model.Height; y++)
                {
                    _cells[x, y].SetState(_model.GetCellState(coord: new Vector2Int(x, y)));
                }
            }
            UpdateConstraints();
        }

        private void UpdateConstraints()
        {
            for (int y = 0; y < _model.Height; y++)
            {
                _rowTexts[y].color = _model.IsRowSatisfied(y) ? Color.green : Color.white;
            }
            for (int x = 0; x < _model.Width; x++)
            {
                _columnTexts[x].color = _model.IsColumnSatisfied(x) ? Color.green : Color.white;
            }
        }
    }
}
