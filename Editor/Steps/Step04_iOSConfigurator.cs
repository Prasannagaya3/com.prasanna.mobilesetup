using UnityEditor;
using UnityEngine.Rendering;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 04 — iOS Configurator
    ///
    /// Applies all iOS-specific Player Settings:
    ///   · IL2CPP scripting backend
    ///   · ARM64 architecture
    ///   · Minimum iOS version 13
    ///   · Metal graphics API
    ///   · Camera usage description
    /// </summary>
    public class Step04_iOSConfigurator : SetupStepBase
    {
        public Step04_iOSConfigurator()
        {
            Name        = "iOS Configurator";
            Description = "Configures iOS Player Settings: IL2CPP, ARM64, Metal, iOS 13+ minimum.";
        }

        protected override void Run()
        {
            // ── Identity ─────────────────────────────────────────────────────────
            PlayerSettings.SetApplicationIdentifier(
                BuildTargetGroup.iOS, SetupConfig.iOSBundleId);

            // ── Scripting Backend ─────────────────────────────────────────────────
            PlayerSettings.SetScriptingBackend(
                BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);

            // ── Architecture: ARM64 ───────────────────────────────────────────────
            // 0 = ARMv7, 1 = ARM64, 2 = Universal
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);

            // ── Minimum iOS Version ───────────────────────────────────────────────
            PlayerSettings.iOS.targetOSVersionString = SetupConfig.iOSMinVersion;

            // ── Graphics API: Metal only ──────────────────────────────────────────
            // Metal is only configurable when the iOS Build Support module is installed.
            // We check by attempting to get the current APIs — if the list contains
            // Metal already (module installed) we set explicitly; otherwise we leave
            // Auto Graphics API enabled so Unity picks the correct default.
            var currentApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
            bool iosModuleInstalled = currentApis != null && currentApis.Length > 0;

            if (iosModuleInstalled)
            {
                PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
                PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[]
                {
                    GraphicsDeviceType.Metal,
                });
            }
            // On Windows without iOS module: leave Auto Graphics API enabled (no warning)

            // ── Multithreaded Rendering ───────────────────────────────────────────
            PlayerSettings.SetMobileMTRendering(BuildTargetGroup.iOS, true);

            // ── Camera Usage Description ──────────────────────────────────────────
            PlayerSettings.iOS.cameraUsageDescription = SetupConfig.iOSCameraUsageDesc;

            // ── .NET API ──────────────────────────────────────────────────────────
            PlayerSettings.SetApiCompatibilityLevel(
                BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_Standard);

            Succeed($"iOS configured. Bundle ID: {SetupConfig.iOSBundleId}, " +
                    $"Min version: iOS {SetupConfig.iOSMinVersion}.");
        }
    }
}
