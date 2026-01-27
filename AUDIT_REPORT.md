# Software Correctness & Fitness Audit Report

## Cursed Portal - Poe Parlor

**Audit Date:** 2026-01-27
**Auditor:** Claude (Automated Code Review)
**Scope:** All 44 C# source files in Assets/Scripts/

---

## Executive Summary

**Cursed Portal** is an LLM-powered gothic horror game built in Unity that integrates local AI (Ollama/llama.cpp) with interactive storytelling. The codebase demonstrates solid architecture with appropriate use of design patterns, but contains several correctness issues and potential problems that should be addressed.

**Overall Assessment**: The software is **largely fit for purpose** as a prototype/indie game, but has notable bugs, edge cases, and architectural concerns that could affect reliability in production.

---

## CRITICAL ISSUES

### 1. Race Condition in LLM Streaming State

**File:** `Assets/Scripts/AI/LLMStreamManager.cs:28-74`

```
Problem: The `isStreaming` flag is not properly protected against concurrent requests.
```

- Line 52-56: If `StreamSpiritSpeech()` is called while already streaming, it returns early but doesn't clear the previous coroutine reference
- Line 278-286: `CancelStream()` stops the coroutine but doesn't handle the case where the HTTP request is still in-flight
- **Impact**: Could leave the system in a stuck state if a stream is cancelled during network operation

### 2. Null Reference Risk in LLM Response Parsing

**File:** `Assets/Scripts/AI/LLMManager.cs:219-227`

```csharp
LLMResponse llmResponse = JsonUtility.FromJson<LLMResponse>(response);
responseContent = llmResponse.content;  // Potential null
```

- While there's a try-catch, `llmResponse` could be non-null but have a null `content` field
- The fallback (line 226) uses raw response which may contain invalid JSON
- **Impact**: Could display malformed text to user or cause silent failures

### 3. Memory Leak in Singleton Persistence

**Files:** `EventManager.cs`, `AudioManager.cs`, `LLMManager.cs`, others

Multiple singletons use `DontDestroyOnLoad()` but share static `Instance` references:
- When scene reloads, destroyed duplicates don't clear static references before destruction
- **Impact**: Potential reference to destroyed objects after scene transitions

---

## HIGH SEVERITY ISSUES

### 4. Debounce Array Index Out of Bounds

**File:** `Assets/Scripts/Core/EventManager.cs:94-97`

```csharp
for (int i = level + 1; i <= maxSpookLevel; i++)
{
    levelTriggered[i] = false;
}
```

- If `maxSpookLevel` is modified at runtime to a value larger than the array, this will crash
- Array is created once in `Awake()` with size `maxSpookLevel + 1`
- **Impact**: Runtime crash if configuration changes

### 5. Streaming Chunk Parsing Incomplete

**File:** `Assets/Scripts/AI/LLMStreamManager.cs:198-230`

```csharp
if (chunk.StartsWith("data:"))
```

- Does not handle multi-line SSE format where chunks may be split
- Doesn't handle the `data: ` prefix with trailing newlines correctly
- Missing handling for SSE events like `:heartbeat` or `event:` lines
- **Impact**: Partial/corrupted responses displayed to user

### 6. Whisper Clip Array Bounds

**File:** `Assets/Scripts/AudioVFX/AudioManager.cs:97-107`

```csharp
if (whisperClips == null || level >= whisperClips.Length)
```

- No guarantee whisperClips array matches `maxSpookLevel` (defined in EventManager)
- If fewer clips are assigned than levels, higher levels silently fail
- **Impact**: Silent audio failure at higher spook levels

### 7. Story File Loading Blocks Main Thread

**File:** `Assets/Scripts/AI/LLMManager.cs:169-185`

```csharp
string fullText = File.ReadAllText(storyPath);
```

- Synchronous file I/O on main thread
- With large story files, this causes frame hitches
- **Impact**: Noticeable stuttering during interaction

---

## MEDIUM SEVERITY ISSUES

### 8. Emotion Parser Keyword Overlap

**File:** `Assets/Scripts/AI/EmotionParser.cs:19-21`

The word "heart" appears in the terror pattern but is thematically relevant to "Tell-Tale Heart" which should trigger Poe-specific scoring:

```csharp
// "heart" in terrorPattern, but heart is Poe-specific
private static readonly Regex terrorPattern = new Regex(
    @"\b(terror|fear|dread|heart|mad|...\b"
```

**Impact**: Emotion scoring skewed for Narrator spirit interactions

### 9. Fog Burst Coroutine Can Stack

**File:** `Assets/Scripts/AudioVFX/VFXManager.cs:112-135`

```csharp
public void AddFogBurst(float additionalDensity)
{
    StartCoroutine(FogBurstCoroutine(additionalDensity, 2f));
}
```

- Multiple rapid calls stack fog bursts without cancelling previous ones
- Each coroutine lerps back to a different "originalDensity" captured at start time
- **Impact**: Fog density oscillates unpredictably during intense sequences

### 10. Portal Distort Singleton Reassignment

