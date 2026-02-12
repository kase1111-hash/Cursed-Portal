# Cursed Portal -- Poe Parlor Development Roadmap

## 1. Overview

This roadmap defines the step-by-step plan for implementing, testing, and releasing the Cursed Portal -- Poe Parlor prototype. It is based on the full seven-part specification and structured for both human and AI co-development.

For a more detailed phase-by-phase guide, see `REBOOT_PLAN.md`.

## 2. Phases

| Phase | Description | Key Deliverable |
|-------|-------------|-----------------|
| Phase 0 | Repository setup, LLMUnity install, URP project creation | Working Unity skeleton |
| Phase 1 | Core scene, camera, and player controller (M1--M2) | Walkable gothic room |
| Phase 2 | LLM integration and UI chat (M3, M7, M9) | Functional Poe spirit chat |
| Phase 3 | Spook escalation logic (M4--M5--M6) | Dynamic atmosphere effects |
| Phase 4 | Prop interactions (M8) | Working triggers per spirit |
| Phase 5 | Automation, prefab setup, YAML builds (M10--M11) | One-click build |
| Phase 6 | Procedural horror systems (M12--M17) | Adaptive AI behavior |
| Phase 7 | Other Dimension finale scene (M18--M25) | Fully playable demo |
| Phase 8 | Playtest + polish | Publishable prototype |

## 3. Milestone Goals

### Milestone 1 -- Foundations Online
- Unity project compiles with LLMUnity installed
- Scene "CursedPortal" loads and player can move
- InteractionManager detects raycast hits
- **Validation:** Build runs at >60 FPS with no console errors.

### Milestone 2 -- First Contact
- Poe spirit chat functional via LLMManager
- UIChat displays and scrolls correctly
- Response tone matches Poe style
- Ambient whispers and fog adjust on interaction
- **Validation:** You can hold a conversation with "The Raven".

### Milestone 3 -- Atmosphere Rising
- EventManager successfully escalates fog and sound
- VFXManager and PostFXController respond to spook level
- Mirror glitch and orb glow activate visually
- **Validation:** Player feels tension build-up organically.

### Milestone 4 -- Dimensional Breach
- PortalSequence transitions to OtherDimension.unity
- All manager singletons persist across scenes
- LLM epilogue loads and displays text on SpiritCore
- **Validation:** End-to-end flow complete.

## 4. Assets & Content

| Asset Type | Source | Location |
|------------|--------|----------|
| Poe Texts | Project Gutenberg | `Assets/StreamingAssets/PoeStories/` |
| CC0 Sounds | Freesound.org | `Assets/Audio/` |
| 3D Models | PolyHaven / Sketchfab (free) | `Assets/Models/` |
| VFX | Unity Particle Systems | `Assets/VFX/` |

## 5. Build & Testing

**Local Build Targets:**
- PC (Primary)
- WebGL (requires LLM proxy -- local LLM cannot run in browser)

**QA Checklist:**
- [ ] LLM responses load within 3 seconds
- [ ] All prefabs instantiate cleanly
- [ ] Fog transitions smooth (no pop-in)
- [ ] Audio does not clip or duplicate
- [ ] Exit transition fires on Level 5

## 6. Post-Build Tasks

1. Package as Unity `.unitypackage`
2. Add README + License summary
3. Push to GitHub releases
4. Publish to:
   - Unity Asset Store (paid license version)
   - Itch.io (free demo version)
   - Hugging Face Spaces (if you add LLM WebGL proxy)

## 7. Future Expansion Notes

See Part 6 & 7 of `DESIGN.md` for advanced modules (Procedural Horror, Other Dimension). Target a v1.0 release once base game loop is stable. Optional v2.0: Multiplayer Seance Mode.

## Recommended Next Steps

| Step | File/Folder | Description |
|------|-------------|-------------|
| 1 | `Assets/Scripts/Core/` | Core systems are implemented -- verify compilation in Unity |
| 2 | `Assets/Scenes/CursedPortal.unity` | Generate via CursedPortal > Setup Main Scene editor tool |
| 3 | `Assets/StreamingAssets/PoeStories/` | Replace placeholder files with full texts from Project Gutenberg |
| 4 | `Assets/cursed_portal_build.yaml` | Automate prefab/build process |
| 5 | `README.md` | Add link to demo video once available |
