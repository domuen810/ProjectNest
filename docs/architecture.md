# ProjectNest Architecture

## Architecture Style

ProjectNest uses a **Modular Monolith** architecture.

The system is deployed as a single application while maintaining clear logical boundaries between the main business modules:

- Identity
- Projects
- Surveys
- Drawings
- Production

A Modular Monolith is appropriate for the current system because the business workflows are closely related and the MVP does not require independent deployment or scaling of individual modules.

This approach keeps development, deployment and operations relatively simple while still providing clear module boundaries and allowing the architecture to evolve if future requirements justify additional separation.

## Backend Layers

The backend is organised into four main layers:

- **API** — Handles HTTP requests and responses, authentication, authorization and API endpoints.
- **Application** — Coordinates application use cases, validation and business workflows.
- **Domain** — Contains the core business concepts and business rules.
- **Infrastructure** — Handles technical concerns such as EF Core, PostgreSQL, ASP.NET Core Identity and file storage.

The dependency direction is:

API → Application → Domain

Infrastructure provides technical implementations required by the application and domain while keeping infrastructure-specific concerns out of the core business logic.

This separation keeps business rules independent from HTTP, database and storage implementation details, making the system easier to understand, test and maintain.

## System Structure

ProjectNest consists of the following main components:

- **Frontend** — React and TypeScript single-page application used by internal users.
- **Backend** — ASP.NET Core Web API containing the application and business logic.
- **Database** — PostgreSQL stores structured application data.
- **File Storage** — Object storage stores project files such as drawing PDFs, CAD files and survey documents.

The main communication flow is:

Frontend → REST API → Backend → PostgreSQL / File Storage

The frontend does not access the database or file storage directly. Business operations are handled through the backend API.
```
Browser
   │
   ▼
React + TypeScript
Single-Page Application
   │
   │ REST / JSON
   ▼
ASP.NET Core Backend
Modular Monolith
   │
   ├── API
   ├── Application
   ├── Domain
   └── Infrastructure
          │
          ├── PostgreSQL
          └── File Storage
```

## Module Responsibilities

The backend is divided into business modules with clear responsibilities:

- **Identity** — User identity, authentication and role-based access.
- **Projects** — Core project information, project status and responsible Designer assignment.
- **Surveys** — Site Survey Work Orders, survey assignments and survey records.
- **Drawings** — Drawing sets, revisions, drawing changes and identification of the current production drawing.
- **Production** — Production orders, production stages and production progress.

Modules may collaborate as part of a business workflow, but each module remains responsible for its own business rules and data.

## Data and File Storage

ProjectNest uses **PostgreSQL** as the primary relational database for structured application data.

Large project files, such as drawing PDFs, CAD files and survey documents, are stored separately in object storage. PostgreSQL stores the metadata and references required to associate these files with the relevant business records.

This approach avoids storing large binary files directly in the relational database while keeping file information connected to projects, surveys and drawings.

## Security and Access

ProjectNest is an internal system and requires authenticated access.

Authentication is handled using ASP.NET Core Identity. Authorization is based on user roles and, where required, resource-specific rules.

The initial access model follows these principles:

- Users must be authenticated before accessing protected project information.
- Role-based access control is used to restrict actions according to business responsibilities.
- Users should only be given the permissions required for their role.
- Sensitive configuration and secrets must not be stored directly in source code.
- Access to project files must be controlled through the application rather than exposed publicly.

## API Style

ProjectNest uses RESTful HTTP APIs with JSON for communication between the frontend and backend.

The API follows these principles:

- Resources are represented using clear and consistent URLs.
- Standard HTTP methods such as GET, POST, PUT and DELETE are used according to the operation being performed.
- Appropriate HTTP status codes are returned for successful and failed requests.
- API request and response models are separated from internal domain entities where appropriate.
- Validation errors and application errors are returned using a consistent error format.
- OpenAPI / Swagger is used to document and explore the API during development.

## CI/CD

ProjectNest uses GitHub Actions to automate build and test checks as part of the development workflow.

The CI/CD process follows these principles:

- Code changes are developed in feature branches and reviewed through pull requests.
- The application is automatically built when relevant changes are pushed or submitted for review.
- Automated tests run as part of the CI process.
- Changes should only be merged when required build and test checks pass.
- Deployment will be automated through the CI/CD pipeline once the deployment environment is established.
- Environment-specific configuration and secrets are managed outside the source code.

## Deployment Approach

ProjectNest is designed to be deployed to a cloud environment rather than run only on a developer's local machine.

The deployment will include:

- The React frontend.
- The ASP.NET Core backend.
- A PostgreSQL database.
- Object storage for project files.

Azure is the preferred cloud platform for the initial deployment.

Development and production environments will use separate configuration and data. The exact Azure services and deployment topology will be selected when deployment requirements are finalised.

## Observability

ProjectNest includes basic observability so that application behaviour and failures can be understood after deployment.

The initial approach includes:

- Structured application logging in the ASP.NET Core backend.
- Logging of important errors and operational events.
- Production logs must not expose passwords, authentication secrets or other sensitive information.
- Cloud monitoring may be added through Azure Application Insights or an equivalent service during deployment.

## Architecture Constraints

The initial ProjectNest architecture intentionally avoids unnecessary distributed-system complexity.

The MVP does not require:

- Microservices
- Message queues
- Event Sourcing
- Full CQRS
- Kubernetes
- Redis

These technologies should only be introduced if future requirements create a clear technical need, such as independent scaling, asynchronous processing or separate deployment of system components.

The architecture should remain as simple as possible while meeting the current business and engineering requirements.

## Architecture Evolution

ProjectNest is designed to evolve when real business or technical requirements justify architectural changes.

Future changes may include separating resource-intensive background processing from the main application, introducing asynchronous processing, or independently scaling specific components.

Such changes should be based on observed requirements and documented through Architecture Decision Records (ADRs) rather than introduced in advance.

## Architecture Overview

```text
Browser
   │
   ▼
React + TypeScript SPA
   │
   │ HTTPS / REST / JSON
   ▼
ASP.NET Core Web API
   │
   ├── API
   │     ↓
   ├── Application
   │     ↓
   ├── Domain
   │
   └── Infrastructure
          │
          ├── EF Core
          │     ↓
          │  PostgreSQL
          │
          └── Object Storage
                ↓
          PDF / CAD / Survey Files


Backend Business Modules

┌──────────┐
│ Identity │
└──────────┘

┌──────────┐
│ Projects │
└──────────┘
      │
      ├──────────────┐
      ▼              ▼
┌──────────┐    ┌──────────┐
│ Surveys  │    │ Drawings │
└──────────┘    └──────────┘
                     │
                     ▼
                ┌────────────┐
                │ Production │
                └────────────┘