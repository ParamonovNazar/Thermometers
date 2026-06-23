using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Config/LevelConfig")]
    public class LevelConfig: ScriptableObject
    {
        [field: SerializeField] public string Id { get; set; }
        [field: SerializeField] public int Width { get; set; }
        [field: SerializeField] public int Height { get; set; }
        [field: SerializeField] public int[] RowConstraints { get; set; }
        [field: SerializeField] public int[] ColumnConstraints { get; set; }
        [field: SerializeField] public List<ThermometerData> Thermometers { get; set; }
    }
}