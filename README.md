# ProjectNest

ProjectNest is a portfolio software engineering project based on a real-world retail fit-out workflow.

It is designed as an internal operations system for teams involved in the design, manufacturing, and delivery of retail stores and counters. The system aims to provide a single source of truth for project progress, site surveys, drawing revisions, and factory production information.

## Problem

Retail fit-out projects involve information moving across design, engineering, factory, and management teams.

In the original workflow, communication may happen through email, phone, messaging apps, and face-to-face discussions. Important project information and drawing revisions can therefore become fragmented across different channels and local files.

ProjectNest is intended to make critical operational information easier to track and answer three key questions:

- Where is the project now?
- Which information or drawing should be used now?
- What changed?

Existing communication channels are not replaced. ProjectNest acts as the central source for important project information and history.

## Current Scope

The MVP focuses on the workflow from project creation through factory production, including:

- Projects and stores
- Site survey work orders and survey visits
- Drawing stages and revision history
- Production orders
- Factory production stage progress
- Internal users and roles
- File metadata and document relationships

Later phases may extend into logistics, site installation, acceptance, and maintenance workflows.

## Architecture

ProjectNest uses a Modular Monolith architecture with explicit layer boundaries, following Clean Architecture principles where appropriate.

The backend currently contains four .NET projects:

- `ProjectNest.Api` — ASP.NET Core Web API and application entry point
- `ProjectNest.Application` — application use cases and orchestration
- `ProjectNest.Domain` — core business concepts and business rules
- `ProjectNest.Infrastructure` — persistence and external technical concerns

Current dependency direction:

```text
Api → Application
Api → Infrastructure
Infrastructure → Application
Application → Domain
Domain → nothing
```

The system is organised around business areas including Projects, Surveys, Drawings, Production, and Identity.

## Technology Stack

### Backend

- C#
- .NET 10
- ASP.NET Core Web API
- OpenAPI
- Entity Framework Core — planned
- PostgreSQL — planned

### Frontend

- React — planned
- TypeScript — planned

### Engineering

- Git and GitHub
- Feature branch and Pull Request workflow
- Automated testing — planned
- GitHub Actions CI/CD — planned
- Cloud deployment — planned

## Repository Structure

```text
ProjectNest/
├── docs/
│   ├── architecture.md
│   ├── business-lifecycle.md
│   ├── domain-and-data-model.md
│   ├── requirements.md
│   ├── technical-overview.md
│   └── adr/
├── src/
│   ├── ProjectNest.Api/
│   ├── ProjectNest.Application/
│   ├── ProjectNest.Domain/
│   └── ProjectNest.Infrastructure/
├── README.md
└── ProjectNest.slnx
```

## Documentation

Detailed project documentation is maintained in the `docs` directory:

- [Business Lifecycle](docs/business-lifecycle.md) — real-world business workflow and project lifecycle
- [Requirements](docs/requirements.md) — system requirements and MVP scope
- [Architecture](docs/architecture.md) — architecture decisions and system boundaries
- [Domain and Data Model](docs/domain-and-data-model.md) — domain concepts, relationships, rules, and logical data model
- [Technical Overview](docs/technical-overview.md) — technical direction and implementation context

## Development Status

Completed:

- Business workflow analysis
- Requirements and MVP definition
- Architecture design
- Domain and logical data modelling
- Backend solution and project structure
- Initial dependency boundaries
- ASP.NET Core API foundation

Next:

- Database foundation with Entity Framework Core and PostgreSQL
- Initial domain implementation
- API use cases
- Automated testing
- Frontend
- CI/CD and deployment
