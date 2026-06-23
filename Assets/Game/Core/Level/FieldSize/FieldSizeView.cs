using TMPro;
using UnityEngine;

namespace Core.Level
{
    public class FieldSizeView: MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI WithLabel { get; set; }
        [field: SerializeField] public TextMeshProUGUI HeightLabel { get; set; }

        public void Setup(int width, int height)
        {
            WithLabel.text = width.ToString();
            HeightLabel.text = height.ToString();
        }
    }
}