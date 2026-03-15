using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

// URP types are only available after the URP package is installed.
// The URP_INSTALLED define is set in the asmdef versionDefines once
// com.unity.render-pipelines.universal >= 7.0.0 is present.
#if URP_INSTALLED
using UnityEngine.Rendering.Universal;
#endif

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 02 — URP Configurator
    ///
    /// Creates a UniversalRenderPipelineAsset and assigns it to both
    /// Graphics Settings and all Quality levels.
    ///
    /// Requires the URP package (com.unity.render-pipelines.universal) to be
    /// installed. If packages are still installing from Step 01, this step will
    /// warn gracefully — re-run setup after Unity's domain reload.
    /// </summary>
    public class Step02_URPConfigurator : SetupStepBase
    {
        public Step02_URPConfigurator()
        {
            Name        = "URP Configurator";
            Description = "Creates URP Pipeline Asset and assigns it to Graphics & Quality settings.";
        }

        protected override void Run()
        {
#if !URP_INSTALLED
            Warn("URP package is not installed yet. " +
                 "Wait for Unity to finish installing packages, then re-run setup.");
#else
            EnsureSettingsFolder();

            var pipelineAsset = GetOrCreatePipelineAsset();

            // Assign to Graphics settings (global default)
            GraphicsSettings.defaultRenderPipeline = pipelineAsset;
            EditorUtility.SetDirty(GraphicsSettings.defaultRenderPipeline);

            // Assign to every Quality level
            string[] qualityNames = QualitySettings.names;
            for (int i = 0; i < qualityNames.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipelineAsset;
            }

            // Restore to the first quality level
            QualitySettings.SetQualityLevel(0, false);

            AssetDatabase.SaveAssets();
            Succeed($"URP configured across {qualityNames.Length} quality level(s). " +
                    $"Asset: {SetupConfig.URPPipelineAssetPath}");
#endif
        }

#if URP_INSTALLED
        private static UniversalRenderPipelineAsset GetOrCreatePipelineAsset()
        {
            // Return existing asset if already created
            var existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(
                SetupConfig.URPPipelineAssetPath);
            if (existing != null) return existing;

            // Create Universal Renderer Data
            var rendererData = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(
                SetupConfig.URPRendererDataPath);

            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
                AssetDatabase.CreateAsset(rendererData, SetupConfig.URPRendererDataPath);
            }

            // Create Pipeline Asset using the renderer data
            var pipelineAsset = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(pipelineAsset, SetupConfig.URPPipelineAssetPath);
            AssetDatabase.SaveAssets();

            return pipelineAsset;
        }
#endif

        private static void EnsureSettingsFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");
        }
    }
}
