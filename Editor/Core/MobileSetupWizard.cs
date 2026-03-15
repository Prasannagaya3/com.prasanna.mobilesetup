using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Main EditorWindow for the Mobile Project Setup automation.
    /// Opens automatically on first install; accessible via Tools → Trio Games → Mobile Setup Wizard.
    /// </summary>
    public class MobileSetupWizard : EditorWindow
    {
        // ── Fields ────────────────────────────────────────────────────────────────
        private List<SetupStepBase> _steps;
        private Vector2             _scrollPos;
        private bool                _isRunning;

        // ── Colours ───────────────────────────────────────────────────────────────
        private static readonly Color ColSuccess = new Color(0.30f, 0.72f, 0.30f, 1f);
        private static readonly Color ColWarning = new Color(0.85f, 0.75f, 0.15f, 1f);
        private static readonly Color ColFailed  = new Color(0.80f, 0.22f, 0.22f, 1f);
        private static readonly Color ColRunning = new Color(0.20f, 0.50f, 0.90f, 1f);
        private static readonly Color ColPending = new Color(0.40f, 0.40f, 0.40f, 0.35f);
        private static readonly Color ColRule    = new Color(0.30f, 0.30f, 0.30f, 1f);

        // ── Auto-open on first install ────────────────────────────────────────────
        [InitializeOnLoadMethod]
        private static void AutoOpen()
        {
            if (!EditorPrefs.GetBool(SetupConfig.SetupCompletedKey, false))
                EditorApplication.delayCall += OpenWindow;
        }

        // ── Menu item ─────────────────────────────────────────────────────────────
        [MenuItem("Tools/Trio Games/Mobile Setup Wizard", priority = 0)]
        public static void OpenWindow()
        {
            var window = GetWindow<MobileSetupWizard>("Mobile Setup");
            window.minSize = new Vector2(460, 580);
            window.Show();
        }

        // ── Unity lifecycle ───────────────────────────────────────────────────────
        private void OnEnable() => _steps = BuildSteps();

        private void OnGUI()
        {
            DrawHeader();
            DrawStepList();
            GUILayout.FlexibleSpace();
            DrawFooter();
        }

        // ── Step factory ──────────────────────────────────────────────────────────
        private static List<SetupStepBase> BuildSteps() => new List<SetupStepBase>
        {
            new Step01_PackageInstaller(),
            new Step02_URPConfigurator(),
            new Step03_AndroidConfigurator(),
            new Step04_iOSConfigurator(),
            new Step05_PlayerSettings(),
            new Step06_FolderStructure(),
            new Step07_SceneCreator(),
            new Step08_UIToolkitSetup(),
            new Step09_VCSSettings(),
            new Step10_BuildOptimizer(),
            new Step11_Verify(),
        };

        // ── Drawing ───────────────────────────────────────────────────────────────
        private void DrawHeader()
        {
            EditorGUILayout.Space(10);

            var titleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 15,
                alignment = TextAnchor.MiddleCenter,
            };
            EditorGUILayout.LabelField("🚀  Mobile Starter Setup", titleStyle, GUILayout.Height(26));

            EditorGUILayout.LabelField(
                "Android & iOS  ·  URP  ·  UI Toolkit  ·  TextMeshPro",
                new GUIStyle(EditorStyles.centeredGreyMiniLabel),
                GUILayout.Height(18));

            EditorGUILayout.Space(6);
            DrawRule();
        }

        private void DrawStepList()
        {
            EditorGUILayout.Space(4);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            for (int i = 0; i < _steps.Count; i++)
                DrawStepRow(i + 1, _steps[i]);

            EditorGUILayout.EndScrollView();
        }

        private static void DrawStepRow(int number, SetupStepBase step)
        {
            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = step.Status switch
            {
                StepStatus.Success => ColSuccess,
                StepStatus.Warning => ColWarning,
                StepStatus.Failed  => ColFailed,
                StepStatus.Running => ColRunning,
                _                  => ColPending,
            };

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = prevBg;

            // Row header ─ icon + name
            string icon = step.Status switch
            {
                StepStatus.Success => "✅",
                StepStatus.Warning => "⚠️",
                StepStatus.Failed  => "❌",
                StepStatus.Running => "⏳",
                _                  => $"{number:D2}",
            };

            EditorGUILayout.LabelField($"{icon}  {step.Name}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(step.Description, EditorStyles.wordWrappedMiniLabel);

            // Status log line
            if (!string.IsNullOrEmpty(step.StatusLog))
            {
                var logStyle = new GUIStyle(EditorStyles.miniLabel) { wordWrap = true };
                logStyle.normal.textColor = step.Status == StepStatus.Failed
                    ? new Color(1f, 0.5f, 0.5f)
                    : new Color(0.65f, 0.65f, 0.65f);
                EditorGUILayout.LabelField($"  ↳ {step.StatusLog}", logStyle);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawFooter()
        {
            DrawRule();
            EditorGUILayout.Space(6);

            if (EditorPrefs.GetBool(SetupConfig.SetupCompletedKey, false))
            {
                EditorGUILayout.HelpBox(
                    "This project has already been set up. Click Run again to reapply all settings.",
                    MessageType.Info);
            }

            EditorGUI.BeginDisabledGroup(_isRunning);

            if (GUILayout.Button(
                    _isRunning ? "  Running…" : "  ▶   Run Full Setup",
                    GUILayout.Height(36)))
            {
                RunAllSteps();
            }

            EditorGUILayout.Space(4);

            if (GUILayout.Button("  🔍  Verify Settings Only", GUILayout.Height(28)))
            {
                RunVerifyOnly();
            }

            EditorGUILayout.Space(4);

            if (GUILayout.Button("Reset Setup State", EditorStyles.miniButton))
            {
                EditorPrefs.DeleteKey(SetupConfig.SetupCompletedKey);
                _steps = BuildSteps();
                Repaint();
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(8);
        }

        private void RunVerifyOnly()
        {
            _isRunning = true;
            var verifyStep = new Step11_Verify();

            EditorUtility.DisplayProgressBar("Verifying Setup…", "Running all checks…", 0.5f);
            verifyStep.Execute();
            EditorUtility.ClearProgressBar();

            _isRunning = false;

            string msg = verifyStep.Status == StepStatus.Success
                ? "✅ All checks passed!\n\nSee the Console window for the full detailed report."
                : $"⚠️ Some checks failed.\n\n{verifyStep.StatusLog}\n\nSee the Console window for the full detailed report.";

            EditorUtility.DisplayDialog("Verification Report", msg, "Open Console");
            EditorApplication.ExecuteMenuItem("Window/General/Console");
            Repaint();
        }

        // ── Execution ─────────────────────────────────────────────────────────────
        private void RunAllSteps()
        {
            _isRunning = true;
            _steps     = BuildSteps();  // Reset all step states

            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];

                EditorUtility.DisplayProgressBar(
                    "Mobile Setup — Please Wait",
                    $"Step {i + 1}/{_steps.Count}: {step.Name}",
                    (float)(i + 1) / _steps.Count);

                step.Execute();
                Repaint();
            }

            EditorUtility.ClearProgressBar();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorPrefs.SetBool(SetupConfig.SetupCompletedKey, true);

            _isRunning = false;
            Repaint();

            // Summary dialog
            bool anyFailed  = _steps.Exists(s => s.Status == StepStatus.Failed);
            bool anyWarning = _steps.Exists(s => s.Status == StepStatus.Warning);

            string message;
            if (anyFailed)
                message = "Setup finished with errors.\n\nReview the ❌ steps in the wizard window and check the Console for details.";
            else if (anyWarning)
                message = "Setup finished with warnings.\n\nMost likely cause: URP package is still installing.\n\nWait for Unity to finish reloading, then click ▶ Run Full Setup once more.";
            else
                message = "✅ Project is ready!\n\nAndroid & iOS, URP, UI Toolkit and all settings have been applied.\n\nHappy coding, Trio Games! 🎮";

            EditorUtility.DisplayDialog("Mobile Setup — Complete", message, "OK");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────
        private static void DrawRule()
        {
            Rect r = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(r, ColRule);
        }
    }
}
