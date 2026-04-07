# Ava — AI Sign Language Avatar

> An AI-powered 3D avatar that converts spoken and written English into **Indian Sign Language (ISL)** animations in real time, built to bridge communication gaps for the hearing-impaired community.

![Ava Demo](Ava.gif)

---

## What is Ava?

Ava is an intelligent, animated humanoid avatar designed to make communication more accessible. You speak or type in English — Ava understands, responds intelligently, and signs back in Indian Sign Language using fluid 3D animations.

Built on a **VRM-based humanoid model** inside Unity 2022, Ava combines the power of modern AI (speech recognition, large language models) with a custom machine learning pipeline for authentic ISL gesture generation.

---

## Features

- 🎤 **Voice Input** — Speak naturally and Ava listens using real-time speech-to-text
- 🧠 **AI Brain** — Powered by Google Gemini 1.5 Flash for intelligent, context-aware responses
- 🤟 **Sign Language Output** — Animated ISL gestures rendered on a fully rigged 3D humanoid avatar
- 💬 **Text Input** — Type your message and send it directly if you prefer
- 👄 **LipSync** — Avatar lip movements are synchronized with speech output
- 🔄 **Dual Architecture** — Rule-based animation system for fast prototyping + ML-driven bone animation for authentic signing

---

## Demo

![Ava in action](Ava.gif)

> The demo above shows Ava's voice interaction and animation system running inside Unity. Ava listens via the **LISTEN** button, processes the input through Gemini AI, and responds with synchronized avatar animations.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Game Engine | Unity 2022.3 LTS |
| Avatar Format | VRM Humanoid (UniVRM) |
| Speech-to-Text | Whisper AI (ggml-base) |
| AI / LLM | Google Gemini 1.5 Flash |
| ML Pipeline | Python, MediaPipe, TensorFlow |
| Animation | Mixamo + Custom ML Bone Controller |
| LipSync | uLipSync (Unity Package) |

---

## How It Works

Ava has two parallel systems working together:

### 1. Conversation Layer
```
User speaks / types
        ↓
Whisper AI transcribes speech to text
        ↓
Gemini 1.5 Flash generates a response
        ↓
Ava animates and lip-syncs the reply
```

### 2. Sign Language Layer
```
Input text / response
        ↓
Word mapped to ISL gesture
        ↓
ML-generated bone rotation data (CSV)
        ↓
PoseReceiver.cs applies XYZ rotations to Ava's skeleton
        ↓
Ava signs the word in real time
```

---

## ML Pipeline

The sign language animation system is powered by a custom Python pipeline:

1. **Dataset** — ISL word videos with synchronized audio and labels
2. **Pose Extraction** — MediaPipe extracts skeletal keypoints from each video and saves them as `.npy` files
3. **Model Training** — A temporal model is trained to understand the motion sequence of each sign over time
4. **Output** — Bone rotation sequences are exported as `.csv` files containing XYZ coordinates for each joint
5. **Unity Integration** — A custom `PoseReceiver.cs` script reads the CSV data and applies it directly to Ava's skeleton, bypassing Unity's default Animator for full ML control

---

## Project Structure

```
Ava_ML_Version/
│
├── poseExtraction.py          # MediaPipe keypoint extraction
├── trainModel.py              # Temporal model training
├── Datasets/                  # Raw ISL video dataset
│
├── Assets/
│   ├── ML_Research_Data/      # Generated .npy and .csv files
│   ├── Scenes/
│   │   ├── Ava-Main           # Main scene (Mixamo animations)
│   │   └── Ava_Research_Sandbox  # ML bone animation testing scene
│   ├── Scripts/
│   │   ├── GeminiManager.cs   # Gemini API integration
│   │   ├── WhisperManager.cs  # Speech-to-text handler
│   │   ├── PoseReceiver.cs    # ML bone rotation controller
│   │   ├── BoneController.cs  # Skeleton mapping
│   │   └── LipSyncManager.cs # LipSync handler
│   └── Ava.Avatar/            # VRM humanoid model and assets
```

---

## Getting Started

### Prerequisites

- Unity 2022.3 LTS
- Python 3.9+
- A Google Gemini API key

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/daniyaa1/Ava_ML_Version.git
cd Ava_ML_Version
```

**2. Install Python dependencies**
```bash
pip install mediapipe tensorflow numpy opencv-python
```

**3. Add manual dependencies** *(required — too large for GitHub)*

These files must be added manually before running:

| File | Location |
|---|---|
| `ggml-base.bin` (Whisper model) | `Assets/StreamingAssets/` |
| `gemini_key.json` (your API key) | `Assets/StreamingAssets/` |

Download Whisper model: [ggml-base.bin](https://huggingface.co/ggerganov/whisper.cpp)

**4. Open in Unity**
- Open Unity Hub
- Add the project folder
- Open with Unity 2022.3 LTS
- Open the `Ava-Main` scene and hit Play

---

## Running the ML Pipeline

To extract poses from your own ISL dataset:
```bash
python poseExtraction.py
```

To train the model:
```bash
python trainModel.py
```

The output CSV files will be saved to `Assets/ML_Research_Data/` and automatically picked up by Unity.

---

## Current Status

| Feature | Status |
|---|---|
| Voice input via Whisper AI | ✅ Complete |
| Gemini AI conversation | ✅ Complete |
| LipSync | ✅ Complete |
| Mixamo animation system | ✅ Complete |
| ML pose extraction pipeline | ✅ Complete |
| ML bone rotation in Unity | ✅ Complete |
| Full ISL word library | 🔄 In Progress |
| Dynamic sign loader (runtime) | 🔄 In Progress |
| Natural sign transition blending | 🔄 In Progress |

---

## Roadmap

- [ ] Expand ISL word library with batch-trained gesture sequences
- [ ] Build a dynamic runtime loader so Ava selects signs based on live input
- [ ] Smooth transitions between consecutive signs using interpolation
- [ ] Add sentence-level ISL grammar support
- [ ] Deploy as a standalone accessibility application

---

## Why Ava?

Over **63 million people** in India have significant hearing loss. Most digital interfaces offer no sign language support whatsoever. Ava is a step toward changing that — making AI-powered, real-time ISL communication accessible to everyone.

---

## Contributing

Contributions are welcome, especially in the areas of:
- ISL dataset expansion
- ML model accuracy improvements
- Unity animation blending
- UI/UX improvements

```bash
# Fork the repo, create your branch, and submit a PR
git checkout -b feature/your-feature-name
```

---

## License

MIT License — feel free to use, modify, and build on this project.

---

## Author

**Daniya Ishteyaque**
[GitHub](https://github.com/daniyaa1) • [LinkedIn](https://www.linkedin.com/in/daniya-ishteyaque-8a4816316)

> *Built with the belief that technology should be accessible to everyone.*
