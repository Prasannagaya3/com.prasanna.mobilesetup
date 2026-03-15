using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 11 — Verification
    ///
    /// Reads back every setting applied by Steps 01–10 and reports
    /// a detailed pass/fail for each check. Safe to run at any time.
    /// Does not modify any settings — read-only.
    /// </summary>
    public class Step11_Verify : SetupStepBase
    {
        private readonly List<(string label, bool pass, string detail)> _checks
            = new List<(string, bool, string)>();

        public Step11_Verify()
        {
            Name        = "Verify All Settings";
            Description = "Read-only check of every setting. Reports pass/fail for each. " +
                          "Check the Console for the full report.";
        }

        protected override void Run()
        {
            _checks.Clear();

            CheckPackages();
            CheckURP();
            CheckAndroid();
            CheckiOS();
            CheckPlayerSettings();
            CheckFolderStructure();
            CheckScene();
            CheckUIToolkit();
            CheckVCS();
            CheckBuildSettings();

            PrintReport();

            int passed = _checks.FindAll(c => c.pass).Count;
            int total  = _checks.Count;
            int failed = total - passed;

            if (failed == 0)
                Succeed($"All {total} checks passed ✅");
            else
                Warn($"{passed}/{total} checks passed. {failed} issue(s) found — see Console for details.");
        }

        // ── Individual Checks ─────────────────────────────────────────────────────

        private void CheckPackages()
        {
            string manifestPath = Path.GetFullPath("Packages/manifest.json");
            if (!File.Exists(manifestPath))
            {
                Check("manifest.json exists", false, "File not found");
                return;
            }

            string manifest = File.ReadAllText(manifestPath);
            foreach (var kvp in SetupConfig.RequiredPackages)
            {
                bool found = manifest.Contains($"\"{kvp.Key}\"");
                Check($"Package: {kvp.Key}", found,
                    found ? kvp.Value : "Missing from manifest.json");
            }
        }

        private void CheckURP()
        {
            var pipeline = GraphicsSettings.defaultRenderPipeline;
            bool urpSet  = pipeline != null &&
                           pipeline.GetType().FullName.Contains("Universal");
            Check("URP assigned in Graphics Settings", urpSet,
                urpSet ? pipeline.name : "No render pipeline asset assigned");

            bool assetExists = File.Exists(Path.Combine(
                Application.dataPath.Replace("/Assets", ""),
                SetupConfig.URPPipelineAssetPath));
            Check("URP Pipeline Asset exists", assetExists, SetupConfig.URPPipelineAssetPath);
        }

        private void CheckAndroid()
        {
            // Scripting backend
            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            Check("Android: IL2CPP", backend == ScriptingImplementation.IL2CPP, backend.ToString());

            // Architecture
            var arch = PlayerSettings.Android.targetArchitectures;
            bool arm64Only = arch == AndroidArchitecture.ARM64;
            Check("Android: ARM64 only", arm64Only, arch.ToString());

            // Min SDK
            int minSdk = (int)PlayerSettings.Android.minSdkVersion;
            Check($"Android: Min SDK >= {SetupConfig.AndroidMinSdkVersion}",
                minSdk >= SetupConfig.AndroidMinSdkVersion, $"API {minSdk}");

            // Bundle ID
            string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            Check("Android: Bundle ID set", bundleId != "com.Company.ProductName",
                bundleId);

            // Graphics APIs
            var apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            bool hasVulkan = System.Array.Exists(apis,
                a => a == GraphicsDeviceType.Vulkan);
            Check("Android: Vulkan in Graphics APIs", hasVulkan,
                string.Join(", ", apis));

            // Active platform
            bool isAndroid = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
            Check("Active Build Target: Android", isAndroid,
                EditorUserBuildSettings.activeBuildTarget.ToString());
        }

        private void CheckiOS()
        {
            var backend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.iOS);
            Check("iOS: IL2CPP", backend == ScriptingImplementation.IL2CPP, backend.ToString());

            int arch = PlayerSettings.GetArchitecture(BuildTargetGroup.iOS);
            Check("iOS: ARM64 (arch=1)", arch == 1, $"arch={arch}");

            string minVer = PlayerSettings.iOS.targetOSVersionString;
            Check($"iOS: Min version >= {SetupConfig.iOSMinVersion}",
                !string.IsNullOrEmpty(minVer) && minVer != "7.0", minVer);

            var apis = PlayerSettings.GetGraphicsAPIs(BuildTarget.iOS);
            bool hasMetal = System.Array.Exists(apis,
                a => a == GraphicsDeviceType.Metal);
            Check("iOS: Metal in Graphics APIs", hasMetal, string.Join(", ", apis));
        }

        private void CheckPlayerSettings()
        {
            Check("Color Space: Linear",
                PlayerSettings.colorSpace == ColorSpace.Linear,
                PlayerSettings.colorSpace.ToString());

            Check("Strip Engine Code: Enabled",
                PlayerSettings.stripEngineCode, PlayerSettings.stripEngineCode.ToString());

            var stripAndroid = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android);
            Check("Android: Stripping = Medium",
                stripAndroid == ManagedStrippingLevel.Medium, stripAndroid.ToString());

            // Check active input handler via SerializedObject
            int inputMode = GetActiveInputHandling();
            Check("Input Handling: New Input System (1)",
                inputMode == 1, inputMode switch
                {
                    0 => "Old Input Manager",
                    1 => "New Input System ✓",
                    2 => "Both",
                    _ => $"Unknown ({inputMode})"
                });
        }

        private void CheckFolderStructure()
        {
            foreach (string folder in SetupConfig.FolderStructure)
            {
                bool exists = AssetDatabase.IsValidFolder(folder);
                Check($"Folder: {folder}", exists, exists ? "exists" : "missing");
            }
        }

        private void CheckScene()
        {
            bool sceneExists = File.Exists(Path.Combine(
                Application.dataPath.Replace("/Assets", ""),
                SetupConfig.DefaultScenePath));
            Check($"Scene: {SetupConfig.DefaultScenePath}", sceneExists,
                sceneExists ? "exists" : "missing");

            bool inBuildSettings = false;
            foreach (var s in EditorBuildSettings.scenes)
            {
                if (s.path == SetupConfig.DefaultScenePath && s.enabled)
                {
                    inBuildSettings = true;
                    break;
                }
            }
            Check("Main.unity in Build Settings", inBuildSettings,
                inBuildSettings ? "registered & enabled" : "not in Build Settings");
        }

        private void CheckUIToolkit()
        {
            bool uxmlExists = File.Exists(Path.Combine(
                Application.dataPath.Replace("/Assets", ""), SetupConfig.MainUXMLPath));
            bool ussExists  = File.Exists(Path.Combine(
                Application.dataPath.Replace("/Assets", ""), SetupConfig.MainUSSPath));

            Check($"UXML: {SetupConfig.MainUXMLPath}", uxmlExists,
                uxmlExists ? "exists" : "missing");
            Check($"USS:  {SetupConfig.MainUSSPath}", ussExists,
                ussExists ? "exists" : "missing");
        }

        private void CheckVCS()
        {
            Check("Serialization: Force Text",
                EditorSettings.serializationMode == SerializationMode.ForceText,
                EditorSettings.serializationMode.ToString());

            Check("Meta Files: Visible",
                EditorSettings.externalVersionControl == "Visible Meta Files",
                EditorSettings.externalVersionControl);
        }

        private void CheckBuildSettings()
        {
            var il2cppCfg = PlayerSettings.GetIl2CppCompilerConfiguration(BuildTargetGroup.Android);
            Check("Android: IL2CPP = Release",
                il2cppCfg == Il2CppCompilerConfiguration.Release, il2cppCfg.ToString());

            bool autoApiAndroid = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android);
            Check("Android: Auto Graphics API = Off", !autoApiAndroid,
                autoApiAndroid ? "ON (should be off)" : "Off ✓");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void Check(string label, bool pass, string detail)
        {
            _checks.Add((label, pass, detail));
        }

        private static int GetActiveInputHandling()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets == null || assets.Length == 0) return -1;
            var so   = new SerializedObject(assets[0]);
            var prop = so.FindProperty("activeInputHandler");
            return prop?.intValue ?? -1;
        }

        private void PrintReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("════════════════════════════════════════════════════════");
            sb.AppendLine("  MOBILE SETUP — VERIFICATION REPORT");
            sb.AppendLine("════════════════════════════════════════════════════════");

            string currentSection = "";
            foreach (var (label, pass, detail) in _checks)
            {
                // Detect section from label prefix
                string section = label.Contains("Package") ? "📦 Packages"
                    : label.Contains("URP")               ? "🎨 URP / Rendering"
                    : label.Contains("Android")           ? "🤖 Android"
                    : label.Contains("iOS")               ? "🍎 iOS"
                    : label.Contains("Color") || label.Contains("Strip") ||
                      label.Contains("Input")             ? "⚙️  Player Settings"
                    : label.Contains("Folder")            ? "📁 Folder Structure"
                    : label.Contains("Scene") ||
                      label.Contains("Main.unity")        ? "🎬 Scene"
                    : label.Contains("UXML") ||
                      label.Contains("USS")               ? "🖼  UI Toolkit"
                    : label.Contains("Serial") ||
                      label.Contains("Meta")              ? "🔧 VCS Settings"
                    :                                        "🏗  Build Settings";

                if (section != currentSection)
                {
                    sb.AppendLine($"\n  {section}");
                    sb.AppendLine("  ─────────────────────────────────────────────");
                    currentSection = section;
                }

                string icon   = pass ? "  ✅" : "  ❌";
                string status = pass ? "PASS" : "FAIL";
                sb.AppendLine($"{icon} [{status}]  {label}");
                if (!pass || !label.Contains("Folder"))
                    sb.AppendLine($"         → {detail}");
            }

            int passed = _checks.FindAll(c => c.pass).Count;
            int total  = _checks.Count;
            sb.AppendLine("\n════════════════════════════════════════════════════════");
            sb.AppendLine($"  RESULT: {passed}/{total} checks passed");
            sb.AppendLine("════════════════════════════════════════════════════════\n");

            if (passed == total)
                Debug.Log(sb.ToString());
            else
                Debug.LogWarning(sb.ToString());
        }
    }
}
