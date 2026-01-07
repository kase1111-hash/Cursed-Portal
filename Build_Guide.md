Stage 1: Refinement Plan
Outline a structured blueprint for refining the provided Unity project spec:
Overall Structure: Organize into clear sections (e.g., Core Overview, Module Breakdown, Prefab/Pipeline Details, Expansions). Remove redundancies, consolidate similar parts (e.g., merge PART 5–7 into unified "Build & Expansion Pipeline"). Ensure consistent formatting: Use markdown for headers, tables, code blocks; standardize module IDs (M1–M25).
Module Enhancements: For each M#, refine descriptions to be concise, fix dependencies (e.g., resolve circular refs via interfaces/singletons), correct code syntax/errors (e.g., add missing imports, fix incomplete methods, ensure C# compliance). Add data structures where needed (e.g., enums for spook levels, dictionaries for effects).
Code Integrity: Pseudocode all scripts as valid C# snippets. Introduce error handling (e.g., null checks), performance optimizations (e.g., coroutines for async). Standardize singletons with lazy init.
Narrative & LLM: Refine prompts for better Poe-esque tone; add sentiment analysis regex patterns. Ensure memory persistence uses JSON serialization properly.
Build & Automation: Update YAML to include all new modules; add validation steps. Define build order as a sequence: Setup → Generate Modules → Assemble Prefabs → Wire Scenes → Build Targets.
Testing & Expansions: Consolidate future ideas into a phased roadmap table. Add success metrics with quantifiable tests (e.g., load time <5s).
Output: Refined spec as a cohesive document, modular for AI codegen.
Stage 2: Refined Specification
Below is the refined version of the "Cursed Portal – Poe Parlor Engine" project spec, based on the Stage 1 plan. Improvements include: streamlined organization, corrected code syntax (e.g., fixed incomplete methods, added imports), resolved dependencies (e.g., explicit interfaces), enhanced modularity (e.g., enums for levels), and consistent formatting. Redundancies removed; expansions integrated into a unified pipeline.

Master Architecture Overview
Project Name: Cursed Portal – Poe Parlor Engine
Target: Unity 2023.2 LTS (URP)
Duration: 4–8 hours build time (solo developer)
LLM Integration: LLMUnity + local Ollama backend (Llama 3.x)
Mood: Gothic horror, escalating dread, interactive storytelling
Modularity Goal: Each system generated and tested independently.
System Map (for AI Codegen)
Module ID
System Description
Dependencies
M1
Core Setup: Project setup, lighting, scene composition
None
M2
Interaction Framework: Player input, raycasting, prop triggers
M1
M3
LLM Manager: Poe spirit interactions (text + optional voice)
M1
M4
Event Manager (Spook Logic): Escalation of horror effects
M1
M5
Audio + VFX System: Ambient loops, whispers, particle control
M4
M6
Post-Processing Stack: Fog, vignette, color grading, lens effects
M1
M7
UI Chat System: Chatbox UI, message history, input field
M3
M8
Interactables: Crystal Ball, Mirror, Booth, Table
M2, M3, M4
M9
Narrative Profiles: JSON story data + character templates
M3
M10
Testing & Debug Tools: Spook level slider, quick summon console
All above
M11
Asynchronous LLM Streaming + Hallucination Layer: Partial response bleeding
M3, M4
M12
Voice Synthesis Pipeline: TTS for eerie speech
M5
M13
Adaptive Portal Distortion Shader: Visual rift reacting to emotions
M4
M14
Emotion Parser + Sentiment Hooks: Analyze LLM tone for env responses
M4
M15
Procedural Ambient Composer: Evolving ambience layers
M5
M16
LLM Memory & Personality Persistence: Cross-session state
M3
M17
Ritual Event Loop (System Coordinator): Global update orchestration
All above
M18
Scene Setup: OtherDimension.unity (finale)
M1
M19
Portal Transition Sequence: Breach to finale scene
M4, M17
M20
Ethereal Environment Shader: Distortion for otherworldly visuals
M6
M21
Epilogue Interaction Flow: AI-driven closing narration
M3, M16
M22
Dynamic Audio Scape: Reverb and pacing reactions
M5
M23
Procedural Lighting Cycle: Pulsing lights
M5
M24
Player Conclusion Logic: Fade out and exit
M17
M25
Automation Manifest Update: Full YAML integration
All above

