using System.Collections.Generic;
using System.IO;
using Core.Level;
using Infrastructure.StateMachine.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tools.EditorTools.Editor
{
    public static class MenuTools
    {
        [MenuItem("Tools/Levels/Create Sample Level")]
        public static void CreateSampleLevel()
        {
            var levelConfig = ScriptableObject.CreateInstance<LevelConfig>();

            // 3x3 simple puzzle
            // 1 2 1
            // 2 1 1
            // 1 1 2

            var thermometers = new List<ThermometerData>
            {
                new ThermometerData(new List<Vector2Int>
                    { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, 2) }),
                new ThermometerData(new List<Vector2Int>
                    { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(1, 2) }),
                new ThermometerData(new List<Vector2Int>
                    { new Vector2Int(2, 0), new Vector2Int(2, 1), new Vector2Int(2, 2) })
            };

            // Use reflection or change LevelConfig to allow setting fields for this tool if they are private
            // Since they are [field: SerializeField] public ... { get; private set; } we might need to use serialized object
            SerializedObject so = new SerializedObject(levelConfig);
            so.FindProperty("<Id>k__BackingField").stringValue = "sample_3x3";
            so.FindProperty("<Width>k__BackingField").intValue = 3;
            so.FindProperty("<Height>k__BackingField").intValue = 3;

            var rowConstraints = so.FindProperty("<RowConstraints>k__BackingField");
            rowConstraints.arraySize = 3;
            rowConstraints.GetArrayElementAtIndex(0).intValue = 2; // Row 0 (bottom)
            rowConstraints.GetArrayElementAtIndex(1).intValue = 1; // Row 1
            rowConstraints.GetArrayElementAtIndex(2).intValue = 2; // Row 2 (top)

            var colConstraints = so.FindProperty("<ColumnConstraints>k__BackingField");
            colConstraints.arraySize = 3;
            colConstraints.GetArrayElementAtIndex(0).intValue = 1; // Col 0
            colConstraints.GetArrayElementAtIndex(1).intValue = 2; // Col 1
            colConstraints.GetArrayElementAtIndex(2).intValue = 2; // Col 2

            so.ApplyModifiedProperties();

            // List<ThermometerData> is harder via serializedProperty if we don't want to loop a lot
            // But LevelConfig.Thermometers has a public getter, but private setter.
            // Let's just use reflection for simplicity in a tool
            typeof(LevelConfig).GetProperty("Thermometers").SetValue(levelConfig, thermometers);

            AssetDatabase.CreateAsset(levelConfig, "Assets/Core/Level/SampleLevel.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = levelConfig;
        }

        [MenuItem("Play/PlayFromStart", false, 1)]
        public static void Play()
        {
            EditorSceneManager.OpenScene($"Assets/Meta/{GameStartState.START_SCENE}.unity");
            EditorApplication.EnterPlaymode();
        }

        [MenuItem("Play/ClearSaveAndPlay", false, 2)]
        public static void ClearSaveAndPlay()
        {
            ClearSave();
            Play();
        }

        private static void ClearSave()
        {
            PlayerPrefs.DeleteAll();
            foreach (var directory in Directory.GetDirectories(Application.persistentDataPath))
            {
                DirectoryInfo data_dir = new DirectoryInfo(directory);
                data_dir.Delete(true);
            }

            foreach (var file in Directory.GetFiles(Application.persistentDataPath))
            {
                FileInfo file_info = new FileInfo(file);
                file_info.Delete();
            }
        }
    }
}