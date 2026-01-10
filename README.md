🧩 Master Architecture Overview
Project Name: Cursed Portal – Poe Parlor
Engine Target: Unity 2023.2 LTS (URP)
Duration: 4–8 hours build time (solo developer)
LLM Integration: LLMUnity + local Ollama backend (Llama 3.x)
Mood: Gothic horror, escalating dread, interactive storytelling
Modularity Goal: Each system can be generated and tested independently.

## What Is Cursed Portal?

**Cursed Portal** is an **LLM-powered gothic horror game** that combines **AI-driven narrative** with **interactive storytelling**. Built on Unity with **local LLM integration** via Ollama, this **atmospheric horror experience** channels the spirit of Edgar Allan Poe through **natural language AI characters** that respond dynamically to player interactions.

This project explores **human-AI collaboration** in gaming, where **procedural horror** and **AI character dialogue** create emergent, personalized supernatural encounters. The game features **sentiment-aware environmental effects**, **AI personality persistence**, and **LLM memory systems** that remember past conversations across sessions.

### Key Features
- **Natural language gaming** – Chat with AI spirits using conversational prompts
- **Procedural atmospheric horror** – Dynamic fog, lighting, and audio driven by AI sentiment analysis
- **Local LLM privacy** – All AI processing runs locally via Ollama (Llama 3.x)
- **AI memory persistence** – Spirits remember your past conversations
- **Escalating dread system** – Progressive horror intensity based on player engagement
- **Edgar Allan Poe themed** – Authentic spirit personas from classic gothic literature

🧱 SYSTEM MAP (for AI codegen)
Module ID
System
Description
Dependencies
M1
Core Setup
Project setup, lighting, scene composition
None
M2
Interaction Framework
Handles player input, raycasting, and prop triggers
M1
M3
LLM Manager
Manages Poe spirit interactions (text + optional voice)
M1
M4
Event Manager (Spook Logic)
Handles escalation of horror effects
M1
M5
Audio + VFX System
Ambient loops, whispers, and particle control
M4
M6
Post-Processing Stack
Fog, vignette, color grading, lens effects
M1
M7
UI Chat System
Chatbox UI, message history, input field
M3
M8
Interactables
Crystal Ball, Mirror, Booth, Table
M2, M3, M4
M9
Narrative Profiles
JSON story data + character templates
M3
M10
Testing & Debug Tools
Spook level slider, quick summon console
All above


🧩 M1 — Core Setup
Goal
Create the base environment (room, camera, lighting, URP setup) with minimal manual tuning.
Submodules
Scene:
Scene name: CursedPortal.unity
Default objects:
MainCamera (CinemachinePOV)
EventSystem
DirectionalLight (low intensity)
RoomRoot (walls, floor, props as children)
Room scale: 10x10x5 meters.
Player spawn at (0, 1.8, -8) facing table.
Lighting Setup:
URP Global Volume:
Fog density start = 0.01
Color Grading: desaturated purple-blue
Bloom = off (keeps mood dark)
Vignette = 0.1 base
Point lights per prop (candle flicker via scriptable animation curve).
Camera Controller:
First-person, no jump.
Mouse look, WASD movement.
Field of view: 70°.

🧩 M2 — Interaction Framework
Goal
Generic player interaction manager for all props.
Components
IInteractable interface:
public interface IInteractable {
    void OnInteract();
}


InteractionManager.cs:
Raycasts from camera center.
Highlights object outlines when targeted.
Press E to trigger OnInteract().
Visual feedback (URP Outline via Shader Graph optional).
Dependencies
M1 (camera and input)
Used by M8 (Interactables)

🧩 M3 — LLM Manager (Poe Spirits)
Goal
Central AI system for invoking Poe-like responses from local Ollama LLM via LLMUnity.
Components
LLMManager.cs
using LLMUnity;
using UnityEngine;
using System.IO;

public class LLMManager : MonoBehaviour {
    public static LLMManager Instance;
    public LLM llm;
    public string activeSpirit;
    public string activeStory;
    
    void Awake() => Instance = this;

    public void SummonSpirit(string story, string spiritName) {
        string storyText = File.ReadAllText(Application.streamingAssetsPath + "/PoeStories/" + story);
        string prompt = $"You are {spiritName}, a cursed mimic spirit from Poe's '{story}'. Speak eerily and evoke hallucinations. Respond to the user as if trapped between realms.";
        llm.Chat(prompt, OnResponse);
    }

    void OnResponse(string text) {
        UIChat.Instance.AppendResponse(text);
    }
}


Dependencies:
M7 (UIChat for display)
M9 (Narrative Profiles for story selection)

