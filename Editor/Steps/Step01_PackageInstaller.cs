using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 01 — Package Installer
    ///
    /// Writes missing UPM packages directly into Packages/manifest.json.
    /// Unity's Package Manager will detect the change and install them
    /// on the next domain reload (triggered by AssetDatabase.Refresh).
    ///
    /// Because package installation is async (Unity reloads the domain),
    /// URP-dependent steps will show a warning on the first run.
    /// Simply click Run Full Setup a second time once packages are installed.
    /// </summary>
    public class Step01_PackageInstaller : SetupStepBase
    {
        public Step01_PackageInstaller()
        {
            Name        = "Package Installer";
            Description = "Adds URP, TextMeshPro, Input System, Timeline, Cinemachine and " +
                          "Mobile Notifications to Packages/manifest.json.";
        }

        protected override void Run()
        {
            string manifestPath = Path.GetFullPath("Packages/manifest.json");

            if (!File.Exists(manifestPath))
                throw new FileNotFoundException(
                    "Packages/manifest.json not found. Is this a valid Unity project?");

            string original = File.ReadAllText(manifestPath);
            string updated  = InjectMissingPackages(original, out int addedCount, out var addedList);

            if (addedCount == 0)
            {
                Succeed("All required packages are already present in manifest.json.");
                return;
            }

            File.WriteAllText(manifestPath, updated);

            // Tell UPM to pick up the change
            AssetDatabase.Refresh();

            Warn($"Added {addedCount} package(s): {string.Join(", ", addedList)}.\n" +
                 "Unity is installing them now — re-run setup after the domain reload completes.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static string InjectMissingPackages(
            string manifest,
            out int addedCount,
            out List<string> addedNames)
        {
            addedCount = 0;
            addedNames = new List<string>();

            var insertionBlock = new StringBuilder();

            foreach (var kvp in SetupConfig.RequiredPackages)
            {
                // Check both with and without quotes to be safe
                if (!manifest.Contains($"\"{kvp.Key}\""))
                {
                    insertionBlock.AppendLine($"    \"{kvp.Key}\": \"{kvp.Value}\",");
                    addedNames.Add(kvp.Key);
                    addedCount++;
                }
            }

            if (addedCount == 0)
                return manifest;

            // Find the opening brace of the "dependencies" block and insert after it
            const string Marker = "\"dependencies\": {";
            int idx = manifest.IndexOf(Marker);

            if (idx < 0)
                throw new System.Exception(
                    "Could not locate 'dependencies' block in manifest.json. " +
                    "The file may be malformed.");

            int insertAt = idx + Marker.Length;
            return manifest.Insert(insertAt, "\n" + insertionBlock);
        }
    }
}
