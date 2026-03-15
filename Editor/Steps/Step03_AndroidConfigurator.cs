using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 03 — Android Configurator
    ///
    /// Applies all Android-specific Player Settings:
    ///   · IL2CPP scripting backend
    ///   · ARM64 target architecture
    ///   · Vulkan + OpenGLES3 graphics APIs
    ///   · Minimum API Level 26 (Android 8.0)
    ///   · Multithreaded rendering
    ///   · GPU skinning
    /// </summary>
    public class Step03_AndroidConfigurator : SetupStepBase
    {
        public Step03_AndroidConfigurator()
        {
            Name        = "Android Configurator";
            Description = "Configures Android Player Settings: IL2CPP, ARM64, Vulkan, API 26+, " +
                          "multithreaded rendering and GPU skinning.";
        }

        protected override void Run()
        {
            // ── Identity ─────────────────────────────────────────────────────────
            PlayerSettings.companyName = SetupConfig.CompanyName;
            PlayerSettings.SetApplicationIdentifier(
                BuildTargetGroup.Android, SetupConfig.AndroidBundleId);

            // ── Scripting Backend ─────────────────────────────────────────────────
            PlayerSettings.SetScriptingBackend(
                BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            // ── Architecture ──────────────────────────────────────────────────────
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;

            // ── API Level ────────────────────────────────────────────────────────
            PlayerSettings.Android.minSdkVersion =
                (AndroidSdkVersions)SetupConfig.AndroidMinSdkVersion;  // API 26
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;

            // ── Graphics APIs (Vulkan first, then OpenGLES3 as fallback) ──────────
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[]
            {
                GraphicsDeviceType.Vulkan,
                GraphicsDeviceType.OpenGLES3,
            });

            // ── Internet Access ───────────────────────────────────────────────────
            PlayerSettings.Android.forceInternetPermission = true;

            // ── Multithreaded Rendering ───────────────────────────────────────────
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.Android, true);

            // ── GPU Skinning ──────────────────────────────────────────────────────
            PlayerSettings.gpuSkinning = true;

            // ── .NET API ──────────────────────────────────────────────────────────
            PlayerSettings.SetApiCompatibilityLevel(
                BuildTargetGroup.Android, ApiCompatibilityLevel.NET_Standard);

            EditorUtility.SetDirty(Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings"));

            // ── Switch Active Build Target to Android ─────────────────────────────
            // This triggers a domain reload — it runs last so all settings are saved first.
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                    BuildTargetGroup.Android, BuildTarget.Android);
            }

            Succeed($"Android configured. Bundle ID: {SetupConfig.AndroidBundleId}, " +
                    $"Min SDK: {SetupConfig.AndroidMinSdkVersion}. Active platform switched to Android.");
        }
    }
}