🧩 M4 — Event Manager (Spook Logic)
Goal
Manages fog density, whisper volume, particle ghosts, and screen effects based on cumulative “spook level.”
Core Variable
int spookLevel (0–5)
Core Script: EventManager.cs
Singleton
Methods:
IncrementSpook()
ApplyEffectsForLevel(int level)
Effects Table
Level
Effect
0
Baseline ambience, faint whispers
1
Orb glow, slight vignette
2
Fog thickens, camera shake
3
Loud whispers, phantom silhouettes
4
Mirror glitch shader distortion
5
Full fog, red tint, “DIMENSION BREACH” overlay, camera shake 2x

🧩 M5 — Audio + VFX System
Goal
Control ambience, whispers, creaks, and supernatural VFX based on spook level updates from EventManager.
Submodules
AudioManager.cs

 using UnityEngine;
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioSource ambientSource, sfxSource;
    public AudioClip[] whispers; // indexed by level
    void Awake() => Instance = this;

    public void PlayWhispers(int level) {
        if (level < whispers.Length) {
            ambientSource.clip = whispers[level];
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }
    public void PlaySFX(AudioClip clip, float pitch = 1f) {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip);
    }
}


Ambient bus: whisper loops (levels 0–3)


SFX bus: heartbeats, mirror cracks, portal breach burst


VFXManager.cs


Controls particle systems and screen post effects:


FogParticles (hemisphere shape)


GhostPrefab (semi-transparent skinned mesh)


PortalBurst at level 5


Exposed methods:
 EnableGhosts(), TriggerPortal(), SetFogDensity(float)


Dependencies
M4 (EventManager calls audio/VFX triggers)


M6 (Post-processing values modified here)



🧩 M6 — Post-Processing Stack
Goal
Provide runtime-tunable URP Volume profiles for horror intensity.
Setup
Global Volume object with components:


Vignette (intensity 0 → 0.6)


Color Adjustments (saturation -50 → -100)


Lens Distortion (enabled at level 4+)


Chromatic Aberration (enabled at level 3+)


Script: PostFXController.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostFXController : MonoBehaviour {
    public Volume volume;
    Vignette vignette; ColorAdjustments color;
    LensDistortion lens; ChromaticAberration chroma;

    void Start() => volume.profile.TryGet(out vignette)
        && volume.profile.TryGet(out color)
        && volume.profile.TryGet(out lens)
        && volume.profile.TryGet(out chroma);

    public void SetLevel(int level) {
        vignette.intensity.value = Mathf.Lerp(0.1f, 0.6f, level/5f);
        color.saturation.value   = -50f - (level*10f);
        chroma.active = level >= 3;
        lens.active   = level >= 4;
    }
}


🧩 M7 — UI Chat System
Goal
Allow player to chat with Poe spirits (text and optional voice output).
Components
UI Prefab: Canvas → ScrollView → TextMeshProUGUI log, InputField, Send Button.


UIChat.cs


using UnityEngine;
using TMPro;

public class UIChat : MonoBehaviour {
    public static UIChat Instance;
    public TMP_InputField input;
    public TMP_Text chatLog;

    void Awake() => Instance = this;

    public void SendMessage() {
        string msg = input.text;
        AppendUser(msg);
        input.text = "";
        LLMManager.Instance.llm.Chat(msg, AppendResponse);
    }

    public void AppendUser(string msg)
        => chatLog.text += $"\n<color=#66f>User:</color> {msg}";
    public void AppendResponse(string msg)
        => chatLog.text += $"\n<color=#9f6>Poe Spirit:</color> {msg}";
}

Toggle visibility with Esc key.


Optional: TTS playback per response via AudioManager.


Dependencies
M3 (LLMManager)



🧩 M8 — Interactables
Goal
Reusable prefabs with unique spirit bindings and spook increments.
Prop
Spirit
Trigger
Effect
Crystal Ball
Raven
Look + E
Level +1, “Nevermore” voice clip
Booth
Roderick Usher
Enter trigger zone
Fog 0.05, twin voice dialog
Mirror
Tell-Tale Narrator
Look at mirror
Glitch shader, heartbeat SFX
Table
Random Spirit
Sit down
Ambient whisper increase

Prefab Setup
Each prefab adds an InteractableSpirit script:
using UnityEngine;
public class InteractableSpirit : MonoBehaviour, IInteractable {
    public string spiritName;
    public string storyFile;
    public int spookIncrement = 1;
    public void OnInteract() {
        LLMManager.Instance.SummonSpirit(storyFile, spiritName);
        EventManager.Instance.IncrementSpook(spookIncrement);
    }
}


🧩 M9 — Narrative Profiles (JSON Data)
Goal
Provide external data defining spirits and story contexts.
File: Assets/StreamingAssets/SpiritProfiles/poe_spirits.json
{
  "Raven": {
    "story": "raven.txt",
    "prompt": "You are the Raven, forever croaking 'Nevermore'..."
  },
  "Narrator": {
    "story": "tell-tale-heart.txt",
    "prompt": "You are the guilty narrator, haunted by the beating heart..."
  },
  "Usher": {
    "story": "usher.txt",
    "prompt": "You are Roderick Usher, decaying with your ancestral home..."
  }
}

