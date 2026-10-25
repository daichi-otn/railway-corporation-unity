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
            EnsureWorldMaterial();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), "Assets/Main.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Main.unity", true) };
            AssetDatabase.SaveAssets();
        }

        private static void EnsureWorldMaterial()
        {
            const string folder = "Assets/Resources";
            const string path = folder + "/WorldMaterial.mat";
            if (AssetDatabase.LoadAssetAtPath<Material>(path) != null) return;
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "Resources");
            var material = new Material(Shader.Find("Unlit/Color"));
            AssetDatabase.CreateAsset(material, path);
        }

        public static void BuildWebGL()
        {
            CreateInitialScene();
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Main.unity" },
                locationPathName = "Build/WebGL",
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new System.Exception("WebGL build failed: " + report.summary.result);
        }
    }
}
