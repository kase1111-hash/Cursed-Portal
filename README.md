# Cursed Portal -- Poe Parlor

An LLM-powered gothic horror game built on Unity. Chat with the spirits of Edgar Allan Poe characters -- the Raven, the Tell-Tale Heart Narrator, and Roderick Usher -- in a haunted parlor that grows darker and more oppressive with every conversation. When dread reaches its peak, a dimensional breach transports you to the Other Side for a final farewell.

## Prerequisites

- **Unity 2023.2 LTS** (URP template)
- **Ollama** with a local model (default: `llama3.2:3b`)
  - Install: https://ollama.com
  - Pull a model: `ollama pull llama3.2:3b`
  - Ollama runs automatically on `localhost:11434`

## Quick Start

1. Clone the repo and open in Unity Hub
2. Start Ollama (`ollama serve` if not already running)
3. Open `Assets/Scenes/PoeParlor.unity`
4. Use **CursedPortal > Setup Main Scene** menu to auto-generate scene objects
5. Press Play

### Editor Tools

| Menu Item | What It Does |
|-----------|-------------|
| CursedPortal > Setup Main Scene | Generates parlor with room, props, managers, player rig |
| CursedPortal > Create OtherDimension Scene | Generates finale scene with spirit core and epilogue UI |
| CursedPortal > Build Prefabs | Creates manager and prop prefabs |
| CursedPortal > Wire Scene Objects | Validates and wires scene references |
| CursedPortal > Validate Scene Setup | Reports missing managers, UI, or interactables |

### Debug Keys (Editor / Development Build)

| Key | Action |
|-----|--------|
| F1 | Toggle debug panel |
| F2 | Summon random spirit |
| F3 | Toggle chat UI |
| F4 | Skip to OtherDimension |
| F5 | Reset spook level |
| F12 | Toggle spook level debug overlay |

## LLM Backend Configuration

The game defaults to **Ollama** on `localhost:11434` with model `llama3.2:3b`. To change:

1. Select the **Managers** object in the scene
2. Find the **LLMManager** component
3. Change `Backend` (Ollama or LlamaCpp), `Endpoint`, and `Model` as needed

For **llama.cpp** instead of Ollama:
```
./server -m llama3.2-3b.gguf --host 0.0.0.0 --port 8080
```
Set Backend to `LlamaCpp` and Endpoint to `http://localhost:8080/completion`.

## How It Works

```
Player enters parlor
  -> Approach a prop (crystal ball, mirror, booth)
  -> Press E to interact -> spirit summoned via Ollama
  -> Chat in real time (streaming response)
  -> EmotionParser detects terror/unease/neutral
  -> EventManager escalates spook level (0-5)
  -> Fog thickens, audio builds, vignette tightens
  -> At level 5: DIMENSION BREACH
  -> PortalSequence fades to black, loads OtherDimension
  -> EpilogueNarrator generates farewell from spirit memories
  -> UIEpilogue typewriter display
  -> Press E to awaken -> game ends
```

Spirit memories persist across sessions in `Application.persistentDataPath/SpiritMemory/`.

## Project Structure

```
Assets/
  Scripts/
    AI/           LLMManager, LLMStreamManager, EpilogueNarrator,
                  EmotionParser, SpiritMemory
    AudioVFX/     AudioManager, VFXManager, PostFXController,
                  CameraShake, HeartbeatEffect, AmbienceController,
                  ScreenEffects, DimensionalLight, CandleFlicker,
                  PortalDistort, OrbitalMotion
    Core/         GameManager, EventManager, RitualLoop,
                  PortalSequence, FinaleManager, InteractionManager,
                  IInteractable, SingletonBase, PlayerSpawnMarker,
                  CursorManager
    Player/       FirstPersonController, CameraController, FootstepSystem
    Props/        InteractableSpirit, PropHighlight, CrystalBallProp,
                  MirrorProp, BoothProp, TableProp
    UI/           UIChat, UIEpilogue, DebugUI, SpookLevelDebugUI
    Editor/       SceneSetup, SceneWiringTool, PrefabBuilder,
                  OtherDimensionSetup
  StreamingAssets/
    SpiritProfiles/poe_spirits.json
    PoeStories/   raven.txt, tell-tale-heart.txt, usher.txt
  Scenes/         PoeParlor, OtherDimension
```

## License

MIT
