using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 02 — URP Configurator
    ///
    /// Creates a UniversalRenderPipelineAsset and assigns it to both
    /// Graphics Settings and all Quality levels.
    ///
    /// Uses reflection so no hard assembly reference to URP is required.
    /// Works whether URP was pre-installed or just added by Step 01.
    /// </summary>
    public class Step02_URPConfigurator : SetupStepBase
    {
        // URP type names — stable across URP 10–17
        private const string RendererDataTypeName    = "UnityEngine.Rendering.Universal.UniversalRendererData";
        private const string PipelineAssetTypeName   = "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset";
        private const string CreateMethodName        = "Create";

        public Step02_URPConfigurator()
        {
            Name        = "URP Configurator";
            Description = "Creates URP Pipeline Asset and assigns it to Graphics & Quality settings.";
        }

        protected override void Run()
        {
            // ── Check URP is available via reflection ─────────────────────────────
            Type pipelineAssetType = FindType(PipelineAssetTypeName);
            Type rendererDataType  = FindType(RendererDataTypeName);

            if (pipelineAssetType == null || rendererDataType == null)
            {
                Warn("URP package not found. Wait for Unity to finish installing packages, " +
                     "then re-run setup.");
                return;
            }

            EnsureSettingsFolder();

            // ── Get or create Pipeline Asset ──────────────────────────────────────
            var pipelineAsset = GetOrCreatePipelineAsset(pipelineAssetType, rendererDataType);
            if (pipelineAsset == null)
            {
                Warn("Could not create URP Pipeline Asset. Re-run setup after Unity reloads.");
                return;
            }

            // ── Assign to Graphics Settings ───────────────────────────────────────
            GraphicsSettings.defaultRenderPipeline = pipelineAsset;

            // ── Assign to all Quality levels ──────────────────────────────────────
            string[] qualityNames = QualitySettings.names;
            for (int i = 0; i < qualityNames.Length; i++)
            {
                QualitySettings.SetQualityLevel(i, false);
                QualitySettings.renderPipeline = pipelineAsset;
            }
            QualitySettings.SetQualityLevel(0, false);

            AssetDatabase.SaveAssets();
            Succeed($"URP configured across {qualityNames.Length} quality level(s). " +
                    $"Asset: {SetupConfig.URPPipelineAssetPath}");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static RenderPipelineAsset GetOrCreatePipelineAsset(
            Type pipelineAssetType, Type rendererDataType)
        {
            // Return existing asset if already created
            var existing = AssetDatabase.LoadAssetAtPath(
                SetupConfig.URPPipelineAssetPath, pipelineAssetType) as RenderPipelineAsset;
            if (existing != null) return existing;

            // Create renderer data
            var rendererData = AssetDatabase.LoadAssetAtPath(
                SetupConfig.URPRendererDataPath, rendererDataType) as ScriptableObject;

            if (rendererData == null)
            {
                rendererData = ScriptableObject.CreateInstance(rendererDataType);
                AssetDatabase.CreateAsset(rendererData, SetupConfig.URPRendererDataPath);
            }

            // Call UniversalRenderPipelineAsset.Create(rendererData) via reflection
            MethodInfo createMethod = pipelineAssetType.GetMethod(
                CreateMethodName,
                BindingFlags.Static | BindingFlags.Public,
                null,
                new[] { rendererDataType },
                null);

            if (createMethod == null)
            {
                // Fallback: try Create() with no arguments (older URP versions)
                createMethod = pipelineAssetType.GetMethod(
                    CreateMethodName,
                    BindingFlags.Static | BindingFlags.Public);
            }

            if (createMethod == null) return null;

            var pipelineAsset = createMethod.Invoke(null,
                createMethod.GetParameters().Length == 1
                    ? new object[] { rendererData }
                    : null) as RenderPipelineAsset;

            if (pipelineAsset == null) return null;

            AssetDatabase.CreateAsset(pipelineAsset, SetupConfig.URPPipelineAssetPath);
            AssetDatabase.SaveAssets();

            return pipelineAsset;
        }

        private static void EnsureSettingsFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
                AssetDatabase.CreateFolder("Assets", "Settings");
        }

        /// <summary>Searches all loaded assemblies for a type by full name.</summary>
        private static Type FindType(string fullTypeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullTypeName);
                if (type != null) return type;
            }
            return null;
        }
    }
}
