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

        public List<Vector2Int> Cells => _cells;
        public Color Color => _color;

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