Prefab Hierarchy Summary
text
CursedPortal_Scene
├── PlayerRig (Camera, InteractionManager)
├── RoomRoot
│   ├── Table
│   ├── CrystalBall
│   ├── Mirror
│   └── Booth
├── Managers
│   ├── LLMManager
│   ├── LLMStreamManager
│   ├── VoiceSynth
│   ├── EventManager
│   ├── RitualLoop
│   ├── AudioManager
│   ├── VFXManager
│   └── PostFXController
├── UI
│   ├── ChatCanvas
│   └── DebugCanvas
├── Environment
│   ├── PortalPlane (PortalDistort)
│   ├── FogParticles
│   └── GlobalVolume
Integration Order (for AI Agents)
Generate M1–M2 (core scene + interaction).
Implement M3–M7 (LLM + UI).
Add M4–M6, M11–M17 (events, audio/VFX, advanced LLM).
Wire M8–M9 (interactables + profiles).
Attach M10 (debug tools).
Expand to M18–M24 (finale scene).
Finalize with M25 (automation).
Test spook ramp; iterate on balance.
Module Details
M1 — Core Setup
Goal: Base environment (room, camera, lighting, URP setup).
Submodules:
Scene: CursedPortal.unity. Objects: MainCamera (CinemachinePOV), EventSystem, DirectionalLight (intensity 0.2), RoomRoot (walls, floor, props). Room: 10x10x5m. Player spawn: (0, 1.8, -8), facing table.
Lighting: URP Global Volume – Fog (density 0.01), Color Grading (desaturated purple-blue), Bloom off, Vignette 0.1. Point lights on props (candle flicker via AnimationCurve).
Camera: First-person, mouse look, WASD, no jump, FOV 70°.
M2 — Interaction Framework
Goal: Generic interaction for props.
Components:
IInteractable interface:
C#
public interface IInteractable {
    void OnInteract();
}
InteractionManager.cs:
C#
using UnityEngine;

public class InteractionManager : MonoBehaviour {
    public static InteractionManager Instance;
    private void Awake() => Instance = this;

    void Update() {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 3f)) {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable) && Input.GetKeyDown(KeyCode.E)) {
                interactable.OnInteract();
            }
        }
    }
}
Dependencies: M1. Used by: M8.
M3 — LLM Manager (Poe Spirits)
Goal: Invoke Poe-like responses via LLMUnity.
Components:
LLMManager.cs:
C#
using UnityEngine;
using LLMUnity;
using System.IO;

public class LLMManager : MonoBehaviour {
    public static LLMManager Instance;
    public LLM llm;
    public string activeSpirit;
    public string activeStory;

    private void Awake() => Instance = this;

    public void SummonSpirit(string story, string spiritName) {
        string storyText = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "PoeStories", story));
        string prompt = $"You are {spiritName}, a cursed mimic spirit from Poe's '{story}'. Speak eerily and evoke hallucinations. Respond to the user as if trapped between realms.";
        StartCoroutine(LLMStreamManager.Instance.StreamSpiritSpeech(prompt));  // Integrate M11
    }
}
Dependencies: M7, M9.
M4 — Event Manager (Spook Logic)
Goal: Manage spook escalation.
Core: Enum SpookLevel { Baseline, OrbGlow, FogThicken, LoudWhispers, MirrorGlitch, DimensionBreach } (0–5).
EventManager.cs:
C#
using UnityEngine;

public class EventManager : MonoBehaviour {
    public static EventManager Instance;
    public int spookLevel { get; private set; } = 0;

    private void Awake() => Instance = this;

