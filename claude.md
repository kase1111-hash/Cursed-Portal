# Claude.md - Cursed Portal Development Guide

## Project Overview

**Cursed Portal – Poe Parlor** is a Unity 2023.2 LTS AI-powered gothic horror game that combines interactive storytelling with local LLM integration. Players chat with AI-driven spirits inspired by Edgar Allan Poe's works in a haunted parlor setting.

### Core Features
- Natural language interaction with AI spirits (Raven, Narrator, Usher)
- Procedural atmospheric horror driven by AI dialogue sentiment
- Local LLM privacy via Ollama backend (Llama 3.x)
- Spook level escalation system (0-5) triggering progressive horror effects
- Two-scene narrative arc: Main parlor → Finale dimension

## Architecture

### Key Patterns
- **Singleton Pattern** - All manager classes for global access
- **Observer Pattern** - EventManager events for system communication
- **IInteractable Interface** - Polymorphic interaction system for props

### Core Manager Classes

| Manager | Path | Purpose |
|---------|------|---------|
| EventManager | `Assets/Scripts/Core/EventManager.cs` | Spook escalation (0-5), central horror coordinator |
| InteractionManager | `Assets/Scripts/Core/InteractionManager.cs` | Raycasts, detects IInteractable objects |
| LLMManager | `Assets/Scripts/AI/LLMManager.cs` | Spirit summoning, LLM API calls, memory loading |
| AudioManager | `Assets/Scripts/AudioVFX/AudioManager.cs` | Whispers, SFX, ambient audio |
| VFXManager | `Assets/Scripts/AudioVFX/VFXManager.cs` | Fog, ghost particles, portal effects |
| PostFXController | `Assets/Scripts/AudioVFX/PostFXController.cs` | URP Volume control (vignette, fog, color) |
| UIChat | `Assets/Scripts/UI/UIChat.cs` | Chat log display, input handling |
| PortalSequence | `Assets/Scripts/Core/PortalSequence.cs` | Portal breach, scene transition |

### Directory Structure

```
Assets/
├── Scripts/
│   ├── Core/        # EventManager, InteractionManager, PortalSequence
│   ├── AI/          # LLMManager, SpiritMemory, EmotionParser
│   ├── UI/          # UIChat, DebugUI, UIEpilogue
│   ├── AudioVFX/    # AudioManager, VFXManager, PostFXController
│   ├── Props/       # CrystalBallProp, MirrorProp, etc.
│   ├── Player/      # FirstPersonController, CameraController, CursorManager, FootstepSystem
│   └── Editor/      # PrefabBuilder, SceneWiringTool, SceneSetup, OtherDimensionSetup
├── StreamingAssets/
│   ├── PoeStories/      # raven.txt, usher.txt, tell-tale-heart.txt
│   └── SpiritProfiles/  # poe_spirits.json
└── cursed_portal_build.yaml  # Build configuration manifest
```

## Quick Reference

### Scenes
- **CursedPortal.unity** - Main game scene with parlor and interactable props (generate via CursedPortal > Setup Main Scene)
- **OtherDimension.unity** - Finale scene with epilogue narration (generate via CursedPortal > Create OtherDimension Scene)

Note: Scene files are not checked into the repo. Use the editor tools above to generate them.

### Configuration Files
- `Assets/cursed_portal_build.yaml` - Build manifest, dependencies, LLM config
- `Assets/StreamingAssets/SpiritProfiles/poe_spirits.json` - Spirit definitions and prompts

### LLM Configuration
- **Default Provider:** Ollama
- **Default Endpoint:** `http://localhost:11434/api/generate`
- **Model:** `llama3.2:3b` (recommended)
- **Alternative Provider:** llama.cpp on `http://localhost:8080/completion`
- **Streaming:** Enabled by default
- **Temperature:** 0.8
- **Max tokens:** 256

## Development Commands

### Build
```bash
# Editor build via menu
CursedPortal > Build Prefabs          # Auto-generate prefab templates
CursedPortal > Wire Scene Objects     # Auto-link managers to scene

# Command line build
unity -batchmode -nographics -projectPath . \
  -executeMethod PrefabBuilder.BuildAll \
  -buildWindows64Player ./Builds/CursedPortal.exe
```

