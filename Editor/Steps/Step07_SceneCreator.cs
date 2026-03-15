using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 07 — Scene Creator
    ///
    /// Creates Assets/Scenes/Main.unity with:
    ///   · Main Camera (tagged MainCamera)
    ///   · Directional Light
    ///   · EventSystem + StandaloneInputModule
    ///
    /// Sets the scene as the default startup scene in Build Settings.
    /// Skips creation if the scene already exists.
    /// </summary>
    public class Step07_SceneCreator : SetupStepBase
    {
        public Step07_SceneCreator()
        {
            Name        = "Scene Creator";
            Description = "Creates Main.unity with Camera, Directional Light and EventSystem, " +
                          "and registers it as the startup scene.";
        }

        protected override void Run()
        {
            // Skip if scene already exists
            if (System.IO.File.Exists(
                System.IO.Path.Combine(
                    Application.dataPath.Replace("/Assets", ""),
                    SetupConfig.DefaultScenePath)))
            {
                RegisterStartupScene();
                Succeed($"Scene already exists at {SetupConfig.DefaultScenePath}. " +
                        "Registered as startup scene.");
                return;
            }

            // Create a new empty scene
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // ── Main Camera ───────────────────────────────────────────────────────
            var cameraGO = new GameObject("Main Camera");
            var cam      = cameraGO.AddComponent<Camera>();
            cameraGO.AddComponent<AudioListener>();
            cameraGO.tag                 = "MainCamera";
            cam.clearFlags               = CameraClearFlags.SolidColor;
            cam.backgroundColor          = new Color(0.19f, 0.19f, 0.19f, 1f);
            cameraGO.transform.position  = new Vector3(0f, 1f, -10f);

            // ── Directional Light ─────────────────────────────────────────────────
            var lightGO              = new GameObject("Directional Light");
            var light                = lightGO.AddComponent<Light>();
            light.type               = LightType.Directional;
            light.color              = new Color(1f, 0.96f, 0.84f);
            light.intensity          = 1f;
            light.shadows            = LightShadows.Soft;
            lightGO.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // ── EventSystem ───────────────────────────────────────────────────────
            var eventGO = new GameObject("EventSystem");
            eventGO.AddComponent<EventSystem>();
            eventGO.AddComponent<StandaloneInputModule>();
            // Note: If you are using the New Input System package exclusively,
            // replace StandaloneInputModule with InputSystemUIInputModule.

            // ── Save ──────────────────────────────────────────────────────────────
            EditorSceneManager.SaveScene(scene, SetupConfig.DefaultScenePath);

            RegisterStartupScene();

            Succeed($"Created {SetupConfig.DefaultScenePath} and set as startup scene.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static void RegisterStartupScene()
        {
            var buildScene  = new EditorBuildSettingsScene(SetupConfig.DefaultScenePath, true);
            EditorBuildSettings.scenes = new[] { buildScene };
        }
    }
}