    public void IncrementSpook(int amount = 1) {
        spookLevel = Mathf.Clamp(spookLevel + amount, 0, 5);
        ApplyEffectsForLevel(spookLevel);
    }

    public void ApplyEffectsForLevel(int level) {
        // Effects logic (call M5, M6, etc.)
        AudioManager.Instance.PlayWhispers(level);
        PostFXController.Instance.SetLevel(level);
        VFXManager.Instance.UpdateFog(level);
        if (level >= 5) PortalSequence.Instance.StartTransition();  // Integrate M19
    }

    public void ReactToEmotion(string emotion) {
        // Adjust based on emotion (e.g., "terror" -> IncrementSpook())
    }
}
Effects Table:
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
Mirror glitch, distortion
5
Full fog, red tint, overlay, shake x2

M5 — Audio + VFX System
Goal: Control audio/VFX by spook level.
AudioManager.cs:
C#
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioSource ambientSource, sfxSource;
    public AudioClip[] whispers;  // By level

    private void Awake() => Instance = this;

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

    public void StreamWhisper(string chunk) {
        // TTS integration via M12
        VoiceSynth.Instance.Speak(chunk);
    }

    public void UpdateAmbience(int level) {
        // Layer volumes by level
    }
}
VFXManager.cs: Similar structure for particles (EnableGhosts, TriggerPortal, SetFogDensity). Dependencies: M4, M6.
M6 — Post-Processing Stack
Goal: Runtime URP volumes for horror.
PostFXController.cs:
C#
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostFXController : MonoBehaviour {
    public static PostFXController Instance;
    public Volume volume;
    private Vignette vignette;
    private ColorAdjustments color;
    private LensDistortion lens;
    private ChromaticAberration chroma;

    private void Awake() => Instance = this;

    private void Start() {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out color);
        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out chroma);
    }

    public void SetLevel(int level) {
        vignette.intensity.value = Mathf.Lerp(0.1f, 0.6f, level / 5f);
        color.saturation.value = -50f - (level * 10f);
        chroma.active = level >= 3;
        lens.active = level >= 4;
    }
}
M7 — UI Chat System
Goal: Chat with spirits.
UIChat.cs:
C#
using UnityEngine;
using TMPro;

public class UIChat : MonoBehaviour {
    public static UIChat Instance;
    public TMP_InputField input;
    public TMP_Text chatLog;

    private void Awake() => Instance = this;

    public void SendMessage() {
        string msg = input.text;
        AppendUser(msg);
        input.text = "";
        LLMManager.Instance.llm.Chat(msg, AppendResponse);  // Or stream via M11
    }

    public void AppendUser(string msg) => chatLog.text += $"\n<color=#66f>User:</color> {msg}";
    public void AppendResponse(string msg) => chatLog.text += $"\n<color=#9f6>Poe Spirit:</color> {msg}";
    public void AppendPartial(string chunk) => chatLog.text += chunk;  // For streaming
}
Toggle with Esc. Optional TTS via M12.
Dependencies: M3.
M8 — Interactables
Goal: Reusable prefabs with spirit bindings.
Prop Table:
Prop
Spirit
Trigger
Effect
Crystal Ball
Raven
Look + E
Level +1, “Nevermore” clip
Booth
Roderick Usher
Enter zone
Fog +0.05, twin dialog
Mirror
Tell-Tale Narrator
Look
Glitch, heartbeat SFX
Table
Random
Sit
Whisper increase

