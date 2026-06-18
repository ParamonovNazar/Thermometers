using System.Collections.Generic;
using UnityEngine;

namespace Core.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Config/LevelConfig")]
    public class LevelConfig: ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public int Width { get; private set; }
        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public int[] RowConstraints { get; private set; }
        [field: SerializeField] public int[] ColumnConstraints { get; private set; }
        [field: SerializeField] public List<ThermometerData> Thermometers { get; private set; }
    }
}