# MyCosts

Personal expense tracking application.

## Stack

- **Backend:** .NET 10 Web API · Vertical Slice Architecture · EF Core 10 · PostgreSQL
- **Frontend:** React 19 · TypeScript · Vite · ESLint · Prettier

## Project Structure

```
MyCosts/
├── backend/
│   ├── src/
│   │   ├── MyCosts.Domain/          # Domain entities (pure, no dependencies)
│   │   ├── MyCosts.Application/     # Feature handlers (VSA slices), references Infrastructure
│   │   ├── MyCosts.Infrastructure/  # EF Core DbContext, PostgreSQL, migrations
│   │   ├── MyCosts.Api/             # Minimal API endpoints
│   │   └── MyCosts.Migrator/        # Standalone migration runner (runs before Api in Docker)
│   └── tests/
│       ├── MyCosts.UnitTests/
│       └── MyCosts.IntegrationTests/  # TestContainers-based integration tests
├── frontend/                 # React + Vite + TypeScript
├── docker-compose.yml        # Full stack (postgres → migrator → api → frontend)
└── docker-compose.db.yml     # PostgreSQL only
```

## Architecture

**Vertical Slice Architecture** — each feature lives in its own slice under `Application/Features/{Domain}/{Feature}/`, containing the handler, request, and response in one place. Handlers inject `AppDbContext` directly (no repository layer).

Dependency flow:
```
Api → Application → Infrastructure → Domain
         ↑                ↑
    (feature handlers)  (DbContext)
```

The `Migrator` project is a separate console app that applies EF Core migrations on startup and exits. In Docker Compose, the `api` service depends on `migrator` completing successfully.

## Local Development

**Prerequisites:** .NET 10 SDK, Node.js 24+, Docker

### Backend

```bash
# Start PostgreSQL
docker compose -f docker-compose.db.yml up -d

# Apply migrations
cd backend
dotnet run --project src/MyCosts.Migrator

# Run API
dotnet run --project src/MyCosts.Api
# → http://localhost:5050
```

### Migrations

```bash
cd backend

# Add a new migration
dotnet ef migrations add <MigrationName> \
  --project src/MyCosts.Infrastructure \
  --startup-project src/MyCosts.Migrator \
  --output-dir Persistence/Migrations

# List applied migrations
dotnet ef migrations list \
  --project src/MyCosts.Infrastructure \
  --startup-project src/MyCosts.Migrator

# Remove last migration (if not yet applied)
dotnet ef migrations remove \
  --project src/MyCosts.Infrastructure \
  --startup-project src/MyCosts.Migrator
```

> `dotnet-ef` is pinned as a local tool in `.config/dotnet-tools.json`. Run `dotnet tool restore` once after cloning.

### Frontend

```bash
cd frontend
npm install
npm run dev   # http://localhost:5173
```

> Vite automatically proxies `/api/*` requests to `http://localhost:5050` — no CORS required.

### Frontend scripts

| Command | Description |
|---|---|
| `npm run dev` | Dev server with HMR |
| `npm run build` | Type-check + production build |
| `npm run typecheck` | Type-check only |
| `npm run lint` | ESLint (fails on warnings) |
| `npm run lint:fix` | ESLint with autofix |
| `npm run format` | Prettier — format all files |
| `npm run format:check` | Prettier — check without changes |
| `npm run preview` | Preview production build locally |

## Full Stack in Docker

```bash
docker compose up --build
# → Frontend: http://localhost
# → API:      http://localhost:5050
# → Postgres: localhost:5432
```

Migrations run automatically via the `migrator` service before the API starts.

## Tests

```bash
cd backend && dotnet test
```

Integration tests use TestContainers (Docker required) — each test runs in a transaction that is rolled back on teardown.
