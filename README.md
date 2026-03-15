# Mobile Project Setup
**com.prasanna.mobilesetup** · v1.0.0 · by [Trio Games](https://github.com/Prasannagaya3)

A one-click Unity Editor package that configures any empty Unity project into a **production-ready mobile starter template** for Android & iOS.

---

## What It Does

| Step | Action |
|------|--------|
| 01 | Installs required UPM packages (URP, TextMeshPro, Input System, Timeline, Cinemachine, Mobile Notifications) |
| 02 | Creates URP Pipeline Asset and assigns it to Graphics & Quality settings |
| 03 | Configures Android Player Settings (IL2CPP, ARM64, Vulkan + OpenGLES3) |
| 04 | Configures iOS Player Settings (IL2CPP, ARM64, iOS 13+) |
| 05 | Applies global Player Settings (Linear color space, Both Input Systems, code stripping) |
| 06 | Creates standard folder structure under `Assets/` |
| 07 | Creates `Main.unity` scene with Camera, Directional Light and EventSystem |
| 08 | Creates UI Toolkit root files (`MainUI.uxml` + `MainUI.uss`) |
| 09 | Configures VCS-friendly settings (Force Text Serialization, Visible Meta Files) |
| 10 | Applies mobile build optimizations (Static/Dynamic Batching, Metal for iOS) |

---

## Installation

In Unity, open **Window → Package Manager → `+` → Add package from git URL** and paste:

```
https://github.com/Prasannagaya3/com.prasanna.mobilesetup.git
```

---

## Usage

After installation the **Mobile Setup Wizard** opens automatically on first use.

If it doesn't appear, navigate to: **Tools → Trio Games → Mobile Setup Wizard**

Click **▶ Run Full Setup** — that's it.

### Re-running Setup
The wizard tracks whether setup has run using `EditorPrefs`. Click **Reset Setup State** in the wizard footer to re-enable the auto-open and re-run all steps.

---

## Two-Pass Installation (Important)

Because Step 01 adds packages to `manifest.json` (which triggers a Unity domain reload), **URP-dependent steps (Step 02) require a second run** after Unity finishes installing packages.

**Recommended flow for a brand-new project:**
1. Install this package via Git URL
2. Click **▶ Run Full Setup** → Step 01 adds packages → Unity reloads
3. Click **▶ Run Full Setup** again → all steps including URP complete

---

## Customisation

All predefined values live in a single file:

```
Editor/Core/SetupConfig.cs
```

Edit that file to change company name, bundle IDs, min SDK versions, package list, folder structure, etc. You never need to touch anything else.

---

## Requirements

- Unity **2022.3 LTS** or newer
- Android Build Support module *(install once via Unity Hub)*
- iOS Build Support module *(install via Unity Hub — macOS only)*

---

## Folder Structure Created

```
Assets/
├── Core/
│   ├── Scripts/
│   ├── Managers/
│   ├── Services/
│   └── Utilities/
├── Art/
│   ├── Textures/
│   ├── Materials/
│   └── Fonts/
├── UI/
│   ├── UXML/
│   └── USS/
├── Scenes/
├── Prefabs/
├── Plugins/
└── Settings/
```

---

## License

MIT © 2026 Trio Games