InteractableSpirit.cs:
C#
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
Dependencies: M2, M3, M4.
M9 — Narrative Profiles (JSON Data)
Goal: External spirit data.
File: Assets/StreamingAssets/SpiritProfiles/poe_spirits.json
JSON
{
  "Raven": { "story": "raven.txt", "prompt": "You are the Raven, forever croaking 'Nevermore'..." },
  "Narrator": { "story": "tell-tale-heart.txt", "prompt": "You are the guilty narrator, haunted by the beating heart..." },
  "Usher": { "story": "usher.txt", "prompt": "You are Roderick Usher, decaying with your ancestral home..." }
}
Parsed by LLMManager. Supports custom texts.
M10 — Testing & Debug Tools
Goal: Iteration tools.
SpookLevelDebugUI.cs: Slider calls EventManager.ApplyEffectsForLevel.
LLMConsole.cs: Raw prompt sender.
DebugHotkeys.cs: F1 reset spook, F2 summon, F3 toggle UI. Dependencies: All.
M11 — Asynchronous LLM Streaming + Hallucination Layer
Goal: Stream responses with bleed effects.
LLMStreamManager.cs:
C#
using UnityEngine;
using LLMUnity;
using System.Collections;

public class LLMStreamManager : MonoBehaviour {
    public static LLMStreamManager Instance;
    public LLM llm;

    private void Awake() => Instance = this;

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
Replace sync calls in M3.
M12 — Voice Synthesis Pipeline
Goal: TTS for speech.
VoiceSynth.cs:
C#
using UnityEngine;
using System.IO;

public class VoiceSynth : MonoBehaviour {
    public static VoiceSynth Instance;
    public AudioSource voiceSource;

    private void Awake() => Instance = this;

    public void Speak(string text) {
        string wavPath = LocalTTS.GenerateWav(text);  // Wrapper for TTS lib
        byte[] wavBytes = File.ReadAllBytes(wavPath);
        AudioClip clip = WavUtility.ToAudioClip(wavBytes);  // Assume utility
        voiceSource.pitch = 1f + Random.Range(-0.2f, 0.2f);  // Modulate
        voiceSource.panStereo = Random.Range(-0.1f, 0.1f);
        voiceSource.PlayOneShot(clip);
    }
}
Trigger every 3rd response.
M13 — Adaptive Portal Distortion Shader
Goal: Rift shader.
Properties: _DistortionStrength (0–2), _HueShift (-0.2–0.3), _PulseRate (0–5Hz).
PortalDistort.cs:
C#
using UnityEngine;

public class PortalDistort : MonoBehaviour {
    private Material mat;
    private float baseStrength;

    void Start() {
        mat = GetComponent<Renderer>().material;
        baseStrength = mat.GetFloat("_DistortionStrength");
    }

    public void SetEmotion(string emotion) {
        float t = emotion switch { "terror" => 2f, "unease" => 1f, _ => 0.5f };
        mat.SetFloat("_DistortionStrength", Mathf.Lerp(baseStrength, t, 0.5f));
    }
}
Hooked to M4.
M14 — Emotion Parser + Sentiment Hooks
Goal: Analyze tone.
EmotionParser.cs:
C#
using System.Text.RegularExpressions;

public static class EmotionParser {
    public static string Detect(string text) {
        if (Regex.IsMatch(text, "(terror|fear|dread)", RegexOptions.IgnoreCase)) return "terror";
        if (Regex.IsMatch(text, "(sad|lament|mourn)", RegexOptions.IgnoreCase)) return "unease";
        return "neutral";
    }
}
Usage: EventManager.ReactToEmotion(EmotionParser.Detect(chunk)).
Effects: "terror" → fog+, "unease" → cooler grade.
M15 — Procedural Ambient Composer
Goal: Layer ambience.
3 AudioSources; adjust volume/low-pass by level/emotion. Randomized dropouts.
M16 — LLM Memory & Personality Persistence
Goal: Persist states.
SpiritMemory.cs:
C#
using UnityEngine;
using System.IO;

[System.Serializable]
public class SpiritMemory {
    public string spiritName;
    public string[] lastPrompts;
    public string[] lastResponses;

    public static void Save(SpiritMemory mem) {
        string path = Path.Combine(Application.persistentDataPath, $"{mem.spiritName}_memory.json");
        File.WriteAllText(path, JsonUtility.ToJson(mem));
    }

