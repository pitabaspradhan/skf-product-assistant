# SKF Product Assistant (Mini)

An Agentic AI assistant built using **.NET 10**, **Azure Functions (Isolated Worker)**, and **Microsoft Semantic Kernel (C#)**.

The system answers product datasheet questions and captures user feedback while maintaining conversational state.  
All responses are grounded strictly in local JSON datasheets to prevent hallucinations.

---

# 🧠 Solution Overview

## Key Capabilities

- Natural language product queries
- Datasheet-grounded answers (JSON only)
- Semantic Kernel function calling
- Conversational state across turns
- Feedback capture & linkage
- Redis caching with memory fallback
- Swagger (OpenAPI) testing UI
- Secure configuration via environment variables

---

# 🏗️ Architecture

HTTP Azure Function (.NET 10 Isolated)
        ↓
Intent Orchestrator
   ↙             ↘
Q&A Agent     Feedback Agent
   ↓
Semantic Kernel
   ↓
Datasheet Plugin (Function Calling)
   ↓
Local JSON Datasheets

---

# 📂 Project Structure

Skf.ProductAssistant/
│
├── Skf.ProductAssistant.sln
├── .gitignore
├── Dockerfile (optional)
├── README.md
│
├── src/
│   ├── FunctionApp/
│   ├── Agents/
│   ├── Plugins/
│   ├── Orchestration/
│   ├── Services/
│   ├── State/
│   └── Models/
│
└── tests/

---

# ⚙️ Prerequisites / Dependencies

Install the following:

- .NET 10 SDK  
- Azure Functions Core Tools v4  
- Visual Studio 2026 / VS Code  
- Docker Desktop (optional)  
- Redis (optional — memory fallback supported)

Verify:

dotnet --version

---

# 🔐 Configuration

Create:

local.settings.json

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AOAI_ENDPOINT": "<your-endpoint>",
    "AOAI_KEY": "<your-key>",
    "AOAI_DEPLOYMENT": "<deployment-name>",
    "REDIS_CONNECTION": "<redis-connection-string>"
  }
}

⚠️ Do NOT commit this file.

---

# 🚀 Running the Solution

## Visual Studio

1. Open solution  
2. Set FunctionApp as Startup Project  
3. Press F5  

## CLI

func start

Endpoint:

http://localhost:7079/api/product-assistant

---

# 🌐 Swagger UI

http://localhost:7079/api/swagger/ui

---

# 🧪 Example Request

{
  "conversationId": "demo-1",
  "message": "What is the width of 6205?"
}

Response:

The width of the 6205 bearing is 15 mm.

---

# 🛡️ Hallucination Control

- Datasheet-only answers  
- No generated values  
- Abstains if missing  

---

# 🧠 Semantic Kernel Usage

- Prompt extraction  
- Function calling  
- Intent classification  
- Fallback logic  

---

# 🗄️ State & Caching

| Component | Purpose |
|-----------|---------|
| ConversationState | Context tracking |
| RedisStateStore | Persistent cache |
| MemoryStateStore | Fallback |
| HybridStateStore | Combined strategy |

---

# 🧪 Unit Tests

dotnet test

---

# 🔒 Security

- Env-based secrets  
- .gitignore protected  
- No hardcoded keys  

---

# 🤖 AI‑Assisted Review Evidence

## Tools Used

- ChatGPT (GPT‑5.2)  
- GitHub Copilot  
- Semantic Kernel docs assistant  

## Improvements Applied

- Datasheet grounding enforced  
- Plugin function calling added  
- Conversational state implemented  
- Redis fallback caching  
- Secure config handling  

Outcome:

- Improved reliability  
- No hallucinations  
- Production‑ready architecture  

---

# ✍️ Author

Pitabas Pradhan  
Senior Engineering Leader — Cloud, Microservices & AI Platforms
