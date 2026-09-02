# ProjectNest

## Technical Overview

ProjectNest is an internal web application for managing retail fit-out
projects for cosmetic brands. It is intended to provide a
single source of truth for critical project information across design,
engineering and factory teams, with particular focus on project status,
drawing management and production visibility.

This document provides a high-level view of the existing business
process, the main problems ProjectNest is intended to solve, the MVP
scope, and the proposed architecture and technology stack.

------------------------------------------------------------------------

# 1. Business Lifecycle

The business works with brands through long-term supplier relationships, with projects driven by clients' store opening and renovation plans.

``` text
long-term Supplier Relationship
        ↓
Client Opening Plan
        ↓
Individual Project
        ↓
Site Survey
        ↓
Design & Approval
        ↓
Production
        ↓
Site Construction & Installation
        ↓
Inspection
        ↓
Store Opening
```

A **Store** and a **Project** are different concepts. A store represents a specific brand location within a mall. Over its lifecycle, the same store may have multiple projects, such as a new store project or later renovation projects.

## Project Initiation & Site Survey

A new project is issued by the **client** to the **Team Manager** via email, providing the basic project information. includes the brand, city, mall,
location, area, opening date, client contact, initial
requirements and any available mall drawings.

The **Team Manager Assistant** creates the project in ProjectNest and
assigns the responsible Designer.
Before design begins, the physical site is surveyed through a
cross-department workflow:

``` text
Manager Assistant issues Survey Work Order
        ↓
Engineering Manager Assistant assigns Survey Personnel
        ↓
Site Survey
        ↓
Designer checks completeness / consistency
        ↓
Supplement or correct if required
        ↓
Survey Personnel + Mall Representative sign
        ↓
Survey deliverables uploaded
```

The site survey produces a signed survey pack containing the site measurements, photos, drawings and other relevant information required for design.

## Design, Drawings & Production

The initial design goes through multiple rounds of revisions based on feedback from both the client and the mall. Once approved, the final mall-signed drawing set becomes the basis for developing the detailed construction and production drawings.

``` text
Survey
  ↓
Client Design Direction
  ↓
Concept Design
  ↓
Client Review / Revisions
  ↓
Client Confirmation
  ↓
Mall Submission / Revisions
  ↓
Mall Sign-off
  ↓
Detailed / Production Drawings
```

Existing business practice generally uses **one CAD file together with
one PDF drawing set**. Each Drawing Set can have multiple revisions, and
previous revisions are retained for traceability.

``` text
Production Drawing Set
│
├── R01
├── R02
└── R03
```

The Designer completes and checks the drawing locally before uploading
it to ProjectNest. Once uploaded, the drawing becomes available to the
Factory / Workshop for production.

Further changes may still occur after a drawing has been uploaded. The
Designer chooses how to issue the change according to its impact:

``` text
Change to Current Drawing
│
├── Local / relatively independent change
│   └── Upload revised sheet(s)
│       + identify affected sheet / area
│
└── Wider / related change
    └── Upload updated complete CAD + PDF
        + identify affected sheets
```

ProjectNest maintains an accurate record of drawing changes and the current production information. The Factory can follow the project from the time it is created, with site survey updates, project progress and uploaded documents becoming available as the project develops.

``` text
Project Created
      ↓
Factory gains project visibility
      ↓
Site Survey & Project Updates
      ↓
Documents / Drawings Uploaded
      ↓
Factory follows latest project information
      ↓
Production Drawing Uploaded
      ↓
Production Scheduling
      ↓
Production
```

Production involves multiple stages, such as Woodwork, Metalwork, Painting, Electrical, Glass/Acrylic, Hardware Installation, Pre-assembly and Packing. As production progresses, the Factory can update the current stage and progress in ProjectNest, with photos optionally uploaded to provide additional progress evidence.

## Site Installation & Opening

Factory production and site preparation may run in parallel. The Engineering team coordinates site preparation, mall entry requirements, construction and installation around the planned opening date.

Before opening, the Mall, Client and Company conduct a final inspection. Any identified issues are rectified, and the signed inspection record is retained with the project. After opening, the project enters the warranty and maintenance stage, which is outside the initial MVP.

------------------------------------------------------------------------

# 2. Application Scope & Requirements

Rather than digitising every activity in the company, ProjectNest provides a single, up-to-date source of project information throughout the project lifecycle. Internal teams use the system to update and access project information from project creation through to completion, while clients and mall representatives remain external participants and do not use the system directly.

**Fragmented project information** is one of the main problems. Project information is currently scattered across teams and communication channels. ProjectNest brings it together in one central project view.

**Drawing uncertainty** is another major problem. Drawings may exist in
Designer local files, Email, WeChat and copies distributed to different
teams. Drawing revisions and changes can make it difficult to identify the correct production drawing. ProjectNest maintains the current drawing and its change history.

**Drawing changes during production** need to reflect the actual working
method. Production drawing changes may affect individual sheets or the full drawing set. ProjectNest records both types of changes and their affected areas.

**Production visibility** relies heavily on manual communication. ProjectNest provides real-time visibility into production status and progress.

**Site Survey coordination** between Design and Engineering. ProjectNest need to manages the workflow, assignments, survey documents and signed records.

