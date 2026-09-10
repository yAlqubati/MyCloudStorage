# MyCloudStorage

A personal cloud file‑storage backend built with ASP.NET Core 8, PostgreSQL, JWT auth, and chunked uploads.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Configuration](#configuration)
- [Setup & Run](#setup--run)
- [API Documentation](#api-documentation)
- [Deploying to Production](#deploying-to-production)
- [License](#license)

---

## Features

- **Authentication** — Register, login, email verification, password reset, refresh tokens
- **Files** — Chunked upload, streaming download, rename, move, delete
- **Folders** — Nested folders with full CRUD
- **Sharing** — Share files per‑user (View / Download), expiration, "shared with me", revoke
- **Storage** — Per‑user quotas with live usage tracking
- **Validation** — Extension, MIME type, and magic byte verification
- **Security** — httpOnly cookies, rate limiting, CORS

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| Database | PostgreSQL + Entity Framework Core |
| Auth | ASP.NET Identity + JWT Bearer |
| File Storage | Local disk (swappable via `IStorageService`) |
| Email | SMTP |

---

## Architecture

```
Controllers (thin HTTP layer)
        │
        ▼
Services (business logic) ──► IStorageService (local disk now; swappable to cloud)
        │
        ▼
Repositories (EF Core access)
        │
        ▼
PostgreSQL
```

Controllers stay thin; business logic lives in services and data access in repositories. EF Core migrations are applied automatically on startup. File storage is abstracted behind `IStorageService`, so the backend can switch from local disk to any cloud provider (S3, Azure Blob, Oracle Object Storage) by implementing one interface and changing a single registration in `Program.cs`.

**Project layout**
```
MyCloudStorage/
├── Controllers/          # HTTP layer
├── Application/          # Interfaces, Services, Mapping
├── Domain/Entities/      # EF entities + enums
├── Configuration/        # Typed settings (Storage, Email, Cors)
├── DTOs/                 # Request/response contracts
├── Infrastructure/       # Email, storage implementations
├── Repositories/         # EF Core data access
├── Data/                 # ApplicationDbContext + migrations
├── Exceptions/           # Custom exceptions + global handler
├── Dockerfile            # multi-stage (sdk:8 → aspnet:8), EXPOSE 8080
└── appsettings*.json     # config (gitignored; use appsettings.Example.json)
```

---

## Configuration

All behavior is controlled from `appsettings.json` (copy `appsettings.Example.json` → `appsettings.Development.json` for local dev; `appsettings.json` / `*.Development.json` are gitignored). Secrets come from environment variables loaded via DotNetEnv.

### Storage

```json
"Storage": {
  "BasePath": "uploads",
  "TempPath": "uploads/temp",
  "DefaultUserQuotaBytes": 3221225472, // 3 GB per user
  "AllowedExtensions": [".jpg", ".png", ".pdf", "..."],
  "AllowedMimeTypes": ["image/jpeg", "image/png", "application/pdf", "..."]
}
```

### Environment Variables

| Variable | Description |
|---|---|
| `DB_PASSWORD` | PostgreSQL password (substituted into the connection string where `%DB_PASSWORD%` appears) |
| `JWT_KEY` | JWT signing key (local dev). Docker uses `JWT_SECRET`, mapped to `JWT_KEY`. |
| `JWT_ISSUER` | JWT issuer (API domain) |
| `JWT_AUDIENCE` | JWT audience (frontend domain) |
| `SMTP_HOST` | SMTP server hostname |
| `SMTP_USERNAME` | SMTP login |
| `SMTP_KEY` | SMTP password / API key |

Generate a strong JWT key with `openssl rand -base64 64`.

---

## Setup & Run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL](https://www.postgresql.org/) 14+
- Docker (recommended — Compose runs the API + Postgres together)

### Docker Setup (recommended)

```bash
cp .env.example .env          # fill in your values
mkdir -p MyCloudStorage/uploads/temp
docker compose up -d --build
```

| Service | URL |
|---|---|
| API | http://127.0.0.1:5000 |
| Swagger UI | http://127.0.0.1:5000/swagger |
| PostgreSQL | 127.0.0.1:5432 (`my_cloud_storage_db` / `postgres`) |

Follow the logs:
```bash
docker compose logs -f api
```

### Run Locally (.NET)

```bash
cp .env.example .env          # fill in your values
cp MyCloudStorage/appsettings.Example.json MyCloudStorage/appsettings.Development.json
cd MyCloudStorage
dotnet ef database update     # optional — migrations also auto-apply on startup
dotnet run
```

API: `https://localhost:5001` · Swagger: `https://localhost:5001/swagger`

---

## API Documentation

The complete, interactive API reference is in **Swagger UI**:

- Local (.NET): `https://localhost:5001/swagger`
- Docker: `http://127.0.0.1:5000/swagger`

Auth is cookie based: login issues an `accessToken` (10 min) and `refreshToken` (7 days) as `HttpOnly` / `Secure` / `SameSite=None` cookies. Protected endpoints also accept `Authorization: Bearer <access-token>`. Endpoints cover auth, folders (`api/folders`), files (`api/files`), chunked upload (`api/upload/start`, `api/upload/chunk`, `api/upload/{sessionId}`), and sharing (`api/share`).

---

## License

MIT License — see [LICENSE](LICENSE) for details.