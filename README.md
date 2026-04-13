# MyCosts

Personal expense tracking application.

## Stack

- **Backend:** .NET 10 Web API · Clean Architecture · EF Core · PostgreSQL
- **Frontend:** React 19 · TypeScript · Vite · ESLint · Prettier

## Project Structure

```
MyCosts/
├── backend/                  # .NET solution (Clean Architecture)
│   ├── src/
│   │   ├── MyCosts.Domain/
│   │   ├── MyCosts.Application/
│   │   ├── MyCosts.Infrastructure/
│   │   └── MyCosts.Api/
│   └── tests/
│       ├── MyCosts.UnitTests/
│       └── MyCosts.IntegrationTests/
├── frontend/                 # React + Vite + TypeScript
├── docker-compose.yml        # Full stack
└── docker-compose.db.yml     # PostgreSQL only
```

## Local Development

**Prerequisites:** .NET 10 SDK, Node.js 24+, Docker

### Backend

```bash
# Start PostgreSQL
docker compose -f docker-compose.db.yml up -d

# Run API (listens on http://localhost:5050)
cd backend && dotnet run --project src/MyCosts.Api
```

### Frontend

```bash
cd frontend
npm install
npm run dev   # http://localhost:5173
```

> Vite automatically proxies `/api/*` requests to `http://localhost:5050` - no CORS required.

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

## Tests

```bash
cd backend && dotnet test
```
