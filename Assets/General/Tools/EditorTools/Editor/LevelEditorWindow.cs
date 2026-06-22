using System.Collections.Generic;
using System.Linq;
using Core.Level;
using Infrastructure.Configs;
using UnityEditor;
using UnityEngine;

namespace Tools.EditorTools.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private string _levelId = "Level";
        private int _width = 5;
        private int _height = 5;
        private List<ThermometerData> _thermometers = new List<ThermometerData>();
        private HashSet<Vector2Int> _solution = new HashSet<Vector2Int>();
        
        private int _selectedThermometerIndex = -1;
        private int _selectedColorId = -1;
        
        private Vector2 _scrollPosition;
        private const float CELL_SIZE = 30f;
        private GameConfig _gameConfig;
        private LevelConfig _configToLoad;
        private LevelConfig _activeConfig;

        [MenuItem("Tools/Levels/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnEnable()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            var guids = AssetDatabase.FindAssets("t:GameConfig");
            if (guids.Length > 0)
            {
                var storagePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                _gameConfig= AssetDatabase.LoadAssetAtPath<GameConfig>(storagePath);
            }
            else
            {
                Debug.LogError("GameConfig not found");
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            
            // Left Panel: Controls
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawControls();
            EditorGUILayout.EndVertical();

            GUILayout.Space(20);

            // Right Panel: Grid
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawGrid();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawControls()
        {
            if (_gameConfig == null)
            {
                EditorGUILayout.HelpBox("GameConfig not found at Assets/GameConfig.asset", MessageType.Error);
                if (GUILayout.Button("Reload Config")) LoadConfig();
                return;
            }

            GUILayout.Label("Level Settings", EditorStyles.boldLabel);
            _levelId = EditorGUILayout.TextField("Level ID", _levelId);
            _width = EditorGUILayout.IntField("Width", _width);
            _height = EditorGUILayout.IntField("Height", _height);

            GUILayout.Space(5);
            _configToLoad = (LevelConfig)EditorGUILayout.ObjectField("Config to Load", _configToLoad, typeof(LevelConfig), false);
            if (GUILayout.Button("Load Selected Config"))
            {
                if (_configToLoad != null)
                {
                    LoadLevelFromConfig(_configToLoad);
                }
            }

            GUILayout.Space(5);
            if (GUILayout.Button("Clear Grid"))
            {
                if (EditorUtility.DisplayDialog("Clear Grid", "Are you sure you want to clear all thermometers and solution?", "Yes", "No"))
                {
                    _thermometers.Clear();
                    _solution.Clear();
                    _selectedThermometerIndex = -1;
                    _activeConfig = null;
                }
            }

            GUILayout.Space(10);
            GUILayout.Label("Thermometers", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Add Thermometer"))
            {
                Color color = Color.white;
                int colorId = -1;
                if (_gameConfig.ColorPalette != null && _gameConfig.ColorPalette.Count > 0)
                {
                    int randomIndex = Random.Range(0, _gameConfig.ColorPalette.Count);
                    var entry = _gameConfig.ColorPalette[randomIndex];
                    color = entry.Color;
                    colorId = entry.Id;
                }
                
                _thermometers.Add(new ThermometerData(new List<Vector2Int>(), colorId));
                _selectedThermometerIndex = _thermometers.Count - 1;
            }

            for (int i = 0; i < _thermometers.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Toggle(_selectedThermometerIndex == i, $"Thermometer {i}", "Button"))
                {
                    _selectedThermometerIndex = i;
                }

                if (_gameConfig.ColorPalette != null && _gameConfig.ColorPalette.Count > 0)
                {
                    string[] colorNames = _gameConfig.ColorPalette.Select(c => $"Color {c.Id}").ToArray();
                    int currentColorIndex = -1;
                    // Try to find the color in the palette
                    var matchedColor = _gameConfig.ColorPalette.FirstOrDefault(cp => cp.Id == _thermometers[i].ColorId);
                    if (matchedColor != null) currentColorIndex = _gameConfig.ColorPalette.IndexOf(matchedColor);

                    int newColorIndex = EditorGUILayout.Popup(currentColorIndex, colorNames, GUILayout.Width(80));
                    if (newColorIndex != currentColorIndex && newColorIndex >= 0)
                    {
                        var entry = _gameConfig.ColorPalette[newColorIndex];
                        _thermometers[i].ColorId = entry.Id;
                    }
                }
                
                using (new EditorGUI.DisabledGroupScope(true))
                {
                    EditorGUILayout.ColorField(_gameConfig.ColorPalette.FirstOrDefault(cp => cp.Id == _thermometers[i].ColorId)?.Color??Color.red);
                }
                
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
                Color c = _gameConfig.ColorPalette.FirstOrDefault(cp => cp.Id == thermo.ColorId)?.Color ??
                          Color.red;
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

            LevelConfig levelConfig;
            bool isNew = false;

            if (_activeConfig != null && _activeConfig.Id == _levelId)
            {
                levelConfig = _activeConfig;
            }
            else
            {
                levelConfig = ScriptableObject.CreateInstance<LevelConfig>();
                isNew = true;
            }

            levelConfig.Id = _levelId;
            levelConfig.Width = _width;
            levelConfig.Height = _height;
            levelConfig.RowConstraints = rowConstraints;
            levelConfig.ColumnConstraints = colConstraints;

            List<ThermometerData> savedThermometers = new List<ThermometerData>();
            foreach (var thermometer in _thermometers)
            {
                int fill = 0;
                foreach (var cell in thermometer.Cells)
                {
                    if (_solution.Contains(cell)) fill++;
                    else break;
                }

                // Create a new instance to ensure it's properly serialized in the asset
                var data = new ThermometerData(new List<Vector2Int>(thermometer.Cells), thermometer.ColorId);
                data.SolutionFill = fill;
                savedThermometers.Add(data);
            }
            levelConfig.Thermometers = savedThermometers;

            if (isNew)
            {
                string path = $"Assets/Core/Level/Configs/{_levelId}.asset";
                AssetDatabase.CreateAsset(levelConfig, path);
                _activeConfig = levelConfig;
                Debug.Log($"Level saved to new asset: {path}");
            }
            else
            {
                EditorUtility.SetDirty(levelConfig);
                Debug.Log($"Existing level asset '{levelConfig.name}' updated.");
            }

            AssetDatabase.SaveAssets();
            
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = levelConfig;
        }

        private void LoadLevelFromConfig(LevelConfig config)
        {
            _activeConfig = config;
            _levelId = config.Id;
            _width = config.Width;
            _height = config.Height;
            _thermometers.Clear();
            _solution.Clear();

            foreach (var thermometer in config.Thermometers)
            {
                var newThermo = new ThermometerData(new List<Vector2Int>(thermometer.Cells), thermometer.ColorId);
                newThermo.SolutionFill = thermometer.SolutionFill;
                _thermometers.Add(newThermo);

                // Reconstruct solution based on SolutionFill
                for (int i = 0; i < newThermo.SolutionFill && i < newThermo.Cells.Count; i++)
                {
                    _solution.Add(newThermo.Cells[i]);
                }
            }

            _selectedThermometerIndex = -1;
            Repaint();
            Debug.Log($"Level {config.Id} loaded for modification");
        }
    }
}
