# Changelog

All notable changes to this package are documented here.
Format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).
Versioning follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.0] - 2026-03-15

### Added
- `MobileSetupWizard` — EditorWindow with 11-step automation, progress bar, and per-step status UI
- `SetupConfig.cs` — single source of truth for all predefined values
- `SetupStepBase.cs` — abstract base class with built-in error handling and status tracking
- **Step 01** `PackageInstaller` — writes required UPM packages directly to `manifest.json`
- **Step 02** `URPConfigurator` — creates URP Pipeline Asset via reflection, assigns to Graphics & Quality
- **Step 03** `AndroidConfigurator` — IL2CPP, ARM64, Vulkan + OpenGLES3, API 26+, switches active build target
- **Step 04** `iOSConfigurator` — IL2CPP, ARM64, iOS 13+ minimum, camera usage description
- **Step 05** `PlayerSettings` — Linear color space, New Input System only, Portrait + Landscape, code stripping
- **Step 06** `FolderStructure` — creates standard `Assets/` folder hierarchy with `.gitkeep` files
- **Step 07** `SceneCreator` — creates `Main.unity` with Camera, Directional Light and EventSystem
- **Step 08** `UIToolkitSetup` — generates `MainUI.uxml` and `MainUI.uss` starter templates
- **Step 09** `VCSSettings` — Force Text serialization, Visible Meta Files
- **Step 10** `BuildOptimizer` — Static/Dynamic batching via SerializedObject, ASTC textures, IL2CPP Release
- **Step 11** `Verify` — 46-point read-only verification report printed to Console
- `[InitializeOnLoadMethod]` auto-opens wizard on first install
- `EditorPrefs` setup-completion tracking with Reset button
- `🔍 Verify Settings Only` button — runs Step 11 independently at any time
- GitHub Actions CI workflow — validates package structure and file integrity on every push

### Fixed
- URP assembly reference replaced with reflection — no hard dependency on URP assembly
- Unity 2022 API corrections: `NET_Standard_2_1` → `NET_Standard`, batching via `SerializedObject`
- iOS Metal graphics API call removed — Metal is Unity's default on iOS, explicit call caused warnings on Windows
- iOS IL2CPP compiler configuration removed — configured in Xcode, not Unity
- `.meta` files added for all package assets — resolves UPM immutable folder warnings
- Input Handling corrected from Both (2) to New Input System only (1)
