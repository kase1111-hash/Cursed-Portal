Cursed Portal – Poe Parlor Development Roadmap
1. Overview
This roadmap defines the step-by-step plan for implementing, testing, and releasing the Cursed Portal – Poe Parlor prototype.
 It is based on the full seven-part specification and structured for both human and AI co-development.

2. Phases
Phase
Description
Duration
Key Deliverable
Phase 0
Repository setup, LLMUnity install, URP project creation
0.5 days
Working Unity skeleton
Phase 1
Core scene, camera, and player controller (M1–M2)
1 day
Walkable gothic room
Phase 2
LLM integration and UI chat (M3, M7, M9)
1.5 days
Functional Poe spirit chat
Phase 3
Spook escalation logic (M4–M5–M6)
1 day
Dynamic atmosphere effects
Phase 4
Prop interactions (M8)
0.5 days
Working triggers per spirit
Phase 5
Automation, prefab setup, YAML builds (M10–M11)
0.5 days
One-click build
Phase 6
Procedural horror systems (M12–M17)
1–2 days
Adaptive AI behavior
Phase 7
Other Dimension finale scene (M18–M25)
1 day
Fully playable demo
Phase 8
Playtest + polish
0.5 days
Publishable prototype


3. Milestone Goals
🎯 Milestone 1 – Foundations Online
Unity project compiles with LLMUnity installed


Scene “CursedPortal” loads and player can move


InteractionManager detects raycast hits


Validation: Build runs at >60 FPS with no console errors.

🎯 Milestone 2 – First Contact
Poe spirit chat functional via LLMManager


UIChat displays and scrolls correctly


Response tone matches Poe style


Ambient whispers and fog adjust on interaction


Validation: You can hold a conversation with “The Raven”.

🎯 Milestone 3 – Atmosphere Rising
EventManager successfully escalates fog and sound


VFXManager and PostFXController respond to spook level


Mirror glitch and orb glow activate visually


Validation: Player feels tension build-up organically.

🎯 Milestone 4 – Dimensional Breach
PortalSequence transitions to OtherDimension.unity


All manager singletons persist across scenes


LLM epilogue loads and displays text on SpiritCore


Validation: End-to-end flow complete.

4. Assets & Content
Asset Type
Source
Location
Poe Texts
Project Gutenberg
/StreamingAssets/PoeStories/
CC0 Sounds
Freesound.org
/Audio/
3D Models
PolyHaven / Sketchfab (free)
/Models/
VFX
Unity Particle Systems
/VFX/


5. Build & Testing
Local Build Targets
✅ PC (Primary)


✅ WebGL (Local LLM simulation)


QA Checklist
LLM responses load within 3 seconds


All prefabs instantiate cleanly


Fog transitions smooth (no pop-in)


Audio does not clip or duplicate


Exit transition fires on Level 5



6. Post-Build Tasks
Package as Unity .unitypackage


Add README + License summary


Push to GitHub releases


Publish to:


🧩 Unity Asset Store (Paid license version)


💾 Itch.io (free demo version)


🧠 Hugging Face Spaces (if you add LLM WebGL proxy)



7. Future Expansion Notes
See Part 6 & 7 for advanced modules (Procedural Horror, Other Dimension).
 Target a v1.0 release once base game loop is stable.
 Optional v2.0: Multiplayer Séance Mode.

📦 Recommended Next Steps
Step
File/Folder
Description
1️⃣
Development_Roadmap.md
Create and commit this file
2️⃣
/Assets/Scripts/Managers/
Begin coding from M1–M2
3️⃣
/Assets/Scenes/CursedPortal.unity
Implement room layout
4️⃣
cursed_portal_build.yaml
Automate prefab/build process
5️⃣
README.md
Add license summary and link to demo video (later)


