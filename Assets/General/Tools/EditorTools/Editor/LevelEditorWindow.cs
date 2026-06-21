using System.Collections.Generic;
using System.Linq;
using Core.Level;
using UnityEditor;
using UnityEngine;

namespace Tools.EditorTools.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private string _levelId = "NewLevel";
        private int _width = 5;
        private int _height = 5;
        private List<ThermometerData> _thermometers = new List<ThermometerData>();
        private HashSet<Vector2Int> _solution = new HashSet<Vector2Int>();
        
        private int _selectedThermometerIndex = -1;
        private Color _nextColor = Color.white;
        
        private Vector2 _scrollPosition;
        private const float CELL_SIZE = 30f;

        [MenuItem("Tools/Levels/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            // Left Panel: Controls
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawControls();
            EditorGUILayout.EndVertical();

            // Right Panel: Grid
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawGrid();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawControls()
        {
            GUILayout.Label("Level Settings", EditorStyles.boldLabel);
            _levelId = EditorGUILayout.TextField("Level ID", _levelId);
            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);

            if (GUILayout.Button("Clear Grid"))
            {
                if (EditorUtility.DisplayDialog("Clear Grid", "Are you sure you want to clear all thermometers and solution?", "Yes", "No"))
                {
                    _thermometers.Clear();
                    _solution.Clear();
                    _selectedThermometerIndex = -1;
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Thermometers", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Add Thermometer"))
            {
                _thermometers.Add(new ThermometerData(new List<Vector2Int>(), _nextColor));
                _selectedThermometerIndex = _thermometers.Count - 1;
                _nextColor = new Color(Random.value, Random.value, Random.value, 1f);
            }

            for (int i = 0; i < _thermometers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(_selectedThermometerIndex == i, $"Thermometer {i}", "Button"))
                {
                    _selectedThermometerIndex = i;
                }
                _thermometers[i] = new ThermometerData(_thermometers[i].Cells, EditorGUILayout.ColorField(_thermometers[i].Color));
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    _thermometers.RemoveAt(i);
                    if (_selectedThermometerIndex >= i) _selectedThermometerIndex--;
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            GUILayout.Label("Actions", EditorStyles.boldLabel);
            if (GUILayout.Button("Save Level"))
            {
                SaveLevel();
            }
        }

        private void DrawGrid()
        {
            Rect gridRect = GUILayoutUtility.GetRect(_width * CELL_SIZE, _height * CELL_SIZE);
            
            // Draw background
            EditorGUI.DrawRect(gridRect, new Color(0.2f, 0.2f, 0.2f));

            // Draw cells
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Vector2Int coord = new Vector2Int(x, y);
                    Rect cellRect = new Rect(gridRect.x + x * CELL_SIZE, gridRect.y + (_height - 1 - y) * CELL_SIZE, CELL_SIZE, CELL_SIZE);
                    
                    DrawCell(cellRect, coord);
                }
            }

            // Handle input for the whole grid to allow dragging
            HandleGridInput(gridRect);
        }

        private void DrawCell(Rect rect, Vector2Int coord)
        {
            // Draw grid lines
            Handles.color = Color.gray;
            Handles.DrawPolyLine(
                new Vector3(rect.x, rect.y), 
                new Vector3(rect.xMax, rect.y), 
                new Vector3(rect.xMax, rect.yMax), 
                new Vector3(rect.x, rect.yMax), 
                new Vector3(rect.x, rect.y)
            );

            // Highlight if in a thermometer
            var thermo = _thermometers.FirstOrDefault(t => t.Cells.Contains(coord));
            if (thermo != null)
            {
                Color c = thermo.Color;
                c.a = 0.5f;
                EditorGUI.DrawRect(new Rect(rect.x + 2, rect.y + 2, rect.width - 4, rect.height - 4), c);
                
                // Draw index in thermometer
                int index = thermo.Cells.IndexOf(coord);
                GUI.Label(rect, index.ToString(), EditorStyles.miniLabel);
            }

            // Highlight if in solution
            if (_solution.Contains(coord))
            {
                EditorGUI.DrawRect(new Rect(rect.x + rect.width * 0.3f, rect.y + rect.height * 0.3f, rect.width * 0.4f, rect.height * 0.4f), Color.black);
            }
        }

        private void HandleGridInput(Rect gridRect)
        {
            Event e = Event.current;
            if (gridRect.Contains(e.mousePosition))
            {
                Vector2 localMouse = e.mousePosition - gridRect.position;
                int x = Mathf.FloorToInt(localMouse.x / CELL_SIZE);
                int y = _height - 1 - Mathf.FloorToInt(localMouse.y / CELL_SIZE);
                Vector2Int coord = new Vector2Int(x, y);

                if (x >= 0 && x < _width && y >= 0 && y < _height)
                {
                    if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
                    {
                        if (e.button == 0) // Left click: Thermometer
                        {
                            if (_selectedThermometerIndex >= 0 && _selectedThermometerIndex < _thermometers.Count)
                            {
                                var currentThermo = _thermometers[_selectedThermometerIndex];
                                if (!currentThermo.Cells.Contains(coord))
                                {
                                    // Auto-check for continuity (optional but helpful)
                                    if (currentThermo.Cells.Count > 0)
                                    {
                                        Vector2Int last = currentThermo.Cells.Last();
                                        if (Mathf.Abs(last.x - coord.x) + Mathf.Abs(last.y - coord.y) == 1)
                                        {
                                            currentThermo.Cells.Add(coord);
                                            Repaint();
                                        }
                                    }
                                    else
                                    {
                                        currentThermo.Cells.Add(coord);
                                        Repaint();
                                    }
                                }
                            }
                        }
                        else if (e.button == 1) // Right click: Solution toggle
                        {
                            if (e.type == EventType.MouseDown)
                            {
                                if (_solution.Contains(coord)) _solution.Remove(coord);
                                else _solution.Add(coord);
                                Repaint();
                            }
                        }
                        else if (e.button == 2) // Middle click: Remove from thermometer
                        {
                            foreach (var t in _thermometers)
                            {
                                if (t.Cells.Remove(coord))
                                {
                                    Repaint();
                                    break;
                                }
                            }
                        }
                        e.Use();
                    }
                }
            }
        }

        private void SaveLevel()
        {
            int[] rowConstraints = new int[_height];
            int[] colConstraints = new int[_width];

            for (int y = 0; y < _height; y++)
            {
                int count = 0;
                for (int x = 0; x < _width; x++)
                {
                    if (_solution.Contains(new Vector2Int(x, y))) count++;
                }
                rowConstraints[y] = count;
            }

            for (int x = 0; x < _width; x++)
            {
                int count = 0;
                for (int y = 0; y < _height; y++)
                {
                    if (_solution.Contains(new Vector2Int(x, y))) count++;
                }
                colConstraints[x] = count;
            }

            var levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
            SerializedObject so = new SerializedObject(levelConfig);
            so.FindProperty("<Id>k__BackingField").stringValue = _levelId;
            so.FindProperty("<Width>k__BackingField").intValue = _width;
            so.FindProperty("<Height>k__BackingField").intValue = _height;

            var rowsProp = so.FindProperty("<RowConstraints>k__BackingField");
            rowsProp.arraySize = _height;
            for (int i = 0; i < _height; i++) rowsProp.GetArrayElementAtIndex(i).intValue = rowConstraints[i];

            var colsProp = so.FindProperty("<ColumnConstraints>k__BackingField");
            colsProp.arraySize = _width;
            for (int i = 0; i < _width; i++) colsProp.GetArrayElementAtIndex(i).intValue = colConstraints[i];

            so.ApplyModifiedProperties();

            typeof(LevelConfig).GetProperty("Thermometers").SetValue(levelConfig, _thermometers);

            // Add to LevelStorage
            var guids = AssetDatabase.FindAssets("t:LevelStorage");
            if (guids.Length > 0)
            {
                var storagePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var storage = AssetDatabase.LoadAssetAtPath<LevelStorage>(storagePath);
                if (storage != null)
                {
                    if (storage.Levels == null) storage.Levels = new List<LevelConfig>();
                    if (!storage.Levels.Contains(levelConfig))
                    {
                        storage.Levels.Add(levelConfig);
                        EditorUtility.SetDirty(storage);
                    }
                }
            }

            string path = $"Assets/Core/Level/{_levelId}.asset";
            AssetDatabase.CreateAsset(levelConfig, path);
            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelConfig;
            
            Debug.Log($"Level saved to {path}");
        }
    }
}
