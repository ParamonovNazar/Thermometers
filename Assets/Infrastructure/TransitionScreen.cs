using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Infrastructure
{
    public class TransitionScreen : MonoBehaviour
    {
        private static readonly int IsShowing = Animator.StringToHash("IsShowing");

        [SerializeField] private float _showTime;
        [SerializeField] private float _hideTime;
        [SerializeField] private float _disableTime;

        public bool IsActive { get; set; }

        public static TransitionScreen Instance { get; private set; }
        
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            gameObject.SetActive(false);
            Instance = this;
        }

        public async UniTask Show()
        {
            gameObject.SetActive(true);
            IsActive = true;
            await UniTask.Delay(TimeSpan.FromSeconds(_showTime));
        }

        public async UniTask Hide()
        {
            IsActive = false;
            await UniTask.Delay(TimeSpan.FromSeconds(_hideTime));
            gameObject.SetActive(false);
        }
    }
}