LLMManager parses these to build context on summon.


Supports user-supplied texts for custom spirits.



🧩 M10 — Testing & Debug Tools
Goal
Rapid iteration and QA for spook levels and LLM responses.
SpookLevelDebugUI.cs


using UnityEngine;
using UnityEngine.UI;
public class SpookLevelDebugUI : MonoBehaviour {
    public Slider slider;
    void Start() => slider.onValueChanged.AddListener(v => 
        EventManager.Instance.ApplyEffectsForLevel(Mathf.RoundToInt(v)));
}

LLMConsole.cs


Simple command window to send raw prompts to LLMManager for testing.


DebugHotkeys.cs


F1 = Reset spook.


F2 = Random spirit summon.


F3 = Toggle chat UI.



🧩 Prefab Hierarchy Summary
CursedPortal_Scene
 ├── PlayerRig (Camera, InteractionManager)
 ├── RoomRoot
 │    ├── Table
 │    ├── CrystalBall
 │    ├── Mirror
 │    └── Booth
 ├── Managers
 │    ├── LLMManager
 │    ├── EventManager
 │    ├── AudioManager
 │    ├── VFXManager
 │    └── PostFXController
 ├── UI
 │    ├── ChatCanvas
 │    └── DebugCanvas
 └── Environment (FogParticles, GlobalVolume)


🧱 Integration Order (for AI agents)
Generate M1–M2 (core scene + interaction).


Implement M3 (LLM Manager) and connect to UIChat.


Add M4–M5 (Event & Audio/VFX).


Wire M6 (PostFX) to EventManager.


Add M8 Interactables + JSON profiles (M9).


Attach debug tools (M10).


Test spook ramp and iterate on fog/audio balance.


⚙️ PART 5 — AI BUILD + PREFAB SETUP
Overview
This final part makes the Cursed Portal demo fully reproducible by code-generation pipelines or Unity command-line builds.
Contents:
Prefab Creation Scripts


URP Volume Presets (asset templates)


YAML Build Manifest for AI/CI


Optional Automation Hooks (Editor Tools)



🧩 1. Prefab Creation Scripts
Purpose: Allow AI or batch commands to assemble props + managers automatically from base meshes and scripts.
File: Assets/Editor/PrefabBuilder.cs
using UnityEditor;
using UnityEngine;

public static class PrefabBuilder {
    [MenuItem("CursedPortal/Build Prefabs")]
    public static void BuildAll() {
        string root = "Assets/Prefabs/";

        BuildManagerPrefab(root + "Managers/");
        BuildPropPrefab("CrystalBall", root + "Props/");
        BuildPropPrefab("Mirror", root + "Props/");
        BuildPropPrefab("Booth", root + "Props/");
        BuildPropPrefab("Table", root + "Props/");

        Debug.Log("✅ All Cursed Portal prefabs generated.");
    }

    static void BuildManagerPrefab(string path) {
        GameObject go = new GameObject("Managers");
        go.AddComponent<LLMManager>();
        go.AddComponent<EventManager>();
        go.AddComponent<AudioManager>();
        go.AddComponent<VFXManager>();
        go.AddComponent<PostFXController>();
        PrefabUtility.SaveAsPrefabAsset(go, path + "Managers.prefab");
        GameObject.DestroyImmediate(go);
    }

    static void BuildPropPrefab(string name, string path) {
        GameObject prop = GameObject.CreatePrimitive(PrimitiveType.Cube);
        prop.name = name;
        var interact = prop.AddComponent<InteractableSpirit>();
        interact.spiritName = name;
        interact.storyFile = name.ToLower() + ".txt";
        interact.spookIncrement = 1;
        PrefabUtility.SaveAsPrefabAsset(prop, path + name + ".prefab");
        GameObject.DestroyImmediate(prop);
    }
}

Notes:
Creates default cube-based prefabs (replace meshes later).


Runs via Unity menu: CursedPortal → Build Prefabs


Ideal for AI agents generating Unity content headlessly.



🧩 2. URP Volume Preset Assets
Preset profiles stored in Assets/PostFX/Profiles/:
a. BaseMood.asset
Effect
Value
Vignette Intensity
0.1
Color Saturation
-50
Fog Density
0.01

b. MidHaunt.asset
Effect
Value
Vignette Intensity
0.3
Color Saturation
-75
Fog Density
0.05
Chromatic Aberration
Enabled

c. FullBreach.asset
Effect
Value
Vignette Intensity
0.6
Color Saturation
-100
Fog Density
0.2
Lens Distortion
-0.3 curvature

Each profile can be loaded dynamically by PostFXController depending on spook level.

🧩 3. Build Manifest (YAML Format)
File: cursed_portal_build.yaml
 Used by Unity automation or LLM code orchestration frameworks.
