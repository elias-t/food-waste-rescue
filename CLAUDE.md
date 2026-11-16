# CLAUDE.md

## Project
Food Waste Rescue: donors (shops, restaurants) post surplus food listings; people nearby find them on a map and claim them before they expire. Deployed on Azure (App Service API + Static Web Apps frontend).

- **Backend** (`backend/`): ASP.NET Core, .NET 8 (`net8.0`), EF Core + SQL Server, ASP.NET Identity + JWT auth, CQRS with MediatR, FluentValidation, Hangfire background jobs (separate Worker project, Azure WebJob)
- **Frontend** (`frontend/`): React 19 + TypeScript, Vite, TanStack React Query, axios, react-router, react-hook-form + zod, Leaflet maps, Tailwind
- **Tests**: none yet — being added under `backend/tests/` (xUnit)

## Layout
- `backend/FoodWasteRescue.slnx` — the solution
- `backend/src/FoodWasteRescue.Domain/` — entities, enums, domain events; references nothing
- `backend/src/FoodWasteRescue.Application/` — MediatR commands/queries + handlers, validators, interfaces (`Common/Interfaces/`), `Result<T>` pattern
- `backend/src/FoodWasteRescue.Infrastructure/` — `AppDbContext`, migrations, Identity/JWT/email services
- `backend/src/FoodWasteRescue.API/` — controllers (thin, send MediatR requests), `Program.cs`, middleware
- `backend/src/FoodWasteRescue.Worker/` — Hangfire job host
- `frontend/src/` — `pages/`, `components/`, `hooks/` (one hook per API call), `services/api.ts` (axios instance), `types/api.ts` (mirrors backend DTOs)

## Commands
| What | Command |
|---|---|
| Build backend | `cd backend && dotnet build FoodWasteRescue.slnx` |
| Test backend | `cd backend && dotnet test FoodWasteRescue.slnx` |
| Run API | `cd backend && dotnet run --project src/FoodWasteRescue.API` (listens on :7089) |
| Frontend install / dev | `cd frontend && npm ci && npm run dev` (:5173) |
| Frontend checks | `cd frontend && npm run typecheck && npm run lint` |
| New migration | `cd backend && dotnet ef migrations add <Name> --project src/FoodWasteRescue.Infrastructure --startup-project src/FoodWasteRescue.API` |

The hooks in `.claude/` format each file you edit and run build + tests when you finish. Their failures are real failures: fix them, don't work around them.

## Rules
- Dependency direction: Domain ← Application ← Infrastructure/API. Domain references nothing; Application must not reference Infrastructure or API.
- Controllers stay thin: validate nothing there, just send the MediatR command/query and map `Result<T>` to the HTTP response.
- Handlers return `Result<T>` failures for expected errors; exceptions are for the unexpected (handled by `ExceptionHandlingMiddleware`).
- Use `async`/`await` end to end with `CancellationToken`; never `.Result` or `.Wait()`.
- Every command gets a FluentValidation validator. Every new endpoint needs an explicit auth decision (`[Authorize]` or `[AllowAnonymous]`).
- Never edit an applied migration; add a new one. Never run `dotnet ef database update` unless asked.
- Frontend: function components + hooks only, strict TypeScript, no `any`. One custom hook per API operation (see `hooks/`). When a backend DTO changes, update `frontend/src/types/api.ts` in the same change.
- Every behaviour change gets a test. Bug fixes start with a failing test that reproduces the bug.
- Don't add NuGet/npm packages without asking. Never read or print `.env` files, user secrets, JWT keys or connection strings (`appsettings.Development.json` is gitignored and off limits).

## Workflow
- Anything bigger than a small fix: use the `/feature` skill (spec in `docs/specs/` first).
- Work on a branch (`feat/...`, `fix/...`, `chore/...`), never on `main`. Run the `reviewer` agent before committing.
- Don't push or open PRs unless asked.
