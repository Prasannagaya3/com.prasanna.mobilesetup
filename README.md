# Mobile Project Setup
**com.prasanna.mobilesetup** · v1.0.0 · by [Trio Games](https://github.com/Prasannagaya3)

A one-click Unity Editor package that configures any **empty Unity project** into a
**production-ready mobile starter template** for Android & iOS — automatically.

No more manual setup. No more missed settings. Install the package, click one button, done.

---

## What It Automates

| Step | What Happens |
|------|-------------|
| 01 · Packages | Installs URP, TextMeshPro, Input System, Cinemachine, Timeline, Mobile Notifications |
| 02 · URP | Creates URP Pipeline Asset, assigns to Graphics & all Quality levels |
| 03 · Android | IL2CPP, ARM64, Vulkan + OpenGLES3, API 26+, switches active platform to Android |
| 04 · iOS | IL2CPP, ARM64, Metal, iOS 13+ minimum |
| 05 · Player Settings | Linear color space, New Input System only, Portrait + Landscape, code stripping |
| 06 · Folder Structure | Creates standard `Assets/` hierarchy |
| 07 · Scene | Creates `Main.unity` with Camera, Directional Light, EventSystem |
| 08 · UI Toolkit | Creates `MainUI.uxml` + `MainUI.uss` starter templates |
| 09 · VCS | Force Text serialization, Visible Meta Files (Git-friendly) |
| 10 · Build | Static/Dynamic batching, ASTC textures, IL2CPP Release |
| 11 · Verify | Reads back every setting and reports 46-point pass/fail to Console |

---

## Requirements

Before using this package, make sure your machine has the following.

### 1 · Unity Version
- **Unity 2022.3 LTS** or newer (required)
- Download from [Unity Hub](https://unity.com/download)

### 2 · Unity Hub Modules
Install these once via **Unity Hub → Installs → your version → Add Modules**:

| Module | Platform | Required on |
|--------|----------|------------|
| Android Build Support | Android | Windows + Mac |
| Android SDK & NDK Tools | Android | Windows + Mac |
| OpenJDK | Android | Windows + Mac |
| iOS Build Support | iOS | **Mac only** |

> ⚠️ **Windows users:** iOS Build Support is not available on Windows.
> iOS Player Settings will still be configured, but the Metal graphics API setting
> will be skipped (applied automatically on Mac with the module installed).

### 3 · Git
- Git must be installed and accessible from the command line
- Download from [git-scm.com](https://git-scm.com/)
- Verify with: `git --version`

### 4 · A Fresh Unity Project
- Create a **new empty project** in Unity Hub
- Template: **3D** or **3D (URP)** — either works
- Do **not** use 2D, Mobile, or any other template
- Unity version: 2022.3 LTS or newer

---

## Installation

### Step 1 — Open Package Manager
In your Unity project go to:
**Window → Package Manager**

### Step 2 — Add via Git URL
Click the **`+`** button (top-left) → **"Add package from git URL…"**

Paste this URL:
```
https://github.com/Prasannagaya3/com.prasanna.mobilesetup.git#v1.0.0
```
Click **Add** and wait for Unity to finish downloading.

---

## Usage

### Step 3 — Run the Wizard
After installation, the **Mobile Setup Wizard** opens automatically.

If it doesn't appear, go to: **Tools → Trio Games → Mobile Setup Wizard**

### Step 4 — First Run
Click **▶ Run Full Setup**.

Step 01 will add packages to `manifest.json`. Unity will then **reload automatically**
to install them. This is normal — wait for the progress bar to finish.

### Step 5 — Second Run (Required for URP)
After Unity finishes reloading, open the wizard again:
**Tools → Trio Games → Mobile Setup Wizard**

Click **▶ Run Full Setup** a second time.

This completes the URP setup and all remaining steps.

> **Why two runs?**
> Package installation triggers a Unity domain reload which interrupts execution.
> Step 01 installs packages on the first run. Step 02 onwards complete on the second run.

### Step 6 — Verify
Click **🔍 Verify Settings Only** in the wizard.

Unity's Console will show a full 46-point report:
```
✅ [PASS]  Android: IL2CPP
✅ [PASS]  Android: ARM64 only
✅ [PASS]  URP assigned in Graphics Settings
✅ [PASS]  Input Handling: New Input System
...
RESULT: 46/46 checks passed
```

### Step 7 — Remove the Package (Optional)
Once setup is complete, the package has done its job.
You can safely remove it via **Package Manager → Remove** — all settings, folders,
scenes and assets it created will remain in your project permanently.

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
│   ├── UXML/         ← MainUI.uxml
│   └── USS/          ← MainUI.uss
├── Scenes/           ← Main.unity
├── Prefabs/
├── Plugins/
└── Settings/         ← URP_PipelineAsset.asset
```

---

## Packages Installed

| Package | Version | Purpose |
|---------|---------|---------|
| `com.unity.textmeshpro` | 3.0.6 | Text rendering |
| `com.unity.inputsystem` | 1.7.0 | New Input System |
| `com.unity.mobile.notifications` | 2.3.2 | Push notifications |
| `com.unity.render-pipelines.universal` | 14.0.9 | URP rendering |
| `com.unity.timeline` | 1.7.6 | Animation timelines |
| `com.unity.cinemachine` | 2.9.7 | Camera system |

---

## Customisation

All predefined values live in a single file — **this is the only file you ever need to edit**:

```
Editor/Core/SetupConfig.cs
```

Things you can change there:
- Company name, Bundle IDs
- Minimum Android API level
- Minimum iOS version
- Package list and versions
- Folder structure
- URP asset paths
- Scene path

---

## Does Removing the Package Revert My Settings?

**No.** Removing the package only removes the package files.
All settings, folders, scenes, and assets the package created remain in your project permanently.
The package is a one-time setup tool — remove it freely once done.

---

## Troubleshooting

**"Could not clone… Make sure v1.0.0 is a valid branch name"**
The tag is missing from GitHub. Run:
```bash
git tag v1.0.0
git push origin v1.0.0
```

**URP step shows ⚠️ Warning on first run**
Expected. URP package is still installing. Wait for domain reload, then run setup again.

**iOS Metal warning on Windows**
Expected. iOS Build Support is not available on Windows. The warning is harmless —
iOS settings are still configured; Metal API is applied on Mac with the module installed.

**Setup wizard doesn't open automatically**
Go to **Tools → Trio Games → Mobile Setup Wizard** manually.
If the menu doesn't exist, check the Console for compile errors.

**Want to re-run setup on the same project**
Click **Reset Setup State** at the bottom of the wizard, then click **▶ Run Full Setup**.

---

## License
MIT © 2026 Trio Games