project_name: "Cursed Portal – Poe Parlor"
engine_version: "Unity 2023.2.0f1"
render_pipeline: "URP"

dependencies:
  - "com.unity.textmeshpro"
  - "com.unity.render-pipelines.universal"
  - "https://github.com/undreamai/LLMUnity.git"

scenes:
  - "Assets/Scenes/CursedPortal.unity"

streaming_assets:
  - "Assets/StreamingAssets/PoeStories/"
  - "Assets/StreamingAssets/SpiritProfiles/poe_spirits.json"

prefabs:
  managers: "Assets/Prefabs/Managers/Managers.prefab"
  props:
    - "Assets/Prefabs/Props/CrystalBall.prefab"
    - "Assets/Prefabs/Props/Mirror.prefab"
    - "Assets/Prefabs/Props/Booth.prefab"
    - "Assets/Prefabs/Props/Table.prefab"

build_targets:
  - "StandaloneWindows64"
  - "WebGL"

automation:
  steps:
    - name: "Setup URP"
      command: "unity -batchmode -nographics -projectPath . -executeMethod SetupURP.Run"
    - name: "Generate Prefabs"
      command: "unity -batchmode -nographics -projectPath . -executeMethod PrefabBuilder.BuildAll"
    - name: "Build Player"
      command: "unity -batchmode -nographics -buildWindows64Player ./Builds/CursedPortal.exe"

This manifest allows a CI pipeline or LLM-powered Unity wrapper (like LLMUnity.BuildAgent) to clone, configure, prefab, and build the demo entirely from spec.

🧩 4. Optional Editor Utilities
a. Scene Auto-Wire Tool
File: Assets/Editor/SceneWiringTool.cs
using UnityEditor;
using UnityEngine;

public static class SceneWiringTool {
    [MenuItem("CursedPortal/Wire Scene Objects")]
    public static void WireScene() {
        var eventMgr = Object.FindObjectOfType<EventManager>();
        var fx = Object.FindObjectOfType<PostFXController>();
        if (eventMgr && fx) eventMgr.GetComponent<EventManager>().fxController = fx;
        Debug.Log("🔗 Scene objects linked successfully.");
    }
}

b. Poe Text Importer
Automatically downloads and formats Project Gutenberg Poe works into .txt files for the LLM to read.


Uses standard System.Net.Http in Editor mode (skipped for builds).



🧩 5. Complete Folder Tree (Final)
Assets/
 ├── Scenes/
 │    └── CursedPortal.unity
 ├── Scripts/
 │    ├── Core/ (EventManager.cs, InteractionManager.cs)
 │    ├── AI/ (LLMManager.cs, LLMConsole.cs)
 │    ├── AudioVFX/ (AudioManager.cs, VFXManager.cs)
 │    ├── UI/ (UIChat.cs, DebugUI.cs)
 │    ├── Props/ (InteractableSpirit.cs)
 │    └── Editor/ (PrefabBuilder.cs, SceneWiringTool.cs)
 ├── Prefabs/
 │    ├── Managers/
 │    └── Props/
 ├── PostFX/
 │    └── Profiles/
 ├── StreamingAssets/
 │    ├── PoeStories/
 │    └── SpiritProfiles/
 └── cursed_portal_build.yaml


🧱 Final AI Integration Summary
Stage
Agent Task
Input
Output
1
Bootstrap
YAML manifest
Unity project
2
Generate
M1–M10 specs
C# scripts + prefabs
3
Assemble
PrefabBuilder.cs
Prefabs in /Prefabs
4
Wire
SceneWiringTool.cs
Linked runtime scene
5
Run
Build pipeline
Playable gothic demo
6
Extend
Custom user TXT
Dynamic spirit generation

🌀 PART 6 — Procedural Horror Expansion Pack
Objective
Augment the base prototype with emergent LLM-powered behavior, asynchronous audio streaming, and procedural visual effects that intensify dynamically from user interaction and AI sentiment.

🧩 M11 — Asynchronous LLM Streaming + Hallucination Layer
Goal
Allow the Poe Spirit to “bleed” partial text responses into the world as whispers, light flickers, or on-screen distortions while generating.
Core Additions
// Assets/Scripts/AI/LLMStreamManager.cs
using UnityEngine;
using LLMUnity;
using System.Collections;

public class LLMStreamManager : MonoBehaviour {
    public static LLMStreamManager Instance;
    public LLM llm;
    void Awake() => Instance = this;

    public IEnumerator StreamSpiritSpeech(string prompt) {
        var stream = llm.ChatStream(prompt);
        foreach (var chunk in stream) {
            UIChat.Instance.AppendPartial(chunk);
            AudioManager.Instance.StreamWhisper(chunk);
            yield return null;
        }
        EventManager.Instance.IncrementSpook(1);
    }
}

In LLMManager
Replace .Chat() calls with StartCoroutine(LLMStreamManager.Instance.StreamSpiritSpeech(prompt)).

