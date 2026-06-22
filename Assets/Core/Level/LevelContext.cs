using Core.Level.Input;
using UnityEngine;

namespace Core.Level
{
    public class LevelContext: MonoBehaviour
    {
        [field: SerializeField] public LevelView LevelView { get; set; }
        [field: SerializeField] public InputController InputController { get; set; }
    }
}