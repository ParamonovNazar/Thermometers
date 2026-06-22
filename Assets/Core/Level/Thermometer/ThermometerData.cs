using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [Serializable]
    public class ThermometerData
    {
        [SerializeField] private List<Vector2Int> _cells = new List<Vector2Int>();
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private int _solutionFill;

        public List<Vector2Int> Cells => _cells;
        public Color Color
        {
            get => _color;
            set => _color = value;
        }
        public int SolutionFill
        {
            get => _solutionFill;
            set => _solutionFill = value;
        }

        public ThermometerData(List<Vector2Int> cells, Color color)
        {
            _cells = cells;
            _color = color;
        }

        public ThermometerData()
        {
        }

        public int GetLengthToCell(Vector2Int cellPosition)
        {
            return Cells.IndexOf(cellPosition);
        }
    }
}