### Run LLM Server
```bash
# Option A: Ollama (default backend)
ollama serve                    # Starts on localhost:11434
ollama pull llama3.2:3b         # Download model (first time only)

# Option B: llama.cpp (alternative backend)
./server -m llama3.2-3b.gguf --host 0.0.0.0 --port 8080
# Then set LLMManager backend to LlamaCpp in the Inspector
```

### Debug Keys (Editor / Development Build)
| Key | Function |
|-----|----------|
| Esc | Toggle chat UI |
| E | Interact with object |
| F1 | Toggle debug panel |
| F2 | Summon random spirit |
| F3 | Toggle chat UI |
| F4 | Skip to OtherDimension |
| F5 | Reset spook level to 0 |
| F12 | Toggle spook level debug overlay |

## Code Conventions

### Manager Class Template
All singleton managers inherit from `SingletonBase<T>` (defined in `Assets/Scripts/Core/SingletonBase.cs`):
```csharp
// For managers that persist across scenes:
public class NewManager : SingletonBase<NewManager> {
    protected override void Awake() {
        base.Awake();
        // Custom initialization here
    }
}

// For managers scoped to a single scene:
public class NewSceneManager : SceneSingletonBase<NewSceneManager> {
    protected override void Awake() {
        base.Awake();
    }
}
```

### IInteractable Interface
```csharp
public interface IInteractable {
    void OnInteract();
    void OnHighlightEnter();
    void OnHighlightExit();
}
```

### Error Handling
- Always use null checks before method calls
- Use try-catch for file I/O and JSON parsing
- Provide graceful fallbacks for LLM failures
- Use LogWarning/LogError for debug visibility

## Key Systems

### Spook Level Effects (0-5)
| Level | Effects |
|-------|---------|
| 0 | Baseline ambience, faint whispers |
| 1 | Orb glow, slight vignette increase |
| 2 | Fog thickens, camera shake |
| 3 | Loud whispers, ghost phantoms |
| 4 | Mirror glitch, lens distortion |
| 5 | FULL BREACH: red tint, max fog, portal sequence |

### LLM Chat Flow
1. UIChat.SendMessage() receives player input
2. LLMManager.SummonSpirit() loads profile + story context + memory
3. HTTP POST to Ollama endpoint (`localhost:11434/api/generate`) or llama.cpp (`localhost:8080/completion`)
4. LLMStreamManager streams response chunks to UIChat
5. EmotionParser analyzes each chunk for sentiment
6. RitualLoop reacts to detected emotions, EventManager escalates spook level
7. SpiritMemory persists the exchange

### Scene Transition
When spook level reaches 5, EventManager triggers OnDimensionBreach event, which starts PortalSequence to transition to OtherDimension.unity.

## Performance Targets
- **Target FPS:** 60
- **Max particles:** 1000
- **Max audio sources:** 10
- **MSAA:** 2x
- **Scene load time:** < 5 seconds

## Dependencies
- Unity 2023.2 LTS
- Universal Render Pipeline (URP)
- TextMeshPro (Package Manager)
- Cinemachine (Package Manager)
- LLMUnity (Git: https://github.com/undreamai/LLMUnity.git)

## Common Tasks

### Adding a New Spirit
1. Add story text to `Assets/StreamingAssets/PoeStories/`
2. Add spirit definition to `poe_spirits.json` with story reference and system prompt
3. Spirit will be available via LLMManager.SummonSpirit()

### Adding a New Interactable Prop
1. Create script in `Assets/Scripts/Props/` implementing IInteractable
2. Add prefab to scene with collider and script
3. InteractionManager will automatically detect it

### Modifying Horror Effects
1. Spook level changes trigger EventManager.OnSpookLevelChanged event
2. Subscribe managers (Audio, VFX, PostFX) respond to level changes
3. Adjust intensity curves in respective manager inspectors

## Troubleshooting

| Issue | Solution |
|-------|----------|
| LLM not responding | Check Ollama is running on localhost:11434 (or llama.cpp on localhost:8080 if using that backend) |
| Missing script references | Run CursedPortal > Wire Scene Objects |
| Chat log overflow | Auto-truncates at 5000 chars |
| WebGL LLM issues | Use external proxy or fallback to script simulation |
