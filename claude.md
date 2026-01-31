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
│   ├── Player/      # FirstPersonController, CameraController
│   └── Editor/      # PrefabBuilder, SceneWiringTool
├── StreamingAssets/
│   ├── PoeStories/      # raven.txt, usher.txt, tell-tale-heart.txt
│   └── SpiritProfiles/  # poe_spirits.json
└── cursed_portal_build.yaml  # Build configuration manifest
```

## Quick Reference

### Scenes
- **CursedPortal.unity** - Main game scene with parlor and interactable props
- **OtherDimension.unity** - Finale scene with epilogue narration

### Configuration Files
- `Assets/cursed_portal_build.yaml` - Build manifest, dependencies, LLM config
- `Assets/StreamingAssets/SpiritProfiles/poe_spirits.json` - Spirit definitions and prompts

### LLM Configuration
- **Provider:** llama.cpp via Ollama
- **Endpoint:** localhost:8080
- **Model:** llama3.2-3b (recommended)
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
./server -m llama3.2-3b.gguf --host 0.0.0.0 --port 8080
```

### Debug Keys (In-Game)
| Key | Function |
|-----|----------|
| Esc | Toggle chat UI |
| E | Interact with object |
| F1 | Reset spook level to 0 |
| F2 | Summon random spirit |
| F3 | Skip to OtherDimension |

## Code Conventions

### Manager Class Template
```csharp
public class NewManager : MonoBehaviour {
    public static NewManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
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
3. HTTP POST to llama.cpp endpoint
4. EmotionParser analyzes response
5. UIChat displays response, SpiritMemory persists exchange

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
| LLM not responding | Check Ollama is running on localhost:8080 |
| Missing script references | Run CursedPortal > Wire Scene Objects |
| Chat log overflow | Auto-truncates at 5000 chars |
| WebGL LLM issues | Use external proxy or fallback to script simulation |
