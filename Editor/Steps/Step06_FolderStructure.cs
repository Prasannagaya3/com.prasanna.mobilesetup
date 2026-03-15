using UnityEditor;

namespace Prasanna.MobileSetup.Editor
{
    /// <summary>
    /// Step 06 — Folder Structure
    ///
    /// Creates the standard Assets/ folder hierarchy defined in SetupConfig.
    /// Existing folders are safely skipped (idempotent).
    /// A .gitkeep file is added to each folder so empty directories are
    /// tracked by Git.
    /// </summary>
    public class Step06_FolderStructure : SetupStepBase
    {
        public Step06_FolderStructure()
        {
            Name        = "Folder Structure";
            Description = "Creates standard Assets/ folders: Core, Art, UI, Scenes, Prefabs, Plugins, Settings.";
        }

        protected override void Run()
        {
            int created  = 0;
            int existing = 0;

            foreach (string folderPath in SetupConfig.FolderStructure)
            {
                if (AssetDatabase.IsValidFolder(folderPath))
                {
                    existing++;
                    continue;
                }

                // Split into parent and new folder name
                int lastSlash  = folderPath.LastIndexOf('/');
                string parent  = folderPath.Substring(0, lastSlash);
                string newName = folderPath.Substring(lastSlash + 1);

                // Ensure parent exists (handles nested paths correctly)
                EnsureParentExists(parent);

                AssetDatabase.CreateFolder(parent, newName);
                AddGitKeep(folderPath);
                created++;
            }

            AssetDatabase.Refresh();
            Succeed($"Created {created} folder(s). {existing} already existed.");
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private static void EnsureParentExists(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            if (path == "Assets") return;

            int lastSlash  = path.LastIndexOf('/');
            string parent  = path.Substring(0, lastSlash);
            string newName = path.Substring(lastSlash + 1);

            EnsureParentExists(parent);
            AssetDatabase.CreateFolder(parent, newName);
        }

        private static void AddGitKeep(string folderPath)
        {
            string fullPath = System.IO.Path.Combine(
                UnityEngine.Application.dataPath.Replace("/Assets", ""),
                folderPath,
                ".gitkeep");

            if (!System.IO.File.Exists(fullPath))
                System.IO.File.WriteAllText(fullPath, string.Empty);
        }
    }
}