**File:** `Assets/Scripts/AudioVFX/PortalDistort.cs:51-55`

```csharp
else if (Instance != this)
{
    Instance = this;  // Reassigns without cleanup
}
```

- Allows multiple instances to fight over singleton
- Previous instance's state not transferred
- **Impact**: Visual inconsistency during scene transitions

### 11. UIChat Truncation Breaks Rich Text

**File:** `Assets/Scripts/UI/UIChat.cs:242-254`

```csharp
int newlineIndex = chatLog.text.IndexOf('\n', cutPoint);
if (newlineIndex > 0)
{
    chatLog.text = chatLog.text.Substring(newlineIndex + 1);
}
```

- Can cut in middle of `<color=...>` tags
- Rich text becomes malformed after truncation
- **Impact**: Chat display corruption with color bleeding

### 12. Missing Stop Token Consistency

**Files:** `LLMManager.cs:200` and `LLMStreamManager.cs:89`

Different stop sequences used:
- Non-streaming: `"User:", "\n\n"`
- Streaming: `"User:", "\n\n\n"`

**Impact**: Inconsistent response termination between modes

---

## LOW SEVERITY ISSUES

### 13. Hardcoded LLM Endpoint

- `http://localhost:8080/completion` hardcoded in multiple files
- Should be centralized or configurable

### 14. Player Username in Epilogue

**File:** `Assets/Scripts/AI/EpilogueNarrator.cs:100`

```csharp
string playerName = System.Environment.UserName;
```

- Exposes system username to LLM prompt
- Minor privacy concern

### 15. Unused `currentStreamCoroutine` Variable

**File:** `Assets/Scripts/AI/LLMStreamManager.cs:28`

- Variable declared but never assigned
- `CancelStream()` references it but it's always null

### 16. Missing Timeout on Streaming Request

- `LLMStreamManager.StreamRequest()` has no timeout
- Network issues could hang indefinitely

### 17. Material Instance Leak

**File:** `Assets/Scripts/Props/InteractableSpirit.cs:51-52`

```csharp
originalMaterial = propRenderer.material;  // Creates instance copy
```

- Accessing `.material` creates a new instance
- Use `.sharedMaterial` for read-only access
- **Impact**: Memory growth over time

---

## ARCHITECTURAL CONCERNS

### 18. Excessive Singleton Usage

- 15+ singletons across the codebase
- Creates tight coupling and makes testing difficult
- Consider dependency injection for core systems

### 19. Inconsistent Error Handling

- Some methods log errors, others return silently
- No centralized error reporting

### 20. Mixed Concerns in Managers

- `EventManager` both manages state AND coordinates other systems
- Should separate orchestration from state management

### 21. No Request Cancellation

- Once an LLM request starts, it cannot be cleanly cancelled
- Scene transitions could leave orphaned requests

---

## FITNESS FOR PURPOSE ASSESSMENT

| Aspect | Rating | Notes |
|--------|--------|-------|
| **Core Gameplay Loop** | ✅ Good | Interaction → LLM → Effects works well |
| **LLM Integration** | ⚠️ Adequate | Works but has edge cases |
| **Visual Effects** | ✅ Good | Smooth transitions, appropriate scaling |
| **Audio System** | ✅ Good | Clean implementation |
| **Memory Persistence** | ✅ Good | Robust JSON serialization |
| **Scene Transitions** | ⚠️ Adequate | Works but singleton issues |
| **Error Resilience** | ❌ Needs Work | Many unhandled edge cases |
| **Performance** | ⚠️ Adequate | Blocking I/O is concerning |

---

## RECOMMENDATIONS

### Priority 1 (Critical)
1. Add proper mutex/lock around `isStreaming` flag
2. Add request timeout and cancellation to LLM calls
3. Validate array bounds for whisper clips vs spook levels

### Priority 2 (High)
1. Move file I/O to async/coroutine pattern
2. Fix rich text truncation in UIChat
3. Add fog burst coroutine cancellation

### Priority 3 (Medium)
1. Centralize LLM endpoint configuration
2. Add error recovery for failed LLM requests
3. Review singleton lifecycle management

---

## CODE QUALITY METRICS

| Metric | Value | Assessment |
|--------|-------|------------|
| Total C# Files | 44 | Appropriate for scope |
| Lines of Code | ~5,500 | Manageable |
| Singleton Count | 15+ | High - consider refactoring |
| External Dependencies | 4 | Low - good |
| Test Coverage | 0% | No tests found |

---

## CONCLUSION

The **Cursed Portal** codebase demonstrates competent Unity development with appropriate architectural patterns for a small-scope project. The core experience—interacting with AI-powered spirits in a horror atmosphere—functions correctly under normal conditions.

However, **edge cases around LLM integration, concurrent operations, and scene transitions present reliability risks**. For a prototype or jam game, the current state is acceptable. For production release, the critical and high-severity issues should be addressed.

The software is **fit for purpose as a demonstration/prototype** but requires hardening for production deployment.

---

*This audit was conducted through static code analysis. Runtime testing is recommended to validate findings.*
