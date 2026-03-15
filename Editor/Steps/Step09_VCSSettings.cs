using UnityEditor;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 09 — Version Control Settings
    ///
    /// Configures the project for Git-friendly serialization:
    ///   · Force Text serialization — all assets stored as human-readable YAML
    ///   · Visible Meta Files — .meta files tracked by Git
    ///
    /// These settings are the industry standard for Unity + Git workflows.
    /// </summary>
    public class Step09_VCSSettings : SetupStepBase
    {
        public Step09_VCSSettings()
        {
            Name        = "Version Control Settings";
            Description = "Enables Force Text serialization and Visible Meta Files for clean Git history.";
        }

        protected override void Run()
        {
            // Force all assets to serialize as text (YAML) instead of binary
            EditorSettings.serializationMode = SerializationMode.ForceText;

            // Make .meta files visible to source control (Git)
            EditorSettings.externalVersionControl = "Visible Meta Files";

            Succeed("Serialization: Force Text. Meta files: Visible. Git-ready ✓");
        }
    }
}
