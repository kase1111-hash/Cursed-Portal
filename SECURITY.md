# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 1.0.x   | :white_check_mark: |

## Reporting a Vulnerability

We take security seriously. If you discover a security vulnerability in Cursed Portal, please report it responsibly.

### How to Report

1. **Do not** open a public GitHub issue for security vulnerabilities
2. Email the maintainer directly or use GitHub's private vulnerability reporting feature
3. Include:
   - Description of the vulnerability
   - Steps to reproduce
   - Potential impact
   - Any suggested fixes (optional)

### What to Expect

- **Acknowledgment**: Within 48 hours
- **Initial Assessment**: Within 7 days
- **Resolution Timeline**: Depends on severity, typically 30-90 days

### Scope

Security concerns relevant to this project include:

#### High Priority
- **LLM Prompt Injection**: Malicious prompts that could manipulate spirit behavior
- **File System Access**: Unauthorized read/write to StreamingAssets or persistent data
- **Code Injection**: Vulnerabilities in JSON parsing or external data loading

#### Medium Priority
- **Memory Corruption**: Issues with SpiritMemory persistence
- **Build Pipeline**: Security of the YAML automation manifest

#### Lower Priority
- **Local-only Issues**: As this runs locally with local LLM, network-based attacks are less relevant

### Out of Scope

- Vulnerabilities in Unity itself (report to Unity)
- Issues with LLMUnity package (report to that project)
- Ollama/llama.cpp vulnerabilities (report to those projects)

## Security Best Practices for Users

### LLM Configuration
- Run Ollama on localhost only (default: `http://localhost:11434`)
- Do not expose the LLM endpoint to the network
- Use trusted model files from verified sources

### Data Privacy
- Spirit memory is stored locally in `Application.persistentDataPath`
- No data is transmitted externally by default
- Review custom story files before loading

### Build Security
- Verify downloaded builds from trusted sources
- Check file hashes if provided
- Review StreamingAssets content before running

## Acknowledgments

We appreciate responsible disclosure and will acknowledge security researchers who help improve this project (with permission).
