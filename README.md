# Missing Script Finder & Cleaner for Unity

A powerful Unity Editor extension to **detect and safely remove missing script references** across your entire project or selected assets. Designed to clean up corrupted GameObjects and prefabs, saving hours of tedious manual work.

---

## Overview

Every Unity developer has faced this:

> You open a scene or prefab and see `(Missing Script)` on multiple GameObjects.

Manually searching and fixing these broken references is time-consuming, error-prone, and frustrating, especially in large projects.

**Missing Script Finder & Cleaner** automates this process with:

- Comprehensive project-wide scanning
- Selective search for scenes, prefabs, or selected assets
- Safe removal workflow with undo support
- Clean, modern Editor window interface

---

## Problem Statement

### Common Issues
- `(Missing Script)` warnings due to deleted scripts or misconfigured prefabs
- Hard-to-find broken references in nested GameObjects
- Risk of accidentally removing valid components
- No native Unity solution for batch fixing

---

## Solution

A **dedicated Unity Editor window** that allows you to:

- Scan entire project or selected assets for missing scripts
- Visualize affected GameObjects with context
- Safely remove missing components with undo support
- Reduce project corruption and improve maintainability

---

## Key Features

### Project-Wide Scanning
- Scans all scenes, prefabs, and selected folders
- Detects missing MonoBehaviour or Script references
- Supports large projects efficiently

### Detailed Results List
- Lists GameObjects with missing scripts
- Shows path in hierarchy / asset path
- Indicates scene or prefab source

### Safe Cleanup
- One-click **Remove Component**
- Undoable via Unity’s Undo system
- Prevents accidental deletion of valid components

### Multi-Selection Support
- Select multiple GameObjects or assets from the list
- Batch remove missing scripts

### Search & Filter
- Quickly filter by scene, prefab, or hierarchy path
- Focus on problematic assets first

### User-Friendly Interface
- Modern dockable Editor window
- Clear labels and buttons
- Minimal learning curve

---

## Use Cases

### Game Developers
- Keep projects clean from broken scripts
- Prevent runtime errors due to missing MonoBehaviours
- Faster scene and prefab iteration

### Teams & Studios
- Standardize project health across team members
- Reduce merge conflicts caused by missing scripts
- Ensure prefab integrity

### Technical Artists / R&D
- Identify scripts accidentally removed during experimental workflows
- Cleanup project for testing or builds

---

## Technical Architecture

### Folder Structure
```

Editor/
├── MissingScriptFinderWindow.cs
├── MissingScriptScanner.cs
├── MissingScriptCleaner.cs
├── AssetUtility.cs
└── UI/
├── ResultsListView.cs
└── Toolbar.cs

```

### Core Components

#### Finder Window
- Custom `EditorWindow` with search, filter, and results
- Dockable and responsive UI

#### Project Scanner
- Scans scenes, prefabs, and selected folders
- Uses `GameObject.GetComponents<MonoBehaviour>()` to detect missing scripts
- Optimized for large hierarchies

#### Cleaner
- Safely removes missing components
- Supports single and batch removal
- Undo-aware with `Undo.RecordObject()`

#### Asset Utilities
- Handles prefab and scene path retrieval
- Provides consistent asset references for UI display

---

## ⚙️ Installation

### Manual Installation
1. Clone or download the repository
2. Copy the folder into your project:
```

Assets/Editor/MissingScriptCleaner/

```
3. Open Unity
4. Access the tool via:
```

Tools → Missing Script Finder & Cleaner

```

---

## Performance & Safety

- Editor-only execution, no runtime overhead
- Optimized search algorithms for large projects
- Undo system fully integrated
- Non-destructive workflow: components are removed safely

---

## Compatibility

- Unity 2020 LTS and above
- Built-in Render Pipeline
- URP / HDRP compatible
- Works alongside version control systems (Git, Perforce)

---

## 🤝 Contributing

Contributions are welcome:
- Performance improvements
- UI/UX enhancements
- Bug fixes and additional features

Fork the repo and submit a pull request with proper documentation.

---

## License

MIT License  
Free to use, modify, and distribute in both personal and commercial projects.

---

## Why This Tool Matters

This tool exists because **every Unity developer has cursed `(Missing Script)` at least once**.  
Its purpose:

> *Restore project integrity, save time, and prevent frustration.*

---

## Support & Feedback

Open an issue in the repository for any bugs, suggestions, or feature requests.  
Happy developing
