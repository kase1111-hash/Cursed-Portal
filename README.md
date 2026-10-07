# Cursed Portal -- Poe Parlor

An LLM-powered gothic horror game built on Unity. Chat with the spirits of Edgar Allan Poe characters -- the Raven, the Tell-Tale Heart Narrator, and Roderick Usher -- in a haunted parlor that grows darker and more oppressive with every conversation. When dread reaches its peak, a dimensional breach transports you to the Other Side for a final farewell.

## Prerequisites

- **Unity 2023.2.20f1** (Tech Stream; the exact version in `ProjectSettings/ProjectVersion.txt`). Packages: URP 16, uGUI 2.0 (which includes TextMesh Pro), Cinemachine, Timeline — resolved automatically from `Packages/manifest.json`.
- **Ollama** with a local model (default: `llama3.2:3b`)
  - Install: https://ollama.com
  - Pull a model: `ollama pull llama3.2:3b`
  - Ollama runs automatically on `localhost:11434`

## Quick Start

1. Clone the repo and open the folder in Unity Hub (Unity 2023.2.20f1).
2. Start Ollama (`ollama serve` if it isn't already running).
3. Optional but recommended: download the Poe story texts from Project Gutenberg into `Assets/StreamingAssets/PoeStories/` (the included files are placeholders with the links inside; while they are placeholders the spirits simply get no story excerpt). Only the first ~4000 characters after Gutenberg's header are sent to the model, and the Tell-Tale Heart link is a multi-story volume, so keep just that story's text in `tell-tale-heart.txt`.
4. Run **CursedPortal > Setup Main Scene**. It configures URP, imports the TextMesh Pro essentials if needed (re-run the menu item once that import finishes), then generates and saves `Assets/Scenes/CursedPortal.unity` and adds it to Build Settings.
5. Run **CursedPortal > Create OtherDimension Scene** to generate and save the finale (`Assets/Scenes/OtherDimension.unity`).
6. Open `Assets/Scenes/CursedPortal.unity` and press Play. Walk with WASD, look with the mouse, press **E** at the crystal ball (the Raven), the mirror (the Narrator) or the booth (Usher), type, and press Enter. **Esc** closes and reopens the chat.

Re-running a generator replaces that scene (you are asked first); generated materials and volume profiles in `Assets/Materials/Generated` and `Assets/Settings` are kept, so tweaks to them survive.

### Editor Tools

| Menu Item | What It Does |
|-----------|-------------|
| CursedPortal > Configure URP Render Pipeline | Creates `Assets/Settings/CursedPortal_URP.asset` and makes it the render pipeline (the scene generators do this automatically) |
| CursedPortal > Setup Main Scene | Generates and saves the parlor: managers, player, room, props, VFX, post-processing, chat/HUD/debug UI |
| CursedPortal > Create OtherDimension Scene | Generates and saves the finale: platform, spirit core, camera, FinaleManager, epilogue UI |
| CursedPortal > Build Prefabs | Creates manager and prop prefabs in `Assets/Prefabs` (optional; the scene generators don't need them) |
| CursedPortal > Wire Scene Objects | Assigns PostFXController's Volume and reports missing managers in the open scene |
| CursedPortal > Validate Scene Setup | Reports missing managers, UI references, EventSystem, render pipeline and Build Settings entries |
| CursedPortal > Build > Windows (64-bit) / Linux (64-bit) | Generates any missing scene and builds the player to `Builds/` |

Command-line build (from the project folder):
```
"<Unity Hub>/Editor/2023.2.20f1/Editor/Unity" -batchmode -nographics -quit -logFile - \
    -projectPath . -executeMethod CursedPortalBuild.BuildWindows
```
On a fresh clone, open the project in the editor once and run CursedPortal > Setup Main Scene (which imports the TextMesh Pro essentials) before building from the command line; otherwise the first batch build exits with an error asking you to re-run it.

### Keys

| Key | Action |
|-----|--------|
| WASD / Mouse | Move / look (Shift to sprint) |
| E | Interact with the prop you're looking at |
| Esc | Toggle the chat |
| Enter | Send a message |

Debug keys (Editor / Development Build only):

| Key | Action |
|-----|--------|
| F1 | Toggle debug panel (with a spook-level slider) |
| F2 | Summon random spirit |
| F3 | Toggle chat UI |
| F4 | Skip to OtherDimension |
| F5 | Reset spook level |
| F12 | Toggle spook level debug overlay |
| Alt | Toggle cursor lock |

## LLM Backend Configuration

The game defaults to **Ollama** on `localhost:11434` with model `llama3.2:3b`. To change:

1. Select the **Managers** object in the scene
2. Find the **LLMManager** component
3. Change **Backend** (Ollama or LlamaCpp), **Llm Endpoint** and **Ollama Model** as needed (temperature and max tokens for streamed replies are on the **LLMStreamManager** component)

For **llama.cpp** instead of Ollama:
```
llama-server -m llama3.2-3b.gguf --host 127.0.0.1 --port 8080
```
Set Backend to `LlamaCpp` and Endpoint to `http://localhost:8080/completion`.

If the server is down or the model isn't pulled, the chat shows the server's error (for example *model "llama3.2:3b" not found*) instead of a reply.

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

## Current Status

The scripts compile for Unity 2023.2.20f1 and the editor tools generate both scenes fully wired. What the repository does not include:

- **Poe story texts**: `StreamingAssets/PoeStories/` holds placeholders — download the full texts from Project Gutenberg (links are in each file).
- **Audio** (whispers, heartbeat, ambience, SFX): the AudioManager, HeartbeatEffect and prop sound slots are empty, so the game is silent until clips are assigned. Visuals use primitives and generated materials.

The specialised props (`CrystalBallProp`, `MirrorProp`, `BoothProp`, `TableProp`) are optional richer alternatives to `InteractableSpirit`; put only one interactable component on a prop.

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
                  SpookTriggerZone
    Player/       FirstPersonController, CameraController,
                  FootstepSystem, CursorManager
    Props/        InteractableSpirit, PropHighlight, PropAnimator,
                  PropAmbientSound, CrystalBallProp,
                  MirrorProp, BoothProp, TableProp
    UI/           UIChat, UIEpilogue, DebugUI, SpookLevelDebugUI
    Editor/       SceneSetup, OtherDimensionSetup, SetupURP,
                  SceneWiringTool, PrefabBuilder, CursedPortalBuild,
                  CursedPortalEditorUtil
  StreamingAssets/
    SpiritProfiles/poe_spirits.json
    PoeStories/   raven.txt, tell-tale-heart.txt, usher.txt
  Scenes/         CursedPortal, OtherDimension  (generated via editor tools)
```

## License

MIT
