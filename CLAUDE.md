# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Flip is an Umbraco CMS package that allows changing content nodes from one document type to another with property mapping. It targets Umbraco 18 on .NET 10. The package is distributed as `Flip.Umbraco` on NuGet.

## Build Commands

### Frontend (run from `src/Flip/Client/`)
```bash
npm ci                  # install dependencies (Node 24+, npm 11+)
npm run build           # TypeScript compile + Vite build → outputs to ../wwwroot/
npm run watch           # Vite build in watch mode
npm run generate:api    # regenerate OpenAPI client from running backend
```

### Backend (run from repo root)
```bash
dotnet restore Flip.sln --locked-mode
dotnet build Flip.sln --configuration Release
dotnet pack Flip.sln --configuration Release --no-build --output ./artifacts
```

### Full build order (CI mirrors this)
1. `npm ci` + `npm run build` in `src/Flip/Client/`
2. `dotnet restore` + `dotnet build` at repo root

Frontend must be built first — the .NET build expects compiled assets in `src/Flip/wwwroot/`.

## Running Locally

Use the example site at `Examples/Flip.Site/` which references the Flip project directly. Run with `dotnet run` from that directory. The Umbraco backoffice is available at the configured port.

## OpenAPI Client Generation

The frontend API client in `src/Flip/Client/generated/` is auto-generated — do not edit these files manually. To regenerate:
1. Start the example site so the API is available at `http://localhost:23901`
2. Run `npm run generate:api` from `src/Flip/Client/`

The OpenAPI schema is served from `/umbraco/openapi/flip-management.json`.

## Testing

No automated test projects exist. Test changes manually against the example site.

## Architecture

**C# backend** (`src/Flip/`): Razor Class Library with three API controllers under `Api/Controllers/` that handle document type conversion, content model retrieval, and permitted type listing. `FlipService` contains the core conversion logic. DI is registered via `Composer.cs` using Umbraco's `IComposer` pattern. The API is documented with OpenAPI under the `flip-management` group.

**TypeScript frontend** (`src/Flip/Client/`): Umbraco 18 backoffice extension using Lit web components. Entry point is `src/index.ts` which registers manifests and configures the generated API client. The extension is organized into:
- `actions/` — entity action that adds "Change document type" to the content node context menu
- `modal/` — the modal UI for selecting target type and mapping properties
- `permissions/` — granular permission definitions
- `lang/` — localization strings

All manifests are aggregated in `src/manifests.ts` and registered in the entry point.

**Integration**: Vite builds the frontend to `src/Flip/wwwroot/`, which the .csproj packs as `App_Plugins/Flip/` in the NuGet package via `StaticWebAssetBasePath`.

## Key Conventions

- API routes are prefixed with `/flip/management/api` (see `ApiConstants.cs`)
- All controllers inherit from `FlipControllerBase` which sets the route/area/group
- Versioning uses Nerdbank.GitVersioning — version height is computed from git history
- The `umbraco-package.json` version is auto-updated at build time from the git version
- Branch naming: `v{major}/dev` per Umbraco major version (this branch targets Umbraco 18)
- External Umbraco packages are excluded from the Vite bundle (`@umbraco` is external)
