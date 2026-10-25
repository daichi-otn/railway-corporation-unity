using RailwayCorporation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RailwayCorporation.Editor
{
    public static class ProjectSetup
    {
        public static void CreateInitialScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var game = new GameObject("Railway Corporation Game");
            game.AddComponent<RailwayCompanyGame>();
            PlayerSettings.companyName = "Daichi Otonadayo";
            PlayerSettings.productName = "Railway Corporation";
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Main.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Main.unity", true) };
            AssetDatabase.SaveAssets();
        }
    }
}
