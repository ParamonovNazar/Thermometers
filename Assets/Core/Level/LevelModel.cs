using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    public class LevelModel
    {
        private readonly LevelConfig _config;
        private readonly CellState[,] _cellStates;
        private readonly Dictionary<Vector2Int, ThermometerData> _cellToThermometer;
        
        public event System.Action<ThermometerData, int> OnThermometerFillChanged;
        public event System.Action OnLevelSolved;

        public int Width => _config.Width;
        public int Height => _config.Height;
        public int[] RowConstraints => _config.RowConstraints;
        public int[] ColumnConstraints => _config.ColumnConstraints;
        public List<ThermometerData> Thermometers => _config.Thermometers;

        public LevelModel(LevelConfig config)
        {
            _config = config;
            _cellStates = new CellState[config.Width, config.Height];
            _cellToThermometer = new Dictionary<Vector2Int, ThermometerData>();

            foreach (var thermometer in config.Thermometers)
            {
                foreach (var cell in thermometer.Cells)
                {
                    _cellToThermometer[cell] = thermometer;
                }
            }
        }

        public CellState GetCellState(Vector2Int coord)
        {
            if (IsOutOfBounds(coord)) return CellState.Empty;
            return _cellStates[coord.x, coord.y];
        }

        public bool CanToggleCell(Vector2Int coord)
        {
            if (IsOutOfBounds(coord)) return false;
            
            if (!_cellToThermometer.TryGetValue(coord, out var thermometer))
            {
                return true; // Should not happen in a valid level, but allow it for non-thermometer cells if any
            }

            int index = thermometer.Cells.IndexOf(coord);
            var currentState = _cellStates[coord.x, coord.y];

            if (currentState == CellState.Empty || currentState == CellState.CrossedOut)
            {
                // To fill: all previous cells must be filled
                for (int i = 0; i < index; i++)
                {
                    var prevCoord = thermometer.Cells[i];
                    if (_cellStates[prevCoord.x, prevCoord.y] != CellState.Filled) return false;
                }
            }
            else if (currentState == CellState.Filled)
            {
                // To empty: all subsequent cells must be empty/crossed out
                for (int i = index + 1; i < thermometer.Cells.Count; i++)
                {
                    var nextCoord = thermometer.Cells[i];
                    if (_cellStates[nextCoord.x, nextCoord.y] == CellState.Filled) return false;
                }
            }

            return true;
        }

        public void ToggleCell(Vector2Int coord)
        {
            if (!CanToggleCell(coord)) return;

            var currentState = _cellStates[coord.x, coord.y];
            _cellStates[coord.x, coord.y] = currentState switch
            {
                CellState.Empty => CellState.Filled,
                CellState.Filled => CellState.CrossedOut,
                CellState.CrossedOut => CellState.Empty,
                _ => CellState.Empty
            };
        }

        public bool IsSolved()
        {
            // Validate Rows
            for (int y = 0; y < Height; y++)
            {
                int filledCount = 0;
                for (int x = 0; x < Width; x++)
                {
                    if (_cellStates[x, y] == CellState.Filled) filledCount++;
                }
                if (filledCount != RowConstraints[y]) return false;
            }

            // Validate Columns
            for (int x = 0; x < Width; x++)
            {
                int filledCount = 0;
                for (int y = 0; y < Height; y++)
                {
                    if (_cellStates[x, y] == CellState.Filled) filledCount++;
                }
                if (filledCount != ColumnConstraints[x]) return false;
            }

            // Thermometer gaps are implicitly validated by CanToggleCell logic 
            // but for safety, we should ensure no gaps exist in the final state.
            foreach (var thermometer in _config.Thermometers)
            {
                bool foundEmpty = false;
                foreach (var cell in thermometer.Cells)
                {
                    bool isFilled = _cellStates[cell.x, cell.y] == CellState.Filled;
                    if (foundEmpty && isFilled) return false;
                    if (!isFilled) foundEmpty = true;
                }
            }

            return true;
        }

        public bool IsRowSatisfied(int y)
        {
            if (y < 0 || y >= Height) return false;
            int filledCount = 0;
            for (int x = 0; x < Width; x++)
            {
                if (_cellStates[x, y] == CellState.Filled) filledCount++;
            }
            return filledCount == RowConstraints[y];
        }

        public bool IsColumnSatisfied(int x)
        {
            if (x < 0 || x >= Width) return false;
            int filledCount = 0;
            for (int y = 0; y < Height; y++)
            {
                if (_cellStates[x, y] == CellState.Filled) filledCount++;
            }
            return filledCount == ColumnConstraints[x];
        }

        public bool TryGetThermometer(Vector2Int coord, out ThermometerData thermometer)
        {
            return _cellToThermometer.TryGetValue(coord, out thermometer);
        }

        public void SetThermometerFill(ThermometerData thermometer, int targetLength)
        {
            for (int i = 0; i < thermometer.Cells.Count; i++)
            {
                var cell = thermometer.Cells[i];
                var newState = i < targetLength ? CellState.Filled : CellState.Empty;
                _cellStates[cell.x, cell.y] = newState;
            }
            
            OnThermometerFillChanged?.Invoke(thermometer, targetLength);
            
            CheckWin();
        }

        private void CheckWin()
        {
            if (IsSolved())
            {
                OnLevelSolved?.Invoke();
            }
        }

        private bool IsOutOfBounds(Vector2Int coord)
        {
            return coord.x < 0 || coord.x >= Width || coord.y < 0 || coord.y >= Height;
        }
    }
}
