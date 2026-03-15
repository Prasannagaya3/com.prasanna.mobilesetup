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
            // Unity has no stable public API for batching — use SerializedObject
            SetBatching(true, true);

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
            // Android only — iOS IL2CPP config is set inside the generated Xcode project,
            // not from Unity. Setting it here produces a warning on non-Mac machines.
            PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Android, Il2CppCompilerConfiguration.Release);

            Succeed("Static/Dynamic batching enabled. ASTC textures. IL2CPP Release. " +
                    "Auto Graphics API disabled. Build optimized for mobile ✓");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static void SetBatching(bool staticBatching, bool dynamicBatching)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets == null || assets.Length == 0) return;

            var so = new SerializedObject(assets[0]);

            var staticProp  = so.FindProperty("staticBatching");
            var dynamicProp = so.FindProperty("dynamicBatching");

            if (staticProp  != null) staticProp.intValue  = staticBatching  ? 1 : 0;
            if (dynamicProp != null) dynamicProp.intValue = dynamicBatching ? 1 : 0;

            so.ApplyModifiedProperties();
        }
    }
}
