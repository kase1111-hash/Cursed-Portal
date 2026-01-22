# Contributing to Cursed Portal

Thank you for your interest in contributing to Cursed Portal! This guide will help you get started.

## Getting Started

### Prerequisites

- Unity 2023.2 LTS or later
- Universal Render Pipeline (URP)
- Git
- Local LLM setup (Ollama with Llama 3.x recommended)

### Setup

1. Fork the repository
2. Clone your fork:
   ```bash
   git clone https://github.com/YOUR_USERNAME/Cursed-Portal.git
   ```
3. Open the project in Unity 2023.2+
4. Install required packages via Package Manager:
   - TextMeshPro
   - Cinemachine
   - Universal RP
   - LLMUnity (from git URL)

## How to Contribute

### Reporting Bugs

1. Check existing issues to avoid duplicates
2. Use the issue template
3. Include:
   - Unity version
   - OS and hardware specs
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots or logs if applicable

### Suggesting Features

1. Open a discussion or issue
2. Describe the feature and its use case
3. Consider how it fits with the gothic horror theme
4. Reference relevant modules (M1-M25) if applicable

### Code Contributions

1. **Create a branch** from `main`:
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Follow the code structure**:
   - `Assets/Scripts/Core/` - Core game systems
   - `Assets/Scripts/AI/` - LLM and spirit systems
   - `Assets/Scripts/UI/` - User interface
   - `Assets/Scripts/AudioVFX/` - Audio and visual effects
   - `Assets/Scripts/Props/` - Interactable objects
   - `Assets/Scripts/Editor/` - Editor tools

3. **Code standards**:
   - One class per file
   - Use singleton pattern for managers
   - Add header comment with module reference:
     ```csharp
     // Cursed Portal - Module M##
     // Description of file purpose
     ```
   - Follow C# naming conventions

4. **Test your changes**:
   - Verify in both CursedPortal.unity and OtherDimension.unity scenes
   - Test spook level transitions (0-5)
   - Ensure WebGL compatibility where applicable

5. **Submit a Pull Request**:
   - Use the PR template
   - Reference any related issues
   - Describe your changes clearly

## Module Development

When adding new modules, follow the established pattern:

```csharp
using UnityEngine;

public class NewManager : MonoBehaviour {
    public static NewManager Instance;

    void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### Key Integration Points

- **EventManager** - Hook into spook level changes
- **LLMManager** - Integrate with spirit dialogue
- **AudioManager/VFXManager** - Add atmospheric effects
- **RitualLoop** - Register for per-frame updates

## Asset Contributions

### Audio
- Mono, 44.1kHz
- Max 15 seconds for whispers
- Gothic/horror appropriate

### Visual
- URP compatible shaders
- Keep particle count under 1000
- Support both PC and WebGL

### Narrative
- Poe-inspired prose
- Add spirit profiles to `StreamingAssets/SpiritProfiles/`
- Story texts go in `StreamingAssets/PoeStories/`

## Review Process

1. All PRs require at least one review
2. CI checks must pass (if configured)
3. Changes should not break existing functionality
4. Documentation updates are appreciated

## Questions?

- Open a GitHub Discussion
- Check the `Build_Guide.md` and `Spec.md` for technical details
- Review `AI-instructions.md` for implementation guidance

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
