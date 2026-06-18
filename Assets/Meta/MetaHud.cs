using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta
{
    public class MetaHud: MonoBehaviour
    {
        [field: SerializeField] public Button PlayButton { get; private set; }
        
        public event Action OnPlayClicked;

        private void Awake()
        {
            PlayButton.onClick.AddListener(HandlePlayClick);
        }

        private void HandlePlayClick()
        {
            OnPlayClicked?.Invoke();
        }

        private void OnDestroy()
        {
            PlayButton.onClick.RemoveListener(HandlePlayClick);
        }
    }
}