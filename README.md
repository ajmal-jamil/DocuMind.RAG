# DocuMind RAG

**An AI-powered document question-answering system built with .NET 10, Gemini AI, PostgreSQL and pgvector.**

## Overview

DocuMind RAG is a Retrieval-Augmented Generation (RAG) application that allows users to upload documents, extract and process their content, generate vector embeddings, and ask natural-language questions about the uploaded information.

Instead of relying solely on an LLM's pretrained knowledge, DocuMind retrieves relevant information from uploaded documents and uses it as context to generate answers.

## Architecture

The solution follows Clean Architecture principles, separating presentation, business logic, infrastructure and domain concerns.

```text
DocuMind/
│
├── .gitignore
├── README.md
│
├── DocuMind.Rag.Api/
│   ├── Controllers/
│   ├── Contracts/
│   ├── Program.cs
│   └── appsettings.json
│
├── DocuMind.Application/
│   ├── Interfaces/
│   ├── Models/
│   └── Services/
│
├── DocuMind.Infrastructure/
│   ├── Persistence/
│   ├── Services/
│   └── AI/
│
└── DocuMind.Domain/
    └── Entities/
```

### Project responsibilities

| Project | Responsibility |
|---|---|
| DocuMind.Rag.Api | HTTP endpoints, controllers and API contracts |
| DocuMind.Application | Application use cases, interfaces and business orchestration |
| DocuMind.Infrastructure | Database persistence, external AI integrations and technical implementations |
| DocuMind.Domain | Core domain entities and enterprise business rules |

## Technology Stack

- **Backend:** ASP.NET Core / .NET 10
- **Language:** C#
- **AI Provider:** Google Gemini (current implementation)
- **Embeddings:** Gemini Embedding API
- **Vector Database:** PostgreSQL with pgvector
- **ORM:** Entity Framework Core
- **Architecture:** Clean Architecture
- **Document Processing:** PDF and TXT text extraction

## Current Features

- Upload PDF and TXT documents.
- Extract text from uploaded documents.
- Split extracted text into manageable chunks.
- Generate vector embeddings for document chunks.
- Store document information and embeddings in PostgreSQL.
- Perform semantic similarity searches.
- Retrieve relevant document chunks.
- Generate AI answers using retrieved document context.
- Return source information alongside generated answers.

## RAG Workflow

```text
DOCUMENT INGESTION

PDF / TXT
    |
    v
Text Extraction
    |
    v
Text Chunking
    |
    v
Embedding Generation
    |
    v
PostgreSQL + pgvector
```

```text
QUESTION ANSWERING

User Question
    |
    v
Question Embedding
    |
    v
Semantic Search
    |
    v
Retrieve Relevant Chunks
    |
    v
Gemini AI
    |
    v
Contextual Answer + Sources
```

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/documents/upload` | Upload and process a document |
| GET | `/api/documents/search` | Search documents using semantic similarity |
| GET | `/api/documents/ask` | Generate an answer using retrieved document context |

### Example

Upload a document:

```http
POST /api/documents/upload
Content-Type: multipart/form-data
```

Search documents:

```http
GET /api/documents/search?question=What is this document about?&topK=5
```

Ask a question:

```http
GET /api/documents/ask?question=What are the main findings?
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL
- pgvector PostgreSQL extension
- Google Gemini API key
- Visual Studio 2022 or another compatible .NET IDE

## Configuration

AI provider and model settings are intended to be configurable through application configuration.

Example configuration:

```json
{
  "AI": {
    "Chat": {
      "Provider": "Gemini",
      "BaseUrl": "https://generativelanguage.googleapis.com/v1beta",
      "Model": "YOUR_CHAT_MODEL",
      "Temperature": 0.2,
      "MaxOutputTokens": 2048
    },
    "Embedding": {
      "Provider": "Gemini",
      "BaseUrl": "https://generativelanguage.googleapis.com/v1beta",
      "Model": "YOUR_EMBEDDING_MODEL",
      "Dimensions": 768
    }
  }
}
```

**Security:** Never commit API keys, passwords or other credentials. Use .NET User Secrets for local development and environment variables or a secret manager for production.

## Getting Started

1. Clone the repository.
2. Open `DocuMind.Rag.Api.slnx` in Visual Studio.
3. Configure PostgreSQL connection settings.
4. Install and enable pgvector in your PostgreSQL database.
5. Configure your Gemini API credentials securely.
6. Configure the AI models in application settings.
7. Apply Entity Framework Core migrations.
8. Build and run `DocuMind.Rag.Api`.
9. Test the endpoints using Swagger or Postman.

## Future Roadmap

- [ ] Provider-independent LLM integration.
- [ ] Configuration-driven AI provider and model selection.
- [ ] Support for multiple LLM providers.
- [ ] Support for multiple embedding providers.
- [ ] Conversation-aware RAG (multi-turn conversations).
- [ ] Conversation and message persistence.
- [ ] Improved retrieval and reranking.
- [ ] Authentication and authorization.
- [ ] Document management and deletion.
- [ ] Automated testing and evaluation.

## Project Goal

The long-term goal of DocuMind RAG is to build a flexible, extensible and production-oriented RAG platform using modern .NET technologies and provider-independent AI abstractions.