🧩 M12 — Voice Synthesis Pipeline
Goal
Transform LLM output into eerie diegetic speech using local TTS (Bark / Piper) or external services (ElevenLabs optional).
Implementation
// Assets/Scripts/AudioVFX/VoiceSynth.cs
using UnityEngine;
using System.IO;

public class VoiceSynth : MonoBehaviour {
    public AudioSource voiceSource;
    public void Speak(string text) {
        string wavPath = LocalTTS.GenerateWav(text); // custom wrapper
        var clip = WavUtility.ToAudioClip(File.ReadAllBytes(wavPath));
        voiceSource.PlayOneShot(clip);
    }
}

Behavior
Every 3rd response triggers voice playback.


Mix slightly left/right channel imbalance (+/- 0.1) to simulate presence movement.


Pitch modulation ± 0.2 based on spook level.



🧩 M13 — Adaptive Portal Distortion Shader
Goal
Represent the “dimensional rift” visually, reacting to both spook level and LLM emotion tags.
Shader Graph Properties
Property
Description
Range
_DistortionStrength
Sinusoidal UV warping
0 → 2
_HueShift
Color cycling for spectral shimmer
-0.2 → 0.3
_PulseRate
Audio-driven intensity
0 → 5 Hz

C# Controller
// Assets/Scripts/AudioVFX/PortalDistort.cs
using UnityEngine;

public class PortalDistort : MonoBehaviour {
    Material mat; float baseStrength;
    void Start() { mat = GetComponent<Renderer>().material; baseStrength = mat.GetFloat("_DistortionStrength"); }
    public void SetEmotion(string emotion) {
        float t = emotion switch {
            "terror" => 2f, "unease" => 1f, _ => 0.5f
        };
        mat.SetFloat("_DistortionStrength", Mathf.Lerp(baseStrength, t, 0.5f));
    }
}

EventManager hooks into this each time spook level increases.

🧩 M14 — Emotion Parser + Sentiment Hooks
Goal
Analyze LLM output tone to drive environmental responses (fog, light color, audio filters).
Implementation
// Assets/Scripts/AI/EmotionParser.cs
using System.Text.RegularExpressions;

public static class EmotionParser {
    public static string Detect(string text) {
        if (Regex.IsMatch(text, "(terror|fear|dread)", RegexOptions.IgnoreCase)) return "terror";
        if (Regex.IsMatch(text, "(sad|lament|mourn)", RegexOptions.IgnoreCase)) return "unease";
        return "neutral";
    }
}

Usage:
 EventManager.Instance.ReactToEmotion(EmotionParser.Detect(chunk));
EventManager Extension
“terror” → increase fog + bass boost


“unease” → color grade cooler


“neutral” → fade whispers



🧩 M15 — Procedural Ambient Composer
Goal
Layer multiple evolving ambience stems (drone, clock tick, heartbeat) procedurally.
Core Logic
3 looping AudioSources


Volume & low-pass filter adjusted by spook level and emotion


Occasional randomized “dropout” moments (1 s silence → impact)



🧩 M16 — LLM Memory & Personality Persistence
Goal
Persist spirit states across sessions, allowing each spirit to “remember” past chats.
Implementation
// Assets/Scripts/AI/SpiritMemory.cs
using UnityEngine;
using System.IO;

[System.Serializable]
public class SpiritMemory {
    public string spiritName;
    public string[] lastPrompts;
    public string[] lastResponses;

    public static void Save(SpiritMemory mem) =>
        File.WriteAllText(Path(mem.spiritName), JsonUtility.ToJson(mem));
    public static SpiritMemory Load(string name) {
        string path = Path(name);
        return File.Exists(path) ? JsonUtility.FromJson<SpiritMemory>(File.ReadAllText(path)) : new SpiritMemory{ spiritName = name };
    }
    static string Path(string n) => Application.persistentDataPath + $"/{n}_memory.json";
}

LLMManager loads this before chat and appends previous context to new prompt.

🧩 M17 — Ritual Event Loop (System Coordinator)
Goal
Global update loop orchestrating emotion, VFX, and sound per frame.
// Assets/Scripts/Core/RitualLoop.cs
using UnityEngine;

public class RitualLoop : MonoBehaviour {
    void Update() {
        var level = EventManager.Instance.spookLevel;
        PostFXController.Instance.SetLevel(level);
        VFXManager.Instance.UpdateFog(level);
        AudioManager.Instance.UpdateAmbience(level);
    }
}

Optionally run on a fixed timestep (0.5 s) for performance.

⚙️ Pipeline Changes
Addition
Description
Connected Modules
LLMStreamManager
Real-time LLM streaming
M3, M4
VoiceSynth
TTS whisper output
M5
PortalDistort
Shader driven rift visual
M13
EmotionParser
Text sentiment bridge
M14
SpiritMemory
Persistent context store
M16
RitualLoop
Unified runtime controller
All above


