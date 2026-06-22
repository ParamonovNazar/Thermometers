using System;
using UnityEngine;
using UnityEngine.UI;

namespace General.Switch
{
    public class DoubleSwitch : MonoBehaviour
    {
        private static readonly int IsFirstOptionKey = Animator.StringToHash("IsFirstOption");

        [SerializeField] private Animator _animator;
        [SerializeField] private Button _button;

        public event Action<bool> OnOptionChanged;
        public bool IsFirstOption { get; private set; } = true;

        private void Awake()
        {
            _button.onClick.AddListener(HandleClick);
            
            //in case if option changed before Awake
            InitializeFirstOption(IsFirstOption);
        }

        public void InitializeFirstOption(bool isFirstOption)
        {
            IsFirstOption = isFirstOption;
            _animator.Play(IsFirstOption ? "FirstOption" : "SecondOption");
        }

        public void SetOption(bool isFirstOption, bool raiseEvent = true)
        {
            IsFirstOption = isFirstOption;
            _animator.SetBool(IsFirstOptionKey, IsFirstOption);
            if (raiseEvent)
                OnOptionChanged?.Invoke(IsFirstOption);
        }

        private void HandleClick()
        {
            SetOption(!IsFirstOption);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }
}