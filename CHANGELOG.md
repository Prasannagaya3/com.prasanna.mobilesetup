# Changelog

All notable changes to this package are documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-03-15

### Added
- `MobileSetupWizard` — EditorWindow with 10-step automation and progress UI
- `SetupConfig.cs` — single source of truth for all predefined values
- `SetupStepBase.cs` — abstract base class for all setup steps
- **Step 01** `PackageInstaller` — writes required packages to `manifest.json`
- **Step 02** `URPConfigurator` — creates URP Pipeline Asset and assigns to Graphics/Quality
- **Step 03** `AndroidConfigurator` — IL2CPP, ARM64, Vulkan, Android 8.0+
- **Step 04** `iOSConfigurator` — IL2CPP, ARM64, iOS 13+, Metal
- **Step 05** `PlayerSettings` — Linear color space, Both input systems, code stripping
- **Step 06** `FolderStructure` — creates standard `Assets/` folder hierarchy
- **Step 07** `SceneCreator` — creates `Main.unity` with Camera, Light, EventSystem
- **Step 08** `UIToolkitSetup` — generates `MainUI.uxml` and `MainUI.uss` templates
- **Step 09** `VCSSettings` — Force Text serialization, Visible Meta Files
- **Step 10** `BuildOptimizer` — Static/Dynamic Batching, Metal for iOS, stripping
- `[InitializeOnLoadMethod]` auto-open on first install
- `EditorPrefs` setup-completion tracking
- GitHub Actions CI workflow for package validation