🧱 Scene Hierarchy Update
CursedPortal_Scene
 ├── PlayerRig
 ├── RoomRoot
 ├── Managers
 │    ├── LLMManager
 │    ├── LLMStreamManager
 │    ├── VoiceSynth
 │    ├── EventManager
 │    ├── RitualLoop
 ├── Environment
 │    ├── PortalPlane (PortalDistort)
 │    ├── GlobalVolume
 │    └── FogParticles


🧩 Optional Gameplay Loop Enhancement
Ritual Completion: After spookLevel >= 5, LLM prompts “Do you wish to enter the rift?” — player reply “Yes” → scene fade → load OtherDimension.unity.


Dynamic Credits: Generate final message using LLM with memory context as epilogue.



🧩 Automation Manifest Extension
Append to cursed_portal_build.yaml:
modules:
  - "Assets/Scripts/AI/LLMStreamManager.cs"
  - "Assets/Scripts/AI/EmotionParser.cs"
  - "Assets/Scripts/AI/SpiritMemory.cs"
  - "Assets/Scripts/AudioVFX/VoiceSynth.cs"
  - "Assets/Scripts/AudioVFX/PortalDistort.cs"
  - "Assets/Scripts/Core/RitualLoop.cs"


🪄 Future Expansions
Phase
Feature
Description
II
Procedural Room Rearrangement
Physics-based prop movement via LLM cues
III
Multiplayer Séance Mode
Shared LLM session via WebSocket
IV
Real-World Mic Seance
Player voice input summons spirits through speech recognition
V
Adaptive Narrative Engine
Poe style stories assembled from LLM memory logs

🌌 PART 7 — Other Dimension Finale Scene
Objective
Deliver a playable, cinematic conclusion triggered when spookLevel ≥ 5.
 The player steps through the mirror or crystal orb, crosses a dissolving threshold, and experiences a brief AI-driven epilogue.

🧩 M18 — Scene Setup: OtherDimension.unity
Visual Mood
Floating stone platform suspended in purple-red mist.


Rotating skybox gradient (deep indigo → crimson).


Ambient particle field with slow orbital motion.


Required Objects
Object
Purpose
MainCamera
same controls as previous scene
Platform
8 × 8 m plane with cracked marble texture
PortalExit
swirling vortex mesh, entry spawn
SpiritCore
glowing orb representing Poe Spirit
FXVolume
URP Volume controlling saturation/pulse
EpilogueUI
fade-in text canvas
Managers
LLMManager (loaded persistent) + RitualLoop


🧩 M19 — Portal Transition Sequence
Trigger
EventManager.spookLevel == 5
 → PortalSequence.StartTransition()
// Assets/Scripts/Core/PortalSequence.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalSequence : MonoBehaviour {
    public CanvasGroup fade;
    public AudioClip breachSFX;
    public float duration = 6f;

    public void StartTransition() => StartCoroutine(Breach());

    IEnumerator Breach() {
        AudioManager.Instance.PlaySFX(breachSFX);
        for (float t=0; t<duration; t+=Time.deltaTime) {
            fade.alpha = t/duration;
            RenderSettings.fogDensity = Mathf.Lerp(0.2f, 0.6f, t/duration);
            yield return null;
        }
        SceneManager.LoadScene("OtherDimension");
    }
}


🧩 M20 — Ethereal Environment Shader
Shader Graph Properties
Property
Description
Range
_FlowTex
world-space distortion noise
0–1
_EmissionPulse
time-driven glow
0–3
_SpecterColor
base hue
HDR

Assign to skybox + platform edges for an otherworldly undulation.

🧩 M21 — Epilogue Interaction
Flow
Player spawns on Platform.


SpiritCore floats ahead (2 m up).


LLM generates closing narration using memory file.


// Assets/Scripts/AI/EpilogueNarrator.cs
using UnityEngine;

public class EpilogueNarrator : MonoBehaviour {
    void Start() {
        var mem = SpiritMemory.Load("Raven");
        string prompt = $"Compose a poetic farewell from the void to {System.Environment.UserName}, referencing {mem.lastResponses.Length} past confessions.";
        LLMManager.Instance.llm.Chat(prompt, OnText);
    }
    void OnText(string text) => UIEpilogue.Instance.Display(text);
}

Epilogue UI
Typewriter effect reveals final prose.


Background heartbeat fades to silence.


Option “Return / Exit” ends the build.



🧩 M22 — Dynamic Audio Scape
Layered reverse-reverb drones reacting to sentence pacing.


Use AudioReverbZone radius = 10 m; decay = 6 s.


Pitch automation driven by EmotionParser output from last LLM line.



