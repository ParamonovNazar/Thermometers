using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure.TransitionScreen
{
    public class TransitionView : MonoBehaviour
    {
        private static readonly int IsShowing = Animator.StringToHash("IsShowing");

        [SerializeField] private float _showTime;
        [SerializeField] private float _hideTime;
        [SerializeField] private Animator _animator;
        [SerializeField] private Canvas _canvas;
        
        public bool IsActive { get; set; }

        public static TransitionView Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(this.gameObject);
            gameObject.SetActive(false);
            Instance = this;
        }

        public async UniTask Show()
        {
            gameObject.SetActive(true);
            _canvas.enabled = true;
            IsActive = true;
            _animator.SetBool(IsShowing, true);
            await UniTask.Delay(TimeSpan.FromSeconds(_showTime));
        }

        public async UniTask Hide()
        {
            IsActive = false;
            _animator.SetBool(IsShowing, false);
            await UniTask.Delay(TimeSpan.FromSeconds(_hideTime));
            gameObject.SetActive(false);
            _canvas.enabled = false;
        }
    }
}