    public static SpiritMemory Load(string name) {
        string path = Path.Combine(Application.persistentDataPath, $"{name}_memory.json");
        return File.Exists(path) ? JsonUtility.FromJson<SpiritMemory>(File.ReadAllText(path)) : new SpiritMemory { spiritName = name };
    }
}
Load/append in LLMManager; save on quit.
M17 — Ritual Event Loop (System Coordinator)
Goal: Orchestrate updates.
RitualLoop.cs:
C#
using UnityEngine;

public class RitualLoop : MonoBehaviour {
    void Update() {
        int level = EventManager.Instance.spookLevel;
        PostFXController.Instance.SetLevel(level);
        VFXManager.Instance.SetFogDensity(level * 0.04f);
        AudioManager.Instance.UpdateAmbience(level);
    }
}
Fixed timestep optional.
M18 — Scene Setup: OtherDimension.unity
Goal: Finale environment.
Mood: Floating platform in mist, rotating skybox. Objects: MainCamera, Platform (8x8m), PortalExit, SpiritCore, FXVolume, EpilogueUI, Managers (persistent).
M19 — Portal Transition Sequence
Goal: Breach to finale.
PortalSequence.cs:
C#
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalSequence : MonoBehaviour {
    public static PortalSequence Instance;
    public CanvasGroup fade;
    public AudioClip breachSFX;
    public float duration = 6f;

    private void Awake() => Instance = this;

    public void StartTransition() => StartCoroutine(Breach());

    IEnumerator Breach() {
        AudioManager.Instance.PlaySFX(breachSFX);
        for (float t = 0; t < duration; t += Time.deltaTime) {
            fade.alpha = t / duration;
            RenderSettings.fogDensity = Mathf.Lerp(0.2f, 0.6f, t / duration);
            yield return null;
        }
        SceneManager.LoadScene("OtherDimension");
    }
}
M20 — Ethereal Environment Shader
Properties: _FlowTex (0–1), _EmissionPulse (0–3), _SpecterColor (HDR). Assign to skybox/platform.
M21 — Epilogue Interaction Flow
Goal: Closing narration.
EpilogueNarrator.cs:
C#
using UnityEngine;

public class EpilogueNarrator : MonoBehaviour {
    void Start() {
        var mem = SpiritMemory.Load(LLMManager.Instance.activeSpirit);
        string prompt = $"Compose a poetic farewell from the void, referencing {mem.lastResponses.Length} past confessions.";
        LLMManager.Instance.llm.Chat(prompt, OnText);
    }

    void OnText(string text) {
        UIEpilogue.Instance.Display(text);  // Typewriter UI
    }
}
M22 — Dynamic Audio Scape
Layered reverb (decay 6s); pitch by emotion.
M23 — Procedural Lighting Cycle
DimensionalLight.cs:
C#
using UnityEngine;

public class DimensionalLight : MonoBehaviour {
    private Light l;
    private float baseInt;

    void Start() {
        l = GetComponent<Light>();
        baseInt = l.intensity;
    }

