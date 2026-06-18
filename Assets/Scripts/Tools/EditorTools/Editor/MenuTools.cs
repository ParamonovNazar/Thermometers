using System.IO;
using Infrastructure.StateMachine.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tools.EditorTools.Editor
{
    public static class MenuTools
    {
        [MenuItem("Play/PlayFromStart", false, 1)]
        public static void Play()
        {
            EditorSceneManager.OpenScene($"Assets/{GameStartState.START_SCENE}.unity");
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