🧩 M23 — Procedural Lighting Cycle
// Assets/Scripts/AudioVFX/DimensionalLight.cs
using UnityEngine;
public class DimensionalLight : MonoBehaviour {
    Light l; float baseInt;
    void Start() { l = GetComponent<Light>(); baseInt = l.intensity; }
    void Update() {
        l.intensity = baseInt + Mathf.Sin(Time.time*0.3f)*0.5f;
        l.color = Color.Lerp(Color.magenta, Color.cyan, (Mathf.Sin(Time.time)+1)/2);
    }
}

Attach to SpiritCore light source for a living pulse.

🧩 M24 — Player Conclusion Logic
After epilogue text finishes, show “[E] to Awaken.”


On input:


Fade out to black.


Save final memory summary via SpiritMemory.Save().


Quit application or reload main menu.



🧩 M25 — Automation Manifest Update
Append to cursed_portal_build.yaml:
scenes:
  - "Assets/Scenes/CursedPortal.unity"
  - "Assets/Scenes/OtherDimension.unity"

modules:
  - "Assets/Scripts/Core/PortalSequence.cs"
  - "Assets/Scripts/AI/EpilogueNarrator.cs"
  - "Assets/Scripts/AudioVFX/DimensionalLight.cs"


🌑 Epilogue Experience Flow
Threshold: mirror ruptures → fade to black.


Arrival: player awakens on floating platform.


Spirit Address: LLM recites farewell using prior memory.


Silence: sound and fog dissipate; final quote appears:


 “We are but echoes, bound by the words we weave.”



Exit: screen fades; build terminates or restarts loop.



🔮 Optional Phase II Hooks
Real-time Specter Morphs: spawn Poe avatars via mesh blend shapes driven by LLM keywords.


AR Mirror Mode: feed camera to mirror plane for mixed-reality dread.


Procedural Poem Generator: compile LLM responses into stanzas for export to .txt.


🧠 FINAL BUILD DIRECTIVES FOR THE AI CODER
1. Code Structure & Integrity
All modules (M1–M25) are self-contained and must compile without circular dependencies.


If a class needs cross-referencing (e.g., EventManager ↔ AudioManager), use singleton access patterns only — no scene hard-links.


Maintain one class per file, following the provided namespaces:

 Assets/Scripts/Core/
Assets/Scripts/AI/
Assets/Scripts/UI/
Assets/Scripts/AudioVFX/
Assets/Scripts/Props/


Default every manager prefab to DontDestroyOnLoad(this) so persistence across the portal transition works correctly.



2. Scene & Asset Linkage
The CursedPortal.unity scene is the entry point (player spawn, main LLM session).


The OtherDimension.unity scene is the finale (epilogue logic only).


Maintain consistent tag conventions:

 Player
Interactable
Portal
SpiritCore


Both scenes share the same lighting and post-process profiles for smooth fade transitions.



3. LLM Behavior and Context Management
Always prepend prompts with context strings, e.g.:

 "You are the spirit of Roderick Usher... respond in poetic dread..."


Respect SpiritMemory persistence — the AI should:


Load memory before each chat session.


Append the latest message pair after every response.


Save memory on OnApplicationQuit().


Stream responses via LLMStreamManager where possible; only fall back to LLM.Chat() if streaming unsupported.



4. Audio & Visual Performance Notes
Use URP Forward Rendering, MSAA 2x, Fog enabled.


Cap global particle count < 1000 for WebGL compatibility.


Keep AudioSource count ≤ 10 simultaneously.


Whisper clips should be mono 44.1kHz, max length 15s.



5. Spook System (EventManager) Rules
spookLevel always clamps between 0–5.


Trigger incremental transitions only once per level (debounce logic).


Each increment must call:


AudioManager.PlayWhispers(level)


PostFXController.SetLevel(level)


VFXManager.UpdateFog(level)



6. UI and Input Coherence
Global Input Map:


Key
Action
E
Interact
ESC
Toggle Chat UI
F1
Debug Panel
F2
Random Spirit
F3
Skip Scene




Ensure UIChat scroll view auto-scrolls with new messages.


Hide cursor during exploration; show it when Chat UI active.



7. Build Automation Standards
Honor all commands in cursed_portal_build.yaml.


Validate prefab generation (PrefabBuilder.cs) before compile.


Build targets:


Windows64 → primary test


WebGL → LLM disconnected fallback (script simulation)


Post-build verify:


Assets/StreamingAssets/PoeStories/ contains at least 3 .txt files.


No missing script references.


Scene loads in < 5s on cold start.



8. Narrative Consistency
Spirits must reflect tone from their source text:


The Raven → obsession, refrain, despair


Tell-Tale Heart → guilt, heartbeat motifs


Usher → decay, confinement, bloodline rot


Keep dialogue terse, rhythmic, and occasionally poetic — e.g.:


 “You listen still — to the heart you swore was silent.”



In the Other Dimension, all spirits merge into one “Collective Voice”; use stored memory snippets to compose a unified farewell.