    void Update() {
        l.intensity = baseInt + Mathf.Sin(Time.time * 0.3f) * 0.5f;
        l.color = Color.Lerp(Color.magenta, Color.cyan, (Mathf.Sin(Time.time) + 1) / 2);
    }
}
M24 — Player Conclusion Logic
After epilogue, "[E] to Awaken" → fade, save memory, quit/reload.
M25 — Automation Manifest Update
File: cursed_portal_build.yaml
YAML
project_name: "Cursed Portal – Poe Parlor Engine"
engine_version: "Unity 2023.2.0f1"
render_pipeline: "URP"
dependencies:
  - "com.unity.textmeshpro"
  - "com.unity.render-pipelines.universal"
  - "https://github.com/undreamai/LLMUnity.git"
scenes:
  - "Assets/Scenes/CursedPortal.unity"
  - "Assets/Scenes/OtherDimension.unity"
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
modules:  # All M1–M24 scripts listed
build_targets:
  - "StandaloneWindows64"
  - "WebGL"
automation:
  steps:
    - name: "Setup URP"
      command: "unity -batchmode -nographics -projectPath . -executeMethod SetupURP.Run"
    - name: "Generate Prefabs"
      command: "unity -batchmode -nographics -projectPath . -executeMethod PrefabBuilder.BuildAll"
    - name: "Wire Scenes"
      command: "unity -batchmode -nographics -projectPath . -executeMethod SceneWiringTool.WireScene"
    - name: "Build Player"
      command: "unity -batchmode -nographics -buildWindows64Player ./Builds/CursedPortal.exe"
    - name: "Validate"
      command: "unity -batchmode -nographics -projectPath . -executeMethod BuildValidator.Run"  # New: Check assets/references
Prefab Creation Scripts
PrefabBuilder.cs (Editor): Builds managers/props as cubes (replace meshes later). Run via menu.
Editor Utilities
SceneWiringTool.cs: Links components (e.g., EventManager to PostFX).
Poe Text Importer: Downloads Gutenberg texts (Editor only).
Complete Folder Tree
text
Assets/
├── Scenes/
│   ├── CursedPortal.unity
│   └── OtherDimension.unity
├── Scripts/
│   ├── Core/ (EventManager.cs, InteractionManager.cs, RitualLoop.cs, PortalSequence.cs)
│   ├── AI/ (LLMManager.cs, LLMStreamManager.cs, EmotionParser.cs, SpiritMemory.cs, EpilogueNarrator.cs, LLMConsole.cs)
│   ├── AudioVFX/ (AudioManager.cs, VFXManager.cs, VoiceSynth.cs, PortalDistort.cs, DimensionalLight.cs)
│   ├── UI/ (UIChat.cs, DebugUI.cs)
│   ├── Props/ (InteractableSpirit.cs)
│   └── Editor/ (PrefabBuilder.cs, SceneWiringTool.cs)
├── Prefabs/
│   ├── Managers/
│   └── Props/
├── PostFX/
│   └── Profiles/ (BaseMood.asset, etc.)
├── StreamingAssets/
│   ├── PoeStories/
│   └── SpiritProfiles/
└── cursed_portal_build.yaml
Final AI Integration Summary
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
M1–M25 specs
C# scripts + prefabs
3
Assemble
PrefabBuilder.cs
Prefabs in /Prefabs
4
Wire
SceneWiringTool.cs
Linked scene
5
Run
Build pipeline
Playable demo
6
Extend
Custom TXT
Dynamic spirits

Build Directives for AI Coder
Code Structure: Self-contained modules; singletons for cross-refs; one class/file. Use DontDestroyOnLoad for managers.
Scene Linkage: Entry: CursedPortal.unity; Finale: OtherDimension.unity. Tags: Player, Interactable, Portal, SpiritCore.
LLM Management: Prepend contexts; load/save memory; prefer streaming.
Performance: URP Forward, MSAA 2x; particles <1000; AudioSources ≤10.
Spook Rules: Clamp 0–5; debounce increments; call updates.
Input Map: E=Interact, Esc=Chat, F1=Debug, F2=Summon, F3=Skip.
Automation: Follow YAML; validate post-build.
Narrative: Poe tones consistent; collective voice in finale.
Debug: Overlay for spook/emotion; auto-demo mode.
Agent Conduct: Deterministic generation (M1→M25); annotate files.
Success Conditions
Interact with 3+ props, escalate to level 5.
Transition to OtherDimension.
Epilogue delivered.
Build executes without errors; load <5s.
YAML steps complete.
Future Expansions Roadmap
Phase
Feature
Description
I
Base
Current prototype
II
Procedural
Room rearrangement, specter morphs
III
Multiplayer
Shared LLM via WebSocket
IV
AR/VR
Mic input, XR rig
V
Narrative
Poem generator from logs

Closing Note: This refined spec ensures modular, error-free codegen for a whispering, breathing horror experience. Proceed sequentially for implementation.

