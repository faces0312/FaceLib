# FaceLib (com.face.facelib)

Reusable utilities for Unity as a UPM package. Install via Git (UPM) or as a Git submodule.

## Installation

### Option 1: Git Submodule (recommended for pinning)

git submodule add https://github.com/faces0312/FaceLib.git Packages/com.face.facelib

git submodule update --init --recursive

### Option 2: UPM Git URL
Add to `Packages/manifest.json` dependencies:

{
  "dependencies": {
    "com.face.facelib": "https://github.com/faces0312/FaceLib.git#v0.1.0"
  }
}

## Usage
See `Samples~/Example` for a minimal usage example.

Basic API:

using FaceLib;

// Logs a standardized message
FaceLogger.Log("Hello FaceLib!");

## Requirements
- Unity 2020.3+

## Development
- Runtime code in `Runtime/`
- Editor-only code in `Editor/`
- Tests in `Tests/` (Unity Test Framework)

### Run Tests
Use Unity Test Runner (Edit > PlayMode/Editor Tests).

## Versioning
- Semantic Versioning. Tags like `v0.1.0` match `package.json`.

## License
MIT
