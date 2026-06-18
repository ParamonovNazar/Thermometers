using UnityEngine;

namespace Core.Level
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Config/LevelConfig")]
    public class LevelConfig: ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
    }
}