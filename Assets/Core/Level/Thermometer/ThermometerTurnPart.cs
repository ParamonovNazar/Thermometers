using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Level.Thermometer
{
    public class ThermometerTurnPart : ThermometerPartBase
    {
        [SerializeField] private RectTransform _rootRectTransform;
        [SerializeField] private Transform _scaleRoot;
        [SerializeField] private float _straightRatio = 0.15f;
        [SerializeField] private RectTransform _firstStraightRect;
        [SerializeField] private RectTransform _secondStraightRect;
        [SerializeField] private RectTransform _turnRect;

        [SerializeField] private List<Segment> _segments;

        private float? _segmentsSum;

        public override void Setup(Color color)
        {
            foreach (var s in _segments)
                s.FillImage.color = color;
        }

        public override void SetFill(float progress)
        {
            _segmentsSum ??= _segments.Sum(s => s.FillPart);
            CurrentFillProgress = progress;

            if (_segmentsSum == 0) return;

            float currentTargetFill = progress * _segmentsSum.Value;
            float accumulatedFill = 0f;

            foreach (var segment in _segments)
            {
                float segmentStart = accumulatedFill;
                float segmentEnd = accumulatedFill + segment.FillPart;

                float segmentFill = Mathf.Clamp(currentTargetFill - segmentStart, 0f, segment.FillPart);
                segment.FillImage.fillAmount = segmentFill / segment.FillPart;

                accumulatedFill = segmentEnd;
            }
        }

        public override void SetSize(float cellSize)
        {
            _rootRectTransform.sizeDelta = new Vector2(cellSize, cellSize);
        }

        public override void SetScale(float scale)
        {
            _turnRect.localScale = scale * Vector3.one;
            
            var centerFill = (1f - _straightRatio * 2) * scale;
            var straightScale = (1f - centerFill) / (_straightRatio * 2);
            
            _firstStraightRect.localScale = new Vector3(scale, straightScale, 1f);
            _secondStraightRect.localScale = new Vector3(straightScale, scale, 1f);
        }

        public float _testProgress = 0.5f;

        [ContextMenu("Test")]
        public void TestProgress()
        {
            _segmentsSum = _segments.Sum(s => s.FillPart);
            SetFill(_testProgress);
        }

        public float _testScale = 0.5f;

        [ContextMenu("TestScale")]
        public void TestScale()
        {
            _segmentsSum = _segments.Sum(s => s.FillPart);
            SetScale(_testScale);
        }

        [Serializable]
        public class Segment
        {
            [SerializeField] public Image FillImage;
            [SerializeField] public float FillPart;
        }
    }
}