using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.Level.Thermometer
{
    public class ThermometerView : MonoBehaviour
    {
        [SerializeField] private ThermometerPartBase _blobPrefab;
        [SerializeField] private ThermometerPartBase _bodyPrefab;
        [SerializeField] private ThermometerPartBase _endPrefab;

        [SerializeField] private float _partFillTime = 0.1f;

        private List<ThermometerPartBase> _parts = new();
        private List<Vector2Int> _cellCoords = new();

        public int Length { get; private set; }
        public int CurrentFillIndex { get; private set; }
        
        private CancellationTokenSource _fillCancellationTokenSource;

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
                ThermometerPartBase prefab = GetPrefabForIndex(i);
                ThermometerPartBase part = Instantiate(prefab, transform);

                part.transform.localPosition = new Vector3(data.Cells[i].x * cellSize, data.Cells[i].y * cellSize, 0);

                if (i < Length - 1)
                {
                    Vector2Int direction = data.Cells[i + 1] - data.Cells[i];
                    part.transform.localRotation = GetRotationForDirection(direction);
                }
                else if (i > 0)
                {
                    // Last part follows previous direction
                    Vector2Int direction = data.Cells[i] - data.Cells[i - 1];
                    part.transform.localRotation = GetRotationForDirection(direction);
                }

                part.SetSize(cellSize);
                part.SetFill(0);
                _parts.Add(part);
            }
        }

        private ThermometerPartBase GetPrefabForIndex(int index)
        {
            if (index == 0) return _blobPrefab;
            if (index == Length - 1) return _endPrefab;
            return _bodyPrefab;
        }

        private Quaternion GetRotationForDirection(Vector2Int direction)
        {
            if (direction == Vector2Int.up) return Quaternion.Euler(0, 0, 0);
            if (direction == Vector2Int.down) return Quaternion.Euler(0, 0, 180);
            if (direction == Vector2Int.left) return Quaternion.Euler(0, 0, 90);
            if (direction == Vector2Int.right) return Quaternion.Euler(0, 0, -90);
            return Quaternion.identity;
        }

        public async UniTask Fill(int length)
        {
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
                        await FillPart(i, 0f, token);
                        CurrentFillIndex = i;
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

            float elapsed = 0;
            while (elapsed < _partFillTime)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / _partFillTime);
                float current = Mathf.Lerp(startFill, targetFill, progress);
                part.SetFill(current);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            part.SetFill(targetFill);
        }
    }
}