9. Testing & Debug
Implement a debug overlay showing:

 Spook: 3/5
Active Spirit: Raven
Emotion: terror
Fog: 0.05


Add optional “auto-demo” mode: cycles interactions every 15s.



10. AI Agent Conduct
Never overwrite JSON or YAML templates without confirmation.


Never hardcode asset paths; always use Application.streamingAssetsPath or Resources.Load.


Generate code in deterministic order (M1 → M25).


Annotate each file top with:

 // Auto-generated by CursedPortal AI Spec v1.0
// Source: Module M##



✅ FINAL SUCCESS CONDITIONS
The build is considered complete and verified when:
Player can spawn, interact with 3+ props, and trigger full spook escalation.


Portal breach transports to OtherDimension successfully.


Poe spirit delivers epilogue monologue.


App fades out and exits gracefully.


All YAML manifest steps execute with no missing dependencies.



⚜️ OPTIONAL FUTURE VERSIONS
Version
Focus
New Features
v1.1
Audio & Dialogue
Multichannel spatial whispers
v1.2
AR/VR Mode
XR camera rig integration
v2.0
Multiplayer Séance
Shared LLM session per player
v3.0
Procedural Architecture
LLM-generated gothic rooms


🔚 Closing Message for the AI Developer
“Treat every script as a ritual component — modular, self-aware, and written with poetic precision.
 The system must not merely function; it must whisper, breathe, and remember.”
Proceed to code generation in sequential order (M1 → M25), then finalize with build automation per YAML manifest.


Build: PC/WebGL (local LLM needs PC).
Test: Drop Poe TXT, chat – watch spook ramp.
Expand: User TXT upload for custom books.

---

## 🔗 Part of the Ecosystem

Cursed Portal shares infrastructure and design philosophy with these related projects:

### 🔗 NatLangChain Ecosystem
Natural language blockchain and intent-native protocols for human-readable systems.

| Repository | Description |
|------------|-------------|
| [NatLangChain](https://github.com/kase1111-hash/NatLangChain) | Prose-first, intent-native blockchain protocol for natural language smart contracts |
| [IntentLog](https://github.com/kase1111-hash/IntentLog) | Git for human reasoning – version control for decisions and design rationale |
| [RRA-Module](https://github.com/kase1111-hash/RRA-Module) | Revenant Repo Agent – converts abandoned repos into autonomous licensing agents |
| [mediator-node](https://github.com/kase1111-hash/mediator-node) | LLM-powered mediation layer for matching, negotiation, and deal closure |
| [ILR-module](https://github.com/kase1111-hash/ILR-module) | IP & Licensing Reconciliation for dispute resolution |
| [Finite-Intent-Executor](https://github.com/kase1111-hash/Finite-Intent-Executor) | Posthumous execution of predefined intent via Solidity smart contracts |

### 🤖 Agent-OS Ecosystem
Natural language native operating systems and cognitive infrastructure for AI agents.

| Repository | Description |
|------------|-------------|
| [Agent-OS](https://github.com/kase1111-hash/Agent-OS) | Natural language operating system (NLOS) for AI agent coordination |
| [synth-mind](https://github.com/kase1111-hash/synth-mind) | NLOS-based agent with psychological modules for empathy and continuity |
| [boundary-daemon-](https://github.com/kase1111-hash/boundary-daemon-) | Trust enforcement layer defining cognition boundaries for Agent OS |
| [memory-vault](https://github.com/kase1111-hash/memory-vault) | Sovereign, offline-capable storage for cognitive artifacts |
| [value-ledger](https://github.com/kase1111-hash/value-ledger) | Economic accounting layer for cognitive work (ideas, effort, novelty) |
| [learning-contracts](https://github.com/kase1111-hash/learning-contracts) | Safety protocols for AI learning and data governance |

### 🛡️ Security Infrastructure
| Repository | Description |
|------------|-------------|
| [Boundary-SIEM](https://github.com/kase1111-hash/Boundary-SIEM) | Security Information and Event Management for AI systems |

### 🎮 Game Development
| Repository | Description |
|------------|-------------|
| [Shredsquatch](https://github.com/kase1111-hash/Shredsquatch) | 3D first-person snowboarding infinite runner (SkiFree spiritual successor) |
| [Midnight-pulse](https://github.com/kase1111-hash/Midnight-pulse) | Procedurally generated synthwave night driving experience |
| [Long-Home](https://github.com/kase1111-hash/Long-Home) | Atmospheric narrative indie game built with Godot |

---

### 🌐 Cross-Ecosystem Themes

This project contributes to broader explorations in:
- **Authenticity Economy** – Preserving human cognitive labor and intent in AI-augmented systems
- **Natural Language Programming** – Prose-first development and language-native architecture
- **Digital Sovereignty** – Self-hosted AI, owned infrastructure, and private AI systems
- **Human-AI Collaboration** – Building experiences where AI enhances rather than replaces human creativity
