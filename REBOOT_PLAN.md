# CURSED PORTAL — REBOOT PLAN

**Goal:** Transform 44 orphaned C# scripts into a playable Unity prototype.
**Principle:** Every phase ends with something that compiles and runs. No phase depends on assets that don't exist yet.

---

## PHASE 0: SCORCHED EARTH CLEANUP
**Outcome:** A clean repo with only what matters.

### Delete from repo root
| File | Reason |
|------|--------|
| `KEYWORDS.md` | 10KB of SEO keywords for software that doesn't run |
| `Commercial_License.md` | Premature licensing for an uncompilable prototype |
| `CONTRIBUTING.md` | Contribution guidelines for a solo AI-generated project |
| `AUDIT_REPORT.md` | Self-generated audit. Replace with real test results later |
| `AI-instructions.md` | AI prompt toolkit — not part of the game. Archive or move to a wiki |
| `Build_Guide.md` | Build guide for a build that can't happen yet. Rewrite after Phase 1 |

### Delete from Scripts
| File | Reason |
|------|--------|
| `Assets/Scripts/AudioVFX/VoiceSynth.cs` | 100% stub. TTS is a v2 feature. AudioManager whisper fallback covers v1 |

### Rename
- `Spec.md` → `DESIGN.md` (it's a design doc, not a spec — specs are testable)

### Keep but quarantine
Move `Assets/Scripts/Editor/` contents to `Assets/Scripts/_Deferred/Editor/`. These tools wire up a Unity project that doesn't exist yet. They'll be useful in Phase 1 but shouldn't distract until then.

**Deliverable:** `git rm` the dead weight, commit as "Phase 0: Strip to essentials".

---

## PHASE 1: CREATE A REAL UNITY PROJECT
**Outcome:** A Unity project that opens, compiles with zero errors, and shows an empty scene.

### 1.1 — Unity project shell
Create a new Unity 2023.2 LTS project with URP template. This generates:
- `ProjectSettings/` (all mandatory editor configs)
- `Packages/manifest.json` (package manager)
- `ProjectVersion.txt`
- `.meta` files for everything

### 1.2 — Package dependencies
Install via Package Manager:
```
com.unity.textmeshpro
com.unity.render-pipelines.universal
com.unity.cinemachine          (needed later for camera shake)
```
Install via git URL in `manifest.json`:
```
"com.undream.llmunity": "https://github.com/undreamai/LLMUnity.git"
```

### 1.3 — Import existing scripts
Copy `Assets/Scripts/` into the Unity project's `Assets/Scripts/`. Unity will auto-generate `.meta` files.

**Expected:** Compilation errors from missing references (LLMUnity types, TMP types). Fix by ensuring packages are installed. If LLMUnity API has changed since the scripts were written, adapt `LLMManager.cs` and `LLMStreamManager.cs` to the current API.

### 1.4 — Import streaming assets
Copy `Assets/StreamingAssets/` into the Unity project. Verify:
- `StreamingAssets/PoeStories/raven.txt` exists and contains the poem
- `StreamingAssets/PoeStories/tell-tale-heart.txt` exists
- `StreamingAssets/PoeStories/usher.txt` exists
- `StreamingAssets/SpiritProfiles/poe_spirits.json` is valid JSON

If the Poe text files are empty placeholders, download the actual texts from Project Gutenberg now.

### 1.5 — Create SingletonBase<T>
Replace 13 copy-pasted singleton implementations with one generic base class:

```csharp
// Assets/Scripts/Core/SingletonBase.cs
using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
```

Refactor these files to inherit from `SingletonBase<T>` instead of self-implementing:
- `GameManager.cs`
- `EventManager.cs`
- `InteractionManager.cs`
- `LLMManager.cs`
- `LLMStreamManager.cs`
- `AudioManager.cs`
- `VFXManager.cs`
- `PostFXController.cs`
- `UIChat.cs`
- `PortalSequence.cs`
- `CursorManager.cs`
- `RitualLoop.cs`
- `FinaleManager.cs`

This also fixes the `OnDestroy` leak (Instance never nulled) across all managers.

### 1.6 — Compile check
Open the project in Unity. Fix every compiler error. Do not proceed until the console shows zero errors.

**Deliverable:** A Unity project that opens and compiles. Commit as "Phase 1: Real Unity project with zero compile errors".

---

## PHASE 2: ONE ROOM, ONE SPIRIT, ONE CONVERSATION
**Outcome:** Player walks around a room, approaches a prop, presses E, types a message, and the Raven responds via local LLM.

### 2.1 — Create the test scene
Create `Assets/Scenes/CursedPortal.unity`:
- A floor plane (10x10)
- Four walls (cubes scaled as walls)
- A point light or two for basic visibility
- A cube or sphere in the center labeled "CrystalBall" (placeholder for the Raven prop)
- URP lighting: set ambient to dark purple, directional light low intensity

### 2.2 — Wire up the player
Create an empty GameObject "Player":
- Add `CharacterController` component
- Attach `FirstPersonController.cs`
- Create child "MainCamera" with `Camera` component
- Attach `CameraController.cs` to the camera
- Attach `CursorManager.cs` to the player

Position at `(0, 1, -4)`. Verify WASD movement and mouse look work.

### 2.3 — Wire up the minimum managers
Create an empty GameObject "Managers":
- Attach `GameManager.cs`
- Attach `EventManager.cs`
- Attach `InteractionManager.cs`

Set `InteractionManager` to raycast from the main camera. Verify raycasts fire (check Debug.Log output).

### 2.4 — Wire up the prop
On the CrystalBall placeholder object:
- Add a `Collider` (BoxCollider or SphereCollider)
- Attach `InteractableSpirit.cs`
- Set `spiritKey` to "Raven"
- Set `storyKey` to "raven.txt"

Verify: walk up to it, see highlight log, press E, see interaction log.

### 2.5 — Wire up the LLM
- Attach `LLMManager.cs` to Managers
- Attach `LLMStreamManager.cs` to Managers
- Verify `poe_spirits.json` loads correctly
- Start a local Ollama instance: `ollama run llama3.2:3b`
- Confirm the endpoint URL in `LLMManager.cs` matches (default: `http://localhost:8080/completion` — may need to change to Ollama's default `http://localhost:11434/api/generate`)

**Critical:** The hardcoded endpoint in `LLMManager.cs` assumes llama.cpp format. Ollama uses a different API format. Either:
- (a) Run llama.cpp server on port 8080, or
- (b) Update `LLMManager.cs` to use Ollama's API: `POST http://localhost:11434/api/generate` with `{"model": "llama3.2:3b", "prompt": "...", "stream": true}`

This is the most likely point of failure. Test with `curl` first before wiring into Unity.

### 2.6 — Wire up the chat UI
Create a Canvas:
- Attach `UIChat.cs`
- Create ScrollView with TextMeshPro text for chat log
- Create TMP InputField for player input
- Create Send button
- Wire references in the Inspector

Test: press ESC to toggle chat, type a message, verify it appears in the log and triggers `LLMManager.SummonSpirit()`.

### 2.7 — End-to-end test
1. Play the scene
2. Walk to the crystal ball
3. Press E (summon the Raven)
4. Press ESC (open chat)
5. Type "Who are you?"
6. See the Raven respond in Poe's voice

If this works, the core loop is proven.

**Deliverable:** A playable scene with one working spirit conversation. Commit as "Phase 2: Core loop — Raven speaks".

---

## PHASE 3: FIX THE BUGS
**Outcome:** Known bugs from the evaluation are resolved.

### 3.1 — LLMStreamManager race condition
The `currentStreamCoroutine` assignment happens outside the lock, then gets nulled inside the lock immediately after. Fix:

```csharp
// Move coroutine tracking inside the lock properly
// Ensure CancelStreamInternal() can actually stop a running stream
// Option: track streaming state via a CancellationToken-like bool
//         instead of relying on StopCoroutine()
```

Specifically: add an `isCancelled` flag checked inside the streaming loop. When a new stream request arrives, set `isCancelled = true`, wait one frame, then start the new stream.

### 3.2 — InteractableSpirit material leak
Replace `.material = highlightMaterial` with `MaterialPropertyBlock` usage:
```csharp
private MaterialPropertyBlock propBlock;
private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

public void OnHighlightEnter()
{
    if (propBlock == null) propBlock = new MaterialPropertyBlock();
    propRenderer.GetPropertyBlock(propBlock);
    propBlock.SetColor(EmissionColor, highlightColor);
    propRenderer.SetPropertyBlock(propBlock);
}
```
This avoids creating material instances entirely.

### 3.3 — EventManager empty switch cases
Either implement the per-level effects or delete the empty cases. For the MVP, simplify to a linear interpolation:
```csharp
// Instead of a 6-case switch with 4 empty cases,
// use continuous interpolation:
float t = currentSpookLevel / (float)maxSpookLevel;
AudioManager.Instance?.SetSpookIntensity(t);
VFXManager.Instance?.SetSpookIntensity(t);
PostFXController.Instance?.SetLevel(currentSpookLevel);
```

### 3.4 — FinaleManager null safety
- Add null-check before `CharacterController` re-enable
- Create `PlayerSpawnMarker` class (or use a tagged Transform)
- Make spirit names array configurable via Inspector instead of hardcoded

### 3.5 — EpilogueNarrator error handling
- Wrap JSON parsing in try-catch
- Add `UnityWebRequest.Result` check before parsing
- Fallback for empty `Environment.UserName`

**Deliverable:** All evaluation bugs fixed. Commit as "Phase 3: Fix concurrency, memory, and null-safety bugs".

---

## PHASE 4: ATMOSPHERE
**Outcome:** The spook escalation system works. Fog thickens, audio builds, post-processing intensifies as the player interacts with spirits.

### 4.1 — Audio setup
- Attach `AudioManager.cs` to Managers
- Create 4 AudioSource components (ambient, SFX, voice, whisper) on the Managers object
- Source placeholder audio: download 5 free ambient/whisper clips from Freesound.org (CC0)
- Assign clips to the whisper array (one per spook level)

### 4.2 — VFX setup
- Attach `VFXManager.cs` to Managers
- Enable Unity fog in Lighting Settings (Exponential Squared, start density 0.01)
- Create a basic ParticleSystem for fog particles (low emission, large soft particles, no texture needed)
- Wire references

### 4.3 — Post-processing setup
- Attach `PostFXController.cs` to Managers
- Create a URP Volume (Global) with:
  - Vignette (start at 0.1)
  - Color Adjustments (saturation start at 0)
  - Chromatic Aberration (disabled, enabled at level 3+)
  - Lens Distortion (disabled, enabled at level 4+)
- Wire the Volume reference

### 4.4 — Integration test
1. Interact with the Raven 5 times
2. Watch spook level climb 0 → 5
3. Verify: fog gets denser, audio gets louder, vignette tightens, colors desaturate
4. At level 5, confirm `OnDimensionBreach` event fires (log it — don't need the portal scene yet)

**Deliverable:** Escalating atmosphere works end-to-end. Commit as "Phase 4: Spook escalation system live".

---

## PHASE 5: ALL THREE SPIRITS
**Outcome:** Crystal Ball (Raven), Mirror (Narrator), and Booth (Usher) are all interactable with distinct visual behaviors.

### 5.1 — Add props
Place three more placeholder objects in the scene:
- "Mirror" — a tall thin cube on a wall
- "Booth" — a cube formation suggesting an enclosed space
- "Table" — a flat cube

Attach the corresponding prop scripts (`MirrorProp.cs`, `BoothProp.cs`, `TableProp.cs`) and `InteractableSpirit.cs` with correct spirit/story keys.

### 5.2 — Prop highlights
- Attach `PropHighlight.cs` to each prop
- Create two materials: `DefaultMaterial` (dark) and `HighlightMaterial` (emissive outline)
- Wire them up

### 5.3 — Spirit memory
- Attach `SpiritMemory.cs` to Managers (or let `LLMManager` manage it)
- Verify: talk to the Raven twice, close and reopen the game, talk again — the Raven remembers the previous conversation

### 5.4 — Emotion parsing
- Verify `EmotionParser.cs` is wired into the response pipeline
- When the Raven says something with "terror" or "dread", the spook level should get a small reactive bump

**Deliverable:** Three distinct spirits with memory and emotional reactivity. Commit as "Phase 5: Three spirits with memory".

---

## PHASE 6: THE FINALE
**Outcome:** At spook level 5, a portal sequence transitions to the OtherDimension scene with an epilogue.

### 6.1 — Create OtherDimension scene
Create `Assets/Scenes/OtherDimension.unity`:
- Empty dark environment
- A glowing sphere ("SpiritCore") in the center
- Attach `DimensionalLight.cs` for pulsing light
- A Canvas with `UIEpilogue.cs` for the farewell text

### 6.2 — Wire PortalSequence
- Attach `PortalSequence.cs` to a new "PortalSequence" object in the main scene
- Wire it to trigger on `EventManager.OnDimensionBreach`
- Create a simple fade-to-black screen effect using `ScreenEffects.cs`
- Async-load the OtherDimension scene

### 6.3 — Wire FinaleManager + EpilogueNarrator
- In OtherDimension scene, create Managers with `FinaleManager.cs`
- Attach `EpilogueNarrator.cs`
- The narrator should compose a farewell using stored `SpiritMemory` context
- After the epilogue, display a "Press any key to exit" prompt

### 6.4 — End-to-end playthrough
1. Start in the parlor
2. Interact with all three spirits
3. Reach spook level 5
4. Portal fires, screen fades
5. OtherDimension loads
6. Epilogue plays
7. Game ends

**Deliverable:** Complete game loop from start to finish. Commit as "Phase 6: Full playthrough — parlor to portal to epilogue".

---

## PHASE 7: POLISH & SHIP
**Outcome:** A publishable prototype.

### 7.1 — Restore deferred Editor tools
Move `_Deferred/Editor/` scripts back to `Assets/Scripts/Editor/`. Now that the Unity project exists, these tools can:
- Auto-generate prefabs (`PrefabBuilder.cs`)
- Validate scene wiring (`SceneWiringTool.cs`)
- Set up OtherDimension (`OtherDimensionSetup.cs`)

### 7.2 — Add remaining atmosphere scripts
Wire up the polish layer — only what has real audio/visual assets to back it:
- `CameraShake.cs` (Cinemachine Impulse or Perlin-noise fallback)
- `CandleFlicker.cs` (if candle lights exist)
- `HeartbeatEffect.cs` (if heartbeat audio clip exists)
- `AmbienceController.cs` (layered ambient audio)
- `FootstepSystem.cs` (if floor has physics material for surface detection)

Skip anything that doesn't have an asset to drive it.

### 7.3 — Debug tools
- Wire `DebugUI.cs` and `SpookLevelDebugUI.cs` behind a `#if UNITY_EDITOR || DEVELOPMENT_BUILD` guard
- Verify F1/F2/F3 hotkeys work

### 7.4 — Build
- Test Windows build (standalone)
- Test WebGL build (note: LLM needs a proxy or embedded model for WebGL)
- Verify StreamingAssets are included
- Verify no missing script references in build

### 7.5 — Rewrite the README
Replace the current 36KB specification-README with a concise project README:
- What it is (2-3 sentences)
- How to run it (prerequisites, build steps)
- How to configure the LLM backend
- Screenshot or GIF
- License

**Deliverable:** Publishable prototype. Tag as `v0.1.0`.

---

## PHASE SUMMARY

| Phase | Outcome | Files Touched | Dependency |
|-------|---------|---------------|------------|
| 0 | Clean repo | 7 deleted, 1 renamed | None |
| 1 | Compiling Unity project | All scripts + new SingletonBase | Phase 0 |
| 2 | One spirit talks | ~18 scripts + scene | Phase 1 |
| 3 | Bugs fixed | 5 scripts patched | Phase 2 |
| 4 | Atmosphere works | 3 manager scripts + assets | Phase 3 |
| 5 | Three spirits with memory | 6 prop/AI scripts + assets | Phase 4 |
| 6 | Full game loop with finale | 5 scripts + 2nd scene | Phase 5 |
| 7 | Shippable prototype | Editor tools + build | Phase 6 |

### Script Disposition (44 files)

| Disposition | Count | Files |
|-------------|-------|-------|
| **Keep as-is** | 33 | All Props, all Player, most AudioVFX, UI, EmotionParser, SpiritMemory, IInteractable, RitualLoop, SpookTriggerZone |
| **Fix specific bugs** | 5 | LLMStreamManager, InteractableSpirit, EventManager, FinaleManager, EpilogueNarrator |
| **Refactor (singleton)** | 13 | All singleton managers → inherit SingletonBase\<T\> |
| **Cut** | 1 | VoiceSynth.cs (defer TTS to v2) |
| **New file needed** | 1 | SingletonBase\<T\> |

### Documentation Disposition (13 files)

| Disposition | Count | Files |
|-------------|-------|-------|
| **Keep** | 4 | README.md (rewrite in Phase 7), LICENSE.md, SECURITY.md, CHANGELOG.md |
| **Rename** | 1 | Spec.md → DESIGN.md |
| **Delete** | 5 | KEYWORDS.md, Commercial_License.md, CONTRIBUTING.md, AUDIT_REPORT.md, AI-instructions.md |
| **Delete & rebuild later** | 1 | Build_Guide.md |
| **Already added** | 2 | EVALUATION_REPORT.md, REBOOT_PLAN.md |

---

## NON-NEGOTIABLE RULES FOR THE REBOOT

1. **No phase starts until the previous phase compiles and runs.** No exceptions.
2. **No script references assets that don't exist.** If a script needs an AudioClip, the clip must be in the project or the reference must be nullable with a silent skip.
3. **No new documentation until Phase 7.** Build first, document second.
4. **Test with real LLM responses.** The Raven must actually say something. Mock responses are acceptable only for CI/CD, not for milestone sign-off.
5. **One commit per phase.** Each phase is a meaningful, reversible checkpoint.
