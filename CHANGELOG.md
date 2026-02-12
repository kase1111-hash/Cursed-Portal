# Changelog

All notable changes to Cursed Portal will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Project documentation and ecosystem links
- SEO keywords and related repository connections

## [1.0.0] - 2026-01-22

### Added

#### Core Systems (M1-M10)
- **M1 - Core Setup**: Base environment with URP lighting, fog, and player spawn
- **M2 - Interaction Framework**: Raycast-based interaction system with IInteractable interface
- **M3 - LLM Manager**: Central AI system for Poe spirit interactions via LLMUnity
- **M4 - Event Manager**: Spook level system (0-5) with escalating horror effects
- **M5 - Audio + VFX System**: Ambient audio, whispers, and particle effects
- **M6 - Post-Processing Stack**: URP Volume profiles for horror intensity
- **M7 - UI Chat System**: TextMeshPro-based chat interface for spirit dialogue
- **M8 - Interactables**: Crystal Ball, Mirror, Booth, and Table props
- **M9 - Narrative Profiles**: JSON-based spirit configurations and Poe story texts
- **M10 - Testing & Debug Tools**: Spook level slider and debug hotkeys

#### Advanced Systems (M11-M17)
- **M11 - LLM Streaming**: Asynchronous response streaming with hallucination effects
- **M12 - Voice Synthesis**: TTS pipeline for spirit voice output (deferred — VoiceSynth.cs removed, see REBOOT_PLAN.md)
- **M13 - Portal Distortion Shader**: Emotion-reactive dimensional rift visuals
- **M14 - Emotion Parser**: Sentiment analysis driving environmental responses
- **M15 - Procedural Ambience**: Layered ambient audio composer
- **M16 - Spirit Memory**: Cross-session persistence for spirit conversations
- **M17 - Ritual Loop**: Global system coordinator

#### Finale Content (M18-M25)
- **M18 - OtherDimension Scene**: Floating platform finale environment
- **M19 - Portal Transition**: Scene transition with fade and breach effects
- **M20 - Ethereal Shader**: Otherworldly visual effects for finale
- **M21 - Epilogue Narrator**: AI-generated closing narration using memory
- **M22 - Dynamic Audio Scape**: Reverse-reverb drones for finale
- **M23 - Dimensional Lighting**: Animated lighting with color cycling
- **M24 - Player Conclusion**: Exit flow with memory save
- **M25 - Build Automation**: YAML manifest and CI pipeline support

#### Spirit Characters
- The Raven - Obsession, refrain, and despair
- Tell-Tale Heart Narrator - Guilt and heartbeat motifs
- Roderick Usher - Decay, confinement, and bloodline rot

#### Build Targets
- Windows 64-bit
- Linux 64-bit
- WebGL (with LLM fallback)

### Technical Details
- Unity 2023.2 LTS with Universal Render Pipeline
- Local LLM integration via Ollama/llama.cpp
- ~11,200 lines of C# across 46 script files
- Modular architecture with 25 independent modules
