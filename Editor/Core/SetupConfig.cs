using System.Collections.Generic;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Single source of truth for all Mobile Setup configuration values.
    ///
    /// This is the ONLY file you ever need to edit to customise the setup
    /// for your studio. Change values here; the steps pick them up automatically.
    /// </summary>
    public static class SetupConfig
    {
        // ── Identity ──────────────────────────────────────────────────────────────
        public const string CompanyName = "Trio Games";

        // ── UPM Packages ──────────────────────────────────────────────────────────
        // Key   = package id
        // Value = minimum version compatible with Unity 2022.3 LTS
        //         Update versions here when you upgrade your Unity version.
        public static readonly Dictionary<string, string> RequiredPackages =
            new Dictionary<string, string>
            {
                { "com.unity.textmeshpro",                "3.0.6"  },
                { "com.unity.inputsystem",                "1.7.0"  },
                { "com.unity.mobile.notifications",       "2.3.2"  },
                { "com.unity.render-pipelines.universal", "14.0.9" },
                { "com.unity.timeline",                   "1.7.6"  },
                { "com.unity.cinemachine",                "2.9.7"  },
            };

        // ── Android ───────────────────────────────────────────────────────────────
        public const string AndroidBundleId      = "com.triogames.game";
        public const int    AndroidMinSdkVersion = 26;   // Android 8.0

        // ── iOS ───────────────────────────────────────────────────────────────────
        public const string iOSBundleId          = "com.triogames.game";
        public const string iOSMinVersion        = "13.0";
        public const string iOSCameraUsageDesc   = "Camera access required for future features.";

        // ── URP Asset Paths ───────────────────────────────────────────────────────
        public const string URPPipelineAssetPath = "Assets/Settings/URP_PipelineAsset.asset";
        public const string URPRendererDataPath  = "Assets/Settings/URP_RendererData.asset";

        // ── Folder Structure ──────────────────────────────────────────────────────
        // Add or remove paths here to change what gets created.
        public static readonly string[] FolderStructure =
        {
            "Assets/Core",
            "Assets/Core/Scripts",
            "Assets/Core/Managers",
            "Assets/Core/Services",
            "Assets/Core/Utilities",
            "Assets/Art",
            "Assets/Art/Textures",
            "Assets/Art/Materials",
            "Assets/Art/Fonts",
            "Assets/UI",
            "Assets/UI/UXML",
            "Assets/UI/USS",
            "Assets/Scenes",
            "Assets/Prefabs",
            "Assets/Plugins",
            "Assets/Settings",
        };

        // ── Scene ─────────────────────────────────────────────────────────────────
        public const string DefaultScenePath = "Assets/Scenes/Main.unity";

        // ── UI Toolkit ────────────────────────────────────────────────────────────
        public const string MainUXMLPath = "Assets/UI/UXML/MainUI.uxml";
        public const string MainUSSPath  = "Assets/UI/USS/MainUI.uss";

        // ── EditorPrefs Key ───────────────────────────────────────────────────────
        // Change this key if you release a v2 that should re-run on existing projects.
        public const string SetupCompletedKey = "com.prasanna.mobilesetup.v1.completed";
    }
}
