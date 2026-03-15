using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 10 — Mobile Build Optimizer
    ///
    /// Applies mobile-specific optimizations:
    ///   · Static Batching
    ///   · Dynamic Batching
    ///   · GPU Instancing hint (project-wide)
    ///   · Disable Auto Graphics API (we set it explicitly in Steps 03 & 04)
    ///   · Managed Code Stripping (already set in Step 05 — confirmed here)
    ///   · Accelerometer Frequency set to a sensible default
    /// </summary>
    public class Step10_BuildOptimizer : SetupStepBase
    {
        public Step10_BuildOptimizer()
        {
            Name        = "Mobile Build Optimizer";
            Description = "Enables Static/Dynamic Batching, disables Auto Graphics API, " +
                          "and applies final mobile performance settings.";
        }

        protected override void Run()
        {
            // ── Batching ──────────────────────────────────────────────────────────
            // Unity 2022+: use SetBatchingForPlatform(group, staticBatching, dynamicBatching)
            PlayerSettings.SetBatchingForPlatform(BuildTargetGroup.Android, 1, 1);
            PlayerSettings.SetBatchingForPlatform(BuildTargetGroup.iOS,     1, 1);

            // ── Auto Graphics API off — we set APIs explicitly ────────────────────
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS,     false);

            // ── Accelerometer ─────────────────────────────────────────────────────
            // 60 Hz is standard; 0 = disabled. Reduce to 30 to save battery if not needed.
            PlayerSettings.accelerometerFrequency = 60;

            // ── Confirm stripping levels (defense-in-depth vs Step 05) ─────────────
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Medium);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS,     ManagedStrippingLevel.Medium);

            // ── Texture Compression: ASTC ─────────────────────────────────────────
            // Unity 2022+: textureCompressionFormat moved to EditorUserBuildSettings
            EditorUserBuildSettings.androidBuildSubtarget = MobileTextureSubtarget.ASTC;

            // ── IL2CPP Compiler Configuration ────────────────────────────────────
            // Release = full optimizations in production builds
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.iOS,     Il2CppCompilerConfiguration.Release);

            Succeed("Static/Dynamic batching enabled. ASTC textures. IL2CPP Release. " +
                    "Auto Graphics API disabled. Build optimized for mobile ✓");
        }
    }
}
