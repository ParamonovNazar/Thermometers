using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Level.Thermometer
{
    public class ThermometerView : MonoBehaviour
    {
        [SerializeField] private ThermometerPartBase _blobPrefab;
        [SerializeField] private ThermometerPartBase _straightPrefab;
        [SerializeField] private ThermometerPartBase _turnRightPrefab;
        [SerializeField] private ThermometerPartBase _turnLeftPrefab;
        [SerializeField] private ThermometerPartBase _endPrefab;

        [SerializeField] private float _partFillTime = 0.1f;
        [SerializeField] private AnimationCurve _scaleCurve;
        [SerializeField] private float _scaleTime = 0.2f;
        [SerializeField] private float _scaleModifier = 1.1f;
        [SerializeField] private Color _defaultBorderColor = Color.white;

        private List<ThermometerPartBase> _parts = new();
        private List<Vector2Int> _cellCoords = new();

        public int Length { get; private set; }
        public int CurrentFillIndex { get; private set; }
        public bool IsBlocked { get; set; } = false;

        private CancellationTokenSource _fillCancellationTokenSource;
        private CancellationTokenSource _scaleCancellationTokenSource;
        private CancellationTokenSource _highlightCancellationTokenSource;

        public void Initialize(ThermometerData data, float cellSize)
        {
            _cellCoords = new List<Vector2Int>(data.Cells);
            Length = data.Cells.Count;
            CurrentFillIndex = 0;

            foreach (var part in _parts)
            {
                Destroy(part.gameObject);
            }

            _parts.Clear();

            for (int i = 0; i < Length; i++)
            {
                ThermometerPartBase prefab = GetPrefabForIndex(i, data.Cells);
                ThermometerPartBase part = Instantiate(prefab, transform);

                part.transform.localPosition = new Vector3(data.Cells[i].x * cellSize, data.Cells[i].y * cellSize, 0);

                if (i == 0)
                {
                    if (Length > 1)
                    {
                        Vector2Int direction = data.Cells[i + 1] - data.Cells[i];
                        part.transform.localRotation = GetRotationForDirection(direction);
                    }
                }
                else if (i == Length - 1)
                {
                    Vector2Int direction = data.Cells[i] - data.Cells[i - 1];
                    part.transform.localRotation = GetRotationForDirection(direction);
                }
                else
                {
                    Vector2Int prevDir = data.Cells[i] - data.Cells[i - 1];
                    Vector2Int nextDir = data.Cells[i + 1] - data.Cells[i];

                    if (prevDir == nextDir)
                    {
                        part.transform.localRotation = GetRotationForDirection(nextDir);
                    }
                    else
                    {
                        part.transform.localRotation = GetRotationForDirection(prevDir);
                    }
                }

                part.SetSize(cellSize);
                part.Setup(data.Color);
                part.SetBorderColor(_defaultBorderColor);
                part.SetFill(0);
                part.SetScale(1f);
                _parts.Add(part);
            }
        }

        public async UniTask Highlight(Color highlightColor, float duration)
        {
            _highlightCancellationTokenSource?.Cancel();
            _highlightCancellationTokenSource?.Dispose();
            _highlightCancellationTokenSource = new CancellationTokenSource();
            var token = _highlightCancellationTokenSource.Token;

            try
            {
                float elapsed = 0f;

                // To highlight color
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / duration);
                    Color currentColor = Color.Lerp(_defaultBorderColor, highlightColor, progress);
                    SetBordersColor(currentColor);
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                SetBordersColor(highlightColor);
            }
            catch (System.OperationCanceledException)
            {
                // Highlight cancelled
            }
        }

        private void SetBordersColor(Color color)
        {
            foreach (var part in _parts)
            {
                part.SetBorderColor(color);
            }
        }

        private ThermometerPartBase GetPrefabForIndex(int index, List<Vector2Int> cells)
        {
            if (index == 0) return _blobPrefab;
            if (index == Length - 1) return _endPrefab;

            Vector2Int prevDir = cells[index] - cells[index - 1];
            Vector2Int nextDir = cells[index + 1] - cells[index];

            if (prevDir == nextDir)
            {
                return _straightPrefab;
            }

            // Determine turn direction
            // In 2D (XY plane), cross product of (x1, y1) and (x2, y2) is x1*y2 - y1*x2
            int crossProduct = prevDir.x * nextDir.y - prevDir.y * nextDir.x;

            return crossProduct < 0 ? _turnRightPrefab : _turnLeftPrefab;
        }

        private Quaternion GetRotationForDirection(Vector2Int direction)
        {
            if (direction == Vector2Int.up) return Quaternion.identity;
            if (direction == Vector2Int.down) return Quaternion.Euler(0, 0, 180);
            if (direction == Vector2Int.left) return Quaternion.Euler(0, 0, 90);
            if (direction == Vector2Int.right) return Quaternion.Euler(0, 0, -90);
            return Quaternion.identity;
        }

        public async UniTask Fill(int length)
        {
            ScaleAnimate().Forget();

            _fillCancellationTokenSource?.Cancel();
            _fillCancellationTokenSource?.Dispose();
            _fillCancellationTokenSource = new CancellationTokenSource();
            var token = _fillCancellationTokenSource.Token;

            try
            {
                if (length > CurrentFillIndex)
                {
                    for (int i = CurrentFillIndex; i < length; i++)
                    {
                        await FillPart(i, 1f, token);
                        if (_fillCancellationTokenSource.IsCancellationRequested)
                        {
                            Debug.Log("Fill cancelled");
                        }

                        if (CurrentFillIndex == _parts.Count - 1)
                        {
                            break;
                        }

                        CurrentFillIndex++;
                    }
                }
                else
                {
                    for (int i = CurrentFillIndex; i >= length; i--)
                    {
                        CurrentFillIndex = i;
                        await FillPart(i, 0f, token);
                        if (_fillCancellationTokenSource.IsCancellationRequested)
                        {
                            Debug.Log("deFill cancelled");
                        }
                    }
                }
            }
            catch (System.OperationCanceledException)
            {
                // Fill interrupted
            }
        }

        private async UniTask FillPart(int index, float targetFill, CancellationToken token)
        {
            var part = _parts[index];
            float startFill = part.CurrentFillProgress;

            if (Mathf.Approximately(startFill, targetFill))
            {
                return;
            }

            float elapsed = targetFill > startFill ? startFill * _partFillTime : (1f - startFill) * _partFillTime;
            while (elapsed < _partFillTime)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _partFillTime);
                float current = Mathf.Lerp(startFill, targetFill, progress);
                part.SetFill(current);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            if (token.IsCancellationRequested)
            {
                Debug.Log("FillPart cancelled");
            }

            part.SetFill(targetFill);
        }

        private async UniTaskVoid ScaleAnimate()
        {
            _scaleCancellationTokenSource?.Cancel();
            _scaleCancellationTokenSource?.Dispose();
            _scaleCancellationTokenSource = new CancellationTokenSource();
            var token = _scaleCancellationTokenSource.Token;

            try
            {
                float elapsed = 0;
                while (elapsed < _scaleTime)
                {
                    elapsed += Time.deltaTime;
                    float progress = Mathf.Clamp01(elapsed / _scaleTime);
                    float scaleValue = 1f + (_scaleCurve.Evaluate(progress) * _scaleModifier);

                    foreach (var part in _parts)
                    {
                        part.SetScale(scaleValue);
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                foreach (var part in _parts)
                {
                    part.SetScale(1f);
                }
            }
            catch (System.OperationCanceledException)
            {
            }
        }
    }
}