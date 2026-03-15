using UnityEditor;
using UnityEngine;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 05 — Global Player Settings
    ///
    /// Applies platform-independent project settings:
    ///   · Linear color space
    ///   · Both input systems active (Old + New)
    ///   · Portrait + Landscape orientation
    ///   · IL2CPP managed code stripping (Medium)
    ///   · Strip Engine Code enabled
    /// </summary>
    public class Step05_PlayerSettings : SetupStepBase
    {
        public Step05_PlayerSettings()
        {
            Name        = "Global Player Settings";
            Description = "Sets color space (Linear), input handling (Both), " +
                          "orientation, and managed code stripping.";
        }

        protected override void Run()
        {
            // ── Color Space ───────────────────────────────────────────────────────
            PlayerSettings.colorSpace = ColorSpace.Linear;

            // ── Orientation ───────────────────────────────────────────────────────
            PlayerSettings.defaultInterfaceOrientation      = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait       = true;
            PlayerSettings.allowedAutorotateToLandscapeLeft  = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;

            // ── Active Input Handling: Both (0=Old, 1=New, 2=Both) ────────────────
            // Unity doesn't expose a direct setter for this — use SerializedObject.
            SetActiveInputHandling(2);

            // ── Code Stripping ────────────────────────────────────────────────────
            PlayerSettings.stripEngineCode = true;
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.Medium);
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS,     ManagedStrippingLevel.Medium);

            Succeed("Color space: Linear. Input: Both. Stripping: Medium. Orientation: Portrait + Landscape.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Sets the Active Input Handling via serialized ProjectSettings.
        /// 0 = Input Manager (Old), 1 = Input System (New), 2 = Both.
        /// </summary>
        private static void SetActiveInputHandling(int value)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets == null || assets.Length == 0)
            {
                Debug.LogWarning("[MobileSetup] Could not load ProjectSettings.asset for input handler.");
                return;
            }

            var so   = new SerializedObject(assets[0]);
            var prop = so.FindProperty("activeInputHandler");

            if (prop == null)
            {
                Debug.LogWarning("[MobileSetup] 'activeInputHandler' property not found in ProjectSettings.");
                return;
            }

            prop.intValue = value;
            so.ApplyModifiedProperties();
        }
    }
}
