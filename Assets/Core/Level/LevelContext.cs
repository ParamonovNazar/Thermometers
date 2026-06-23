using Core.Level.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level
{
    public class LevelContext: MonoBehaviour
    {
        [field: SerializeField] public LevelView LevelView { get; set; }
        [field: SerializeField] public InputController InputController { get; set; }
        [field: SerializeField] public RectTransform LayoutRect { get; set; }
        public void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(LayoutRect);
        }
    }
}