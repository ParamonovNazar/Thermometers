using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [Serializable]
    public class ThermometerData
    {
        [SerializeField] private List<Vector2Int> _cells = new List<Vector2Int>();
        
        public List<Vector2Int> Cells => _cells;

        public ThermometerData(List<Vector2Int> cells)
        {
            _cells = cells;
        }

        public ThermometerData() { }
    }
}