These problems translate into the following core capabilities:

  -----------------------------------------------------------------------
  Business Need                       ProjectNest Capability
  ----------------------------------- -----------------------------------
  Central project information         Project Management and Core Project
                                      View

  Structured survey coordination      Survey Work Order and Survey
                                      Documents

  Reliable drawing history            Drawing Sets and Revisions

  Controlled drawing changes          Local/Partial Changes and Full
                                      Drawing Updates

  Clear current production            Current Effective Drawing
  information                         Information

  Production visibility               Production Order and Stage Progress

  Historical traceability             Operational history and document
                                      metadata
  -----------------------------------------------------------------------

The MVP focuses on the core operational path:

``` text
Project
   ↓
Site Survey
   ↓
Drawing Management
   ↓
Production Visibility
```

The MVP covers project management, site survey, drawing management and production visibility, supported by authentication, role-based access and operational history.

Engineering/site management, procurement, logistics, acceptance, maintenance, finance, notifications, client portal and analytics are deferred to later phases.

Email, WeChat and Phone remain the main communication channels, while ProjectNest records the critical operational information teams need to rely on.

The application should allow a project participant to answer three
questions quickly:

> **Where is the project now?**\
> **Which information or drawing should I use now?**\
> **What has changed?**

------------------------------------------------------------------------

# 3. Architecture & Technology

ProjectNest uses a **Modular Monolith** architecture, keeping the system simple to develop and deploy while maintaining clear boundaries between Projects, Surveys, Drawings, Production and Identity.

``` mermaid
graph TD
    Browser[Web Browser]
    Frontend[React + TypeScript]
    API[ASP.NET Core Web API]
    Application[Application Layer]
    Domain[Domain Layer]
    Infrastructure[Infrastructure Layer]
    DB[(PostgreSQL)]
    Storage[(Private Object Storage)]

    Browser --> Frontend
    Frontend -->|HTTPS / REST / JSON| API
    API --> Application
    Application --> Domain
    Application --> Infrastructure
    Infrastructure --> DB
    Infrastructure --> Storage
```

The backend follows four explicit layers:

-   **API** --- HTTP endpoints, authentication, authorization and
    request/response handling.
-   **Application** --- use cases, validation, workflow orchestration
    and transaction boundaries.
-   **Domain** --- business rules for Projects, Surveys, Drawings and
    Production.
-   **Infrastructure** --- EF Core, PostgreSQL, ASP.NET Core Identity,
    file storage and infrastructure services.

The core domain is organised around:

``` text
Brand
└── Project
    ├── ProjectAssignment ─ User / Role
    ├── SurveyWorkOrder ─ SurveyDocument
    ├── DrawingSet
    │   ├── DrawingRevision
    │   └── DrawingChange
    └── ProductionOrder
        └── ProductionStageProgress ─ ProductionStage
```

Structured operational data is stored in **PostgreSQL**. CAD, PDF and
image binaries are stored in private object storage, with file metadata
retained in the database. Azure Blob Storage is the preferred cloud
direction, while storage access should remain abstracted from the
business layer.

Authentication uses **ASP.NET Core Identity with secure HttpOnly
cookies** for the internal browser application. Authorization combines
**role-based access control (RBAC)** with resource/project-based
policies and follows least privilege.

The frontend uses **React + TypeScript** with feature-oriented
organisation. The API uses **REST/JSON**, DTOs, request validation,
appropriate HTTP status codes, Problem Details and OpenAPI/Swagger
documentation.

Testing follows a risk-based strategy, with priority given to drawing revisions and changes, authorization, concurrency, survey workflows and production state.

- Unit tests verify business rules and application logic in isolation.
- Integration tests verify interactions between the ASP.NET Core API, application layer, EF Core and PostgreSQL, including important authorization and workflow scenarios.
- End-to-end (E2E) tests verify selected critical user workflows across the frontend, API and database.

Automated tests are integrated into GitHub Actions so that builds and tests run as part of the Pull Request and CI/CD workflow.

Git and GitHub are used from the start. Development follows feature
branches, Pull Requests and code review, with **GitHub Actions** used
for build, automated tests and later deployment. Development, testing
and production configuration are separated, and secrets and connection
strings are kept outside the repository.

  Area                Technology
  ------------------- ----------------------------------------------------
  Frontend            React + TypeScript
  Backend             ASP.NET Core / C#
  ORM                 Entity Framework Core
  Database            PostgreSQL
  File Storage        Azure Blob Storage / object storage
  Authentication      ASP.NET Core Identity + secure cookie
  Authorization       RBAC + resource/project-based policies
  API                 REST / JSON
  API Documentation   OpenAPI / Swagger
  Source Control      Git / GitHub
  CI/CD               GitHub Actions
  Cloud Direction     Azure
  Monitoring          Structured logging; Application Insights candidate

The MVP deliberately avoids Microservices, Kubernetes, Redis, RabbitMQ,
Kafka, Event Sourcing and full CQRS. These technologies should only be
introduced if a concrete requirement later justifies the additional
complexity.

Implementation will proceed incrementally through business-oriented
vertical slices:

``` text
Project
→ Survey
→ Drawing Management
→ Drawing Changes
→ Production Visibility
```

Each slice includes its requirement and acceptance criteria, domain/data
changes, API, frontend, validation, automated tests, review and
documentation before it is considered complete.
