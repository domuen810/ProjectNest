# ProjectNest — Domain & Data Model

## 1. Purpose and Scope

This document defines the domain model and logical data model for the ProjectNest MVP.

ProjectNest is an internal operations management system for retail fit-out projects. It provides a single source of truth for project information, site surveys, drawing revisions, factory orders and production progress.

The model is derived from the real business lifecycle and defines:

* core business entities
* relationships and cardinalities
* business rules
* reference data and enums
* logical tables and fields
* primary and foreign key relationships
* important data integrity constraints

### MVP Domain Scope

```text
Projects
Surveys
Drawings
Production
Identity
Files
```

The following lifecycle areas are outside the current MVP:

```text
Logistics
Site construction
Final acceptance
Mall exit / deposit
Store opening
Warranty
Maintenance
Full procurement management
Client portal
```

---

# 2. Domain Overview

## 2.1 Core Domain Relationships

```text
Brand 1 ───── 0..* Store
Mall  1 ───── 0..* Store

Store 1 ───── 0..* Project

Project
├── 1 ───── 0..* ProjectStatusChange
├── 1 ───── 0..* ProjectAssignment
├── 1 ───── 0..* SurveyWorkOrder
├── 1 ───── 0..* DrawingRevision
└── 1 ───── 0..* ProductionOrder

SurveyWorkOrder
└── 1 ───── 0..* SurveyVisit

SurveyVisit
├── 1 ───── 0..* SurveyPersonnel
└── 1 ───── 0..* SurveyVisitFile

DrawingRevision
└── 1 ───── 1..* DrawingRevisionFile

ProductionOrder
├── references DrawingRevision
└── 1 ───── 0..* ProductionStageProgress (one per Workshop)

ProductionStageProgress
├── 1 ───── 0..* ProductionProgressHistory
└── 1 ───── 0..* ReworkRecord
```

## 2.2 Domain Areas

```text
Projects
├── Brand
├── Mall
├── Store
├── Project
├── ProjectStatusChange
├── SavedContact
├── ProjectNature
├── ProjectCategory
└── CounterType

Surveys
├── SurveyWorkOrder
├── SurveyVisit
├── SurveyPersonnel
└── SurveyVisitFile

Drawings
├── DrawingRevision
└── DrawingRevisionFile

Production
├── ProductionOrder
├── Workshop
└── ProductionStageProgress

Identity
├── User
├── Authentication
├── JobPosition (predefined; one per User)
└── SystemRole (predefined; many-to-many with User)

Files
└── StoredFile
```

---

# 3. Project Domain

## 3.1 Brand

A `Brand` represents a retail brand served by the business.

```text
Brand
├── Id
├── Name
├── IsActive
├── CreatedAt
└── UpdatedAt
```

Relationships:

```text
Brand 1 ───── 0..* Store
Brand 1 ───── 0..* SavedContact
```

A Brand may operate multiple Stores across different Malls and cities.

### Brand — Rules Outside the Entity

1. Brand Name must be globally unique.
2. Brand Name uniqueness must be case-insensitive.
3. Leading and trailing whitespace must be ignored when checking uniqueness.
4. A Project can only be created using an Active Brand.

#### Implementation Responsibility

- Application layer: validate these business conditions.
- Database: add uniqueness protection where appropriate.
---

## 3.2 Mall

A `Mall` represents a specific mall location.

In real business usage, the Mall name may already include its city, for example:

```text
City A Mall A
City B Mall A
```

However, the system identifies and relates Mall records through:

```text
Mall.Id
```

rather than relying on the display name.

```text
Mall
├── Id
├── Name
├── City
├── IsActive
├── CreatedAt
└── UpdatedAt
```

`Mall.Name` is display/business data, not the relational identity.

### Mall — Rules Outside the Entity

1. Mall uniqueness is based on City + Name, not Name alone.
   Different cities may have Malls with the same name.

2. Historical Projects must retain the Mall name used at the time so that it remains consistent with related drawings and other project files.

#### Implementation Responsibility

- Application layer: validate Mall duplicate conditions.
- Database: enforce uniqueness for the City + Name combination where appropriate.
- Project: store a Mall name snapshot to preserve historical consistency.
---

## 3.3 Store

A `Store` represents a Brand's operating presence within a specific Mall over one business lifecycle.

```text
Store
├── Id
├── BrandId
├── MallId
├── Status
├── CreatedAt
└── UpdatedAt
```

Relationships:

```text
Brand 1 ───── 0..* Store
Mall 1 ───── 0..* Store
Store 1 ───── 0..* Project
```

A Store must be associated with one Brand and one Mall.

A Store represents one operating lifecycle of a Brand within a Mall.

If a Store is closed and the Brand later returns to the same Mall, a new Store is created rather than reopening the old Store.

### Store Lifecycle

```text
PreOpening → Open → Closed
```

- New Stores normally start as `PreOpening`.
- Existing Stores that are already trading may be created directly as `Open`.
- `Closed` is a terminal state and cannot return to `Open`.

`BrandId` and `MallId` are part of the Store's identity and are not normally changed after creation.

A Store does not store a separate Store name.

A display name can be generated dynamically from the related Brand and Mall, for example:

```text
Brand Name - Mall Name
```

#### Store and Project Modelling Rules

A Store can have multiple Projects over time.

Examples include:

- Temporary counter project
- Permanent counter project
- Independent beauty room project
- Renovation project

Temporary and permanent counters are modelled as Projects rather than Store types.

Counter shape is also modelled at Project level rather than Store level.

Examples include:

- Full Island
- Half Island
- Combined Counter
- Wall Counter

An integrated beauty room is treated as part of a Wall Counter Project.

An independent beauty room is modelled as a separate Project under the same Store because it has its own location, drawings, schedule, and construction work.

Island counters do not have integrated beauty rooms.

Project-specific location information belongs to the Project rather than the Store.

This includes:

- Floor
- Shop Number

This allows Projects under the same Store to have different locations, for example:

- Temporary counter on one floor
- Permanent counter in another location
- Independent beauty room on another floor
---

## 3.4 Project

A `Project` represents one fit-out project carried out for a Store.

```text
Project
├── Id
├── ProjectNumber
├── StoreId
├── ProjectNature
├── ProjectCategory
├── OpeningDate
├── InitialArea
├── MeasuredArea
├── Floor
├── ShopNumber
├── InitialRequirements
├── ResponsibleDesignerId
├── Status
├── BrandPaymentPercentage
├── MallPaymentPercentage
├── BrandNameSnapshot
├── MallNameSnapshot
├── BrandContactName?
├── BrandContactPosition?
├── BrandContactPhone?
├── BrandContactEmail?
├── MallContactName?
├── MallContactPosition?
├── MallContactPhone?
├── MallContactEmail?
├── CounterType?
├── HasIntegratedBeautyRoom?
├── RequiresTemporaryCounter
├── TemporaryCounterNotes
├── HasExistingCounter
├── ExistingCounterLocationRelation?
├── RequiresExistingCounterRemoval (Boolean, defaults to false)
├── ExistingCounterRemovalReason? (optional)
├── CreatedAt
├── CreatedById
├── UpdatedAt
└── UpdatedById
```

`ResponsibleDesignerId` is optional and stores the current Designer. A project may be created before a Designer is assigned. `ProjectAssignment` retains the assignment history.

### ProjectNature

```text
New
Renovation
```

Project nature is independent of project category.

### ProjectCategory

```text
PermanentCounter
IndependentBeautyRoom
```

A permanent counter requires a `CounterType`. A temporary counter is an optional configuration of a permanent-counter Project, not a separate Project or category. Independent beauty-room Projects are separate Projects and have no direct Project-to-Project relationship.

---

## 3.5 Project Status

```text
ProjectStatus
├── Active
├── Paused
├── Cancelled
└── Completed
```

`Project.Status` records the current operational state.

Example:

```text
Active
→ Paused
→ Active
→ Completed
```

Cancelled Projects are retained rather than deleted.

---

## 3.6 ProjectStatusChange

Project status history is retained separately.

```text
ProjectStatusChange
├── Id
├── ProjectId
├── PreviousStatus
├── NewStatus
├── Reason
├── ChangedAt
└── ChangedByUserId
```

Relationship:

```text
Project 1 ───── 0..* ProjectStatusChange
```

Rules:

```text
Paused
→ Reason required

Cancelled
→ Reason required
```

Existing Survey, Drawing, Production and File records remain available regardless of Project status.

### ProjectAssignment

Each assignment history record stores the Designer, when the assignment began and ended, and the User who made the assignment. `Project.ResponsibleDesignerId` continues to store the current Designer.

```text
ProjectAssignment
├── Id
├── ProjectId
├── DesignerUserId
├── AssignedAt
├── EndedAt?
└── AssignedByUserId
```

---

## 3.7 Payment Shares

Brand and Mall payment shares are Project-owned structured data, not contract instalments or free text.

```text
BrandPaymentPercentage
MallPaymentPercentage
```

The two percentages are each between 0 and 100, and their sum must equal 100.

```text
BrandPaymentPercentage + MallPaymentPercentage = 100
```

---

## 3.8 Temporary Counter Requirement

Temporary Counter is represented directly on Project:

```text
RequiresTemporaryCounter
TemporaryCounterNotes
```

The operational Temporary Counter workflow remains outside the MVP.

---

## 3.9 Project Contacts

ProjectNest does not implement a full contact-management system.

Two concepts are separated:

```text
SavedContact
Project Contact Snapshot
```

### SavedContact

Reusable lookup data used when creating Projects.

```text
SavedContact
├── Id
├── BrandId?
├── MallId?
├── Name
├── Position
├── Phone
├── Email
├── IsActive
├── CreatedAt
└── UpdatedAt
```

Exactly one of:

```text
BrandId
MallId
```

should be populated.

### Project Contact Snapshot

When a SavedContact is selected, its details are copied to the Project.

```text
SavedContact
      ↓ copy
Project Contact Snapshot
```

Project fields:

```text
BrandContactName
BrandContactPosition
BrandContactPhone
BrandContactEmail

MallContactName
MallContactPosition
MallContactPhone
MallContactEmail
```

The Project does not depend on future changes to the SavedContact.

This preserves historical contact information.

---

# 4. Survey Domain

## 4.1 SurveyWorkOrder

A `SurveyWorkOrder` represents one complete survey task.

```text
SurveyWorkOrder
├── Id
├── ProjectId
├── SurveyType
├── Status
├── PlannedSurveyDate?
├── Notes
├── FinalSurveyReportFileId?
├── CreatedAt
├── CreatedById
├── UpdatedAt
└── UpdatedById
```

### SurveyType

```text
InitialSurvey
ReMeasurement
FollowUpSurvey
```

### SurveyWorkOrderStatus

```text
Planned
InProgress
Completed
Cancelled
```

Relationship:

```text
Project 1 ───── 0..* SurveyWorkOrder
```

---

## 4.2 SurveyVisit

One SurveyWorkOrder may require multiple physical site visits.

```text
SurveyWorkOrder 1 ───── 0..* SurveyVisit
```

```text
SurveyVisit
├── Id
├── SurveyWorkOrderId
├── PlannedDate
├── ActualVisitDate?
├── Status
├── NoResultReason?
├── Findings
├── Notes
├── VisitReportFileId?
├── CreatedAt
└── UpdatedAt
```

### SurveyVisitStatus

```text
Completed
Cancelled
Planned
InProgress
```

Survey personnel may differ between visits. Each `SurveyPersonnel` record belongs to one `SurveyVisit` and stores only a name and phone number; it is not linked to `User`.

---

## 4.3 SurveyPersonnel

`SurveyPersonnel` records the people who took part in a particular visit. It is a value record owned by `SurveyVisit`, not an account or User assignment.

```text
SurveyVisit 1 ───── 0..* SurveyPersonnel
```

```text
SurveyPersonnel
├── Id
├── SurveyVisitId
├── Name
└── Phone
```

`SurveyPersonnel` is a Domain value object containing only `Name` and `Phone`. `Id` and `SurveyVisitId` above are persistence-mapping details only if required by the chosen EF Core mapping; they are not Domain properties.

---

## 4.4 Survey Source Files

Survey source materials use the shared `StoredFile` entity.

```text
SurveyVisitFile
├── Id
├── SurveyVisitId
├── StoredFileId
└── Category
```

### SurveyFileCategory

```text
SitePhoto
MeasurementSketch
MallProvidedFile
SurveyForm
Other
```

---

## 4.5 Survey Reports

Each SurveyVisit may reference its final visit report:

```text
SurveyVisit.VisitReportFileId
→ StoredFile
```

The complete SurveyWorkOrder may reference the final client-facing survey report:

```text
SurveyWorkOrder.FinalSurveyReportFileId
→ StoredFile
```

A separate SurveyVisitReport Entity is not required.

---

## 4.6 Survey Business Rules

```text
One SurveyWorkOrder
→ may contain multiple SurveyVisits

Survey personnel
→ may differ between visits

SurveyWorkOrder and SurveyVisit statuses
→ Planned, InProgress, Completed, Cancelled

SurveyVisit without usable measurement results
→ NoResultReason is required when confirmed without a result

Survey history
→ retained

Final Survey Report
→ belongs to SurveyWorkOrder
```

---

# 5. Drawing Domain

## 5.1 DrawingRevision

Drawing history is represented directly through `DrawingRevision`.

There is no separate `DrawingSet` Entity in the MVP.

```text
Project 1 ───── 0..* DrawingRevision
```

```text
DrawingRevision
├── Id
├── ProjectId
├── Stage
├── RevisionNumber
├── IsCurrent
├── ChangeScope
├── ChangeReason
├── ChangeSummary
├── FeedbackOrChangeRequest
├── AffectedPagesOrAreas
├── CreatedAt
└── CreatedById
```

---

## 5.2 DrawingStage

```text
InitialDesign
MallSigned
Construction
ProductionOrder
```

Revisions are grouped by Stage.

Example:

```text
Initial Design
├── R01
├── R02
└── R03 ← Current

Mall Signed
├── R01
└── R02 ← Current

Construction
├── R01
└── R02 ← Current

Production / Order
├── R01
├── R02
└── R03 ← Current
```

---

## 5.3 Drawing Revision History

Every formal drawing submission creates a new `DrawingRevision`.

This applies to all four DrawingStages.

Previous revisions are never overwritten.

Initial Design revisions may represent significantly different design concepts.

Example:

```text
R01 → Concept A
R02 → Different Concept B
R03 → Revised Concept B
```

All formal submissions remain traceable.

---

## 5.4 Current Drawing Revision

Each:

```text
Project + DrawingStage
```

may have at most one current DrawingRevision.

`IsCurrent` identifies the revision currently intended for use within that Stage.

---

## 5.5 Drawing Change Scope

```text
DrawingChangeScope
├── FullSet
└── Partial
```

### FullSet

A complete updated drawing package is uploaded.

### Partial

Only affected pages, sheets or areas are updated.

For Partial revisions:

```text
AffectedPagesOrAreas
→ required
```

Example:

```text
Pages 12, 18
Front Counter
Electrical Detail E03
```

---

## 5.6 Drawing Change Reason

Controlled values:

```text
ClientChange
MallRequirement
SiteConditionChange
ManufacturingIssue
Other
```

Each revision records:

```text
ChangeReason
ChangeSummary
FeedbackOrChangeRequest
```

These answer:

```text
What changed?
Why did it change?
```

Supporting feedback files may also be attached.

---

## 5.7 Drawing Files

Drawing files use `StoredFile`.

```text
DrawingRevisionFile
├── Id
├── DrawingRevisionId
├── StoredFileId
└── FileRole
```

FileRole:

```text
Drawing
SupportingFeedback
Other
```

CAD, PDF and supporting documents are file formats, not separate Entities.

---

## 5.8 Drawing Business Rules

```text
Every formal submission
→ creates DrawingRevision

Previous revision
→ never overwritten

Project + DrawingStage
→ at most one current revision

Partial revision
→ AffectedPagesOrAreas required

DrawingRevision
→ records reason for change

Designer
→ self-checks and uploads directly

No separate Drawing Release workflow
```

---

# 6. Production Domain

## 6.1 ProductionOrder

A Project may have multiple factory orders.

`OrderNumber` is required and unique within the Project. It is entered manually in the MVP; automatic numbering is out of scope.

```text
Project 1 ───── 0..* ProductionOrder
```

This represents common phased ordering.

Example:

```text
Project A
├── ProductionOrder #01
│   └── Front Counter
├── ProductionOrder #02
│   └── Back Counter
└── ProductionOrder #03
    └── Additional Components
```

Model:

```text
ProductionOrder
├── Id
├── ProjectId
├── OrderNumber
├── Scope
├── InitialDrawingRevisionId
├── CurrentDrawingRevisionId
├── Status
├── PlannedStartDate
├── PlannedCompletionDate
├── ActualStartDate
├── ActualCompletionDate
├── Notes
├── CreatedAt
├── CreatedById
├── UpdatedAt
└── UpdatedById
```

---

## 6.2 ProductionOrderStatus

```text
Planned
InProduction
Completed
Cancelled
```

Project status and ProductionOrder status are independent.

Example:

```text
Project = Active

ProductionOrder #01 = Completed
ProductionOrder #02 = InProduction
```

---

## 6.3 Production Order Scope

`Scope` is required because different ProductionOrders may cover different parts of the same Project.

Examples:

```text
Front Counter
Back Counter
Wall Cabinets
Island Counter
```

---

## 6.4 Initial and Current Drawing Revision

Each ProductionOrder records:

```text
InitialDrawingRevisionId
CurrentDrawingRevisionId
```

### InitialDrawingRevision

The drawing revision used when the order was originally issued.

### CurrentDrawingRevision

The drawing revision the Factory should currently use.

Example:

```text
ProductionOrder #01

InitialDrawingRevision
→ Production R02

Manufacturing issue
        ↓
DrawingRevision R03

CurrentDrawingRevision
→ Production R03
```

Different ProductionOrders may reference different DrawingRevisions.

---

## 6.5 Workshop

`Workshop` is the trade/work area used to track progress independently.

The MVP catalog contains these seven independent operations:

```text
Carpentry
Painting
Glass
Metal
Electrical
Pre-assembly
Packing & Shipping
```

Model:

```text
Workshop
├── Id
├── Name
└── IsActive
```

---

## 6.6 ProductionStageProgress

```text
ProductionStageProgress
├── Id
├── ProductionOrderId
├── WorkshopId
├── Status
├── Notes
├── CreatedAt
├── CreatedById
├── UpdatedAt
└── UpdatedById
```

Relationship:

```text
ProductionOrder 1 ───── 0..* ProductionStageProgress
Workshop 1 ───── 0..* ProductionStageProgress
UNIQUE (ProductionOrderId, WorkshopId)
```

### ProductionStageStatus

```text
NotStarted
InProgress
Completed
Blocked
```

Workshop progress records may start independently and may be active at the same time.

Every status change is retained in `ProductionProgressHistory`, while `ProductionStageProgress.Status` represents the current status. `ReworkRecord` continues to record rework history independently.

```text
ProductionProgressHistory
├── Id
├── ProductionStageProgressId
├── PreviousStatus
├── NewStatus
├── ChangedAt
└── ChangedByUserId
```

`ReworkRecord` remains a separate history record owned by `ProductionStageProgress`; it records rework reason, optional note, recording User and time. A rework record is not a substitute for a status-change history record.

The current Domain model represents progress using `Status`; a separate percentage or planned/actual progress-date model is not specified by the confirmed MVP decisions.

---

## 6.7 Production Drawing Changes

Production does not maintain a separate drawing-change history.

If a manufacturing issue requires a drawing change:

```text
Manufacturing Issue
        ↓
New DrawingRevision
        ↓
ChangeReason = ManufacturingIssue
        ↓
ProductionOrder.CurrentDrawingRevision updated
```

Responsibilities remain separated:

```text
Drawing Domain
→ drawing history and change reasons

Production Domain
→ factory orders and production progress
```

---

## 6.8 Production Business Rules

```text
Project
→ may have multiple ProductionOrders

ProductionOrder
→ Scope required

OrderNumber
→ unique within Project

Workshop progress
→ may overlap

ProductionStageProgress
→ at most one record per Workshop per ProductionOrder

InitialDrawingRevision
→ retained

CurrentDrawingRevision
→ current factory drawing basis

Referenced DrawingRevision
→ must belong to the same Project
→ must use ProductionOrder DrawingStage
```

---

# 7. Identity Domain

## 7.1 User

A `User` represents an internal ProjectNest user.

```text
User
├── Id
├── Name
├── Email
├── JobPositionId
├── IsActive
├── CreatedAt
└── UpdatedAt
```

Every User has exactly one required `JobPosition`, identified by `JobPositionId`. `JobPosition` is a predefined business-position catalog and is managed independently from `SystemRole`. External Client and Mall contacts are not Users.

Inactive Users remain in the system to preserve historical references.

---

## 7.2 SystemRole and JobPosition Catalogs

`SystemRole` is a predefined authorization-role catalog; users cannot create roles. A User may have multiple SystemRoles, and each SystemRole may be assigned to multiple Users. The User/SystemRole association is many-to-many.

`JobPosition` is a separate predefined business-position catalog. Each User has exactly one JobPosition. JobPosition and SystemRole are managed independently; a JobPosition does not grant authorization.

---

# 8. File Domain

## 8.1 StoredFile

`StoredFile` represents file metadata used by the Domain Model.

```text
StoredFile
├── Id
├── OriginalFileName
├── ContentType
├── FileExtension
├── FileSize
├── StorageKey
├── UploadedAt
└── UploadedById
```

It is shared by:

```text
Survey source files
Survey visit reports
Final survey reports
Drawing files
Drawing supporting files
```

The Domain Model stores the file record and reference; physical file-storage implementation is defined separately in the architecture.

---

# 9. Reference Data and Enums

## 9.1 Enums

```text
ProjectNature
├── New
└── Renovation

ProjectCategory
├── PermanentCounter
└── IndependentBeautyRoom

ProjectStatus
├── Active
├── Paused
├── Cancelled
└── Completed

SurveyType
├── InitialSurvey
├── ReMeasurement
└── FollowUpSurvey

SurveyWorkOrderStatus
├── Planned
├── InProgress
├── Completed
└── Cancelled

SurveyVisitStatus
├── Planned
├── InProgress
├── Completed
└── Cancelled

SurveyFileCategory
├── SitePhoto
├── MeasurementSketch
├── MallProvidedFile
├── SurveyForm
└── Other

DrawingStage
├── InitialDesign
├── MallSigned
├── Construction
└── ProductionOrder

DrawingChangeScope
├── FullSet
└── Partial

DrawingChangeReason
├── ClientChange
├── MallRequirement
├── SiteConditionChange
├── ManufacturingIssue
└── Other

CounterType
├── FullIsland
├── HalfIsland
├── CombinedCounter
└── WallCounter

ProductionOrderStatus
├── Planned
├── InProduction
├── Completed
└── Cancelled

ProductionStageStatus
├── NotStarted
├── InProgress
├── Completed
└── Blocked

```

`SystemRole` and `JobPosition` are predefined reference catalogs, not Domain enums. Their exact catalog entries are not specified here. Every User has one required JobPosition and may have multiple SystemRoles.

## 9.2 Reference Data

```text
Workshop
```

`Workshop` is reference data for the seven MVP trades. `CounterType` is a fixed Domain enum and applies only to `PermanentCounter` Projects.

Workshop names are restricted to the seven values above and cannot be renamed. The records will be initialized by database seed data in the persistence phase.

---

# 10. Logical Data Model

## Brands

```text
Id                  PK
Name                required
IsActive            required
CreatedAt           required
UpdatedAt           required
```

## Malls

```text
Id                  PK
Name                required
City                required
IsActive            required
CreatedAt           required
UpdatedAt           required
```

## Stores

```text
Id                  PK
BrandId             FK → Brands
MallId              FK → Malls
Status              required (PreOpening, Open, Closed)
CreatedAt           required
UpdatedAt           required
```

`Store` remains a separate Brand-in-Mall location entity; counter classification belongs to Project. Its lifecycle is `PreOpening`, `Open`, or `Closed`, and a Store may have multiple Projects over time.

## Projects

```text
Id                          PK
ProjectNumber               required
StoreId                     FK → Stores
ProjectNature               required
ProjectCategory             required
OpeningDate                 required
InitialArea                 optional
MeasuredArea                optional
Floor                       optional
ShopNumber                  optional
InitialRequirements         optional
ResponsibleDesignerId       FK → Users, optional
Status                      required
BrandPaymentPercentage     required
MallPaymentPercentage      required
BrandNameSnapshot           required
MallNameSnapshot            required
BrandContactName            optional
BrandContactPosition        optional
BrandContactPhone           optional
BrandContactEmail           optional
MallContactName             optional
MallContactPosition         optional
MallContactPhone            optional
MallContactEmail            optional
CounterType                 required for PermanentCounter; otherwise absent
HasIntegratedBeautyRoom     optional
RequiresTemporaryCounter    required
TemporaryCounterNotes       optional
HasExistingCounter          required
ExistingCounterLocationRelation conditional
RequiresExistingCounterRemoval required Boolean (defaults to false)
ExistingCounterRemovalReason optional

CreatedAt                   required
CreatedById                 FK → Users
UpdatedAt                   required
UpdatedById                 FK → Users
```

`ResponsibleDesignerId` is optional and stores the current Designer. Assignment changes are retained in `ProjectAssignments`.

`RequiresExistingCounterRemoval` is a Project-level Boolean option for Engineering work planning. It is false by default, and no reason is required when demolition is not selected.

## ProjectAssignments

```text
Id                  PK
ProjectId           FK → Projects
DesignerUserId      FK → Users
AssignedAt          required
EndedAt             optional
AssignedByUserId    FK → Users
```

## ProjectStatusChanges

```text
Id                  PK
ProjectId           FK → Projects
PreviousStatus      required
NewStatus           required
Reason              required for Paused or Cancelled transitions
ChangedAt           required
ChangedByUserId     FK → Users
```

## OpeningDateChanges and MeasuredAreaChanges

```text
OpeningDateChange: ProjectId, PreviousOpeningDate, NewOpeningDate, ChangedByUserId, ChangedAt
MeasuredAreaChange: ProjectId, PreviousArea, NewArea, ChangedByUserId, ChangedAt, Reason
```

Measured-area changes require a non-empty reason.

## SavedContacts

```text
Id                  PK
BrandId             FK → Brands, optional
MallId              FK → Malls, optional
Name                required
Position            optional
Phone               optional
Email               optional
IsActive            required
CreatedAt           required
UpdatedAt           required
```

## SurveyWorkOrders

```text
Id                          PK
ProjectId                   FK → Projects
SurveyType                  required (InitialSurvey, ReMeasurement, FollowUpSurvey)
Status                      required (Planned, InProgress, Completed, Cancelled)
PlannedSurveyDate           optional
Notes                       optional
FinalSurveyReportFileId     FK → StoredFiles, optional
CreatedAt                   required
CreatedById                 FK → Users
UpdatedAt                   required
UpdatedById                 FK → Users
```

## SurveyVisits

```text
Id                  PK
SurveyWorkOrderId   FK → SurveyWorkOrders
PlannedDate         required
ActualVisitDate     optional
Status              required (Planned, InProgress, Completed, Cancelled)
NoResultReason      optional
Findings            optional
Notes               optional
VisitReportFileId   FK → StoredFiles, optional
CreatedAt           required
UpdatedAt           required
```

## SurveyPersonnel

```text
Id                  PK
SurveyVisitId       FK → SurveyVisits
Name                required
Phone               required
```

## SurveyVisitFiles

```text
Id                  PK
SurveyVisitId       FK → SurveyVisits
StoredFileId        FK → StoredFiles
Category            required
```

## DrawingRevisions

```text
Id                          PK
ProjectId                   FK → Projects
DrawingStage                required
RevisionNumber              required
IsCurrent                   required
ChangeScope                 required
ChangeReason                required
ChangeSummary               optional
FeedbackOrChangeRequest     optional
AffectedPagesOrAreas        conditional
CreatedAt                   required
CreatedById                 FK → Users
```

## DrawingRevisionFiles

```text
Id                  PK
DrawingRevisionId   FK → DrawingRevisions
StoredFileId        FK → StoredFiles
FileRole            required
```

## ProductionOrders

```text
Id                          PK
ProjectId                   FK → Projects
OrderNumber                 required
Scope                       required
InitialDrawingRevisionId    FK → DrawingRevisions
CurrentDrawingRevisionId    FK → DrawingRevisions
Status                      required
PlannedStartDate            optional
PlannedCompletionDate       optional
ActualStartDate             optional
ActualCompletionDate        optional
Notes                       optional
CreatedAt                   required
CreatedById                 FK → Users
UpdatedAt                   required
UpdatedById                 FK → Users
```

## Workshops

```text
Id                  PK
Name                required
IsActive            required
```

## ProductionStageProgress

```text
Id                      PK
ProductionOrderId       FK → ProductionOrders
WorkshopId              FK → Workshops
Status                  required
Notes                   optional
CreatedAt               required
CreatedById              FK → Users
UpdatedAt               required
UpdatedById             FK → Users
```

## ProductionProgressHistory

```text
Id                          PK
ProductionStageProgressId   FK → ProductionStageProgress
PreviousStatus              required
NewStatus                  required
ChangedAt                   required
ChangedByUserId             FK → Users
```

`ProductionStageProgress.Status` remains the current status; each status-change record is retained separately.

## ReworkRecords

```text
Id                          PK
ProductionStageProgressId   FK → ProductionStageProgress
ReasonType                  required
Note                        optional
RecordedByUserId            FK → Users
RecordedAt                  required
```

`ReworkRecord` remains separate from `ProductionProgressHistory` and records rework-specific history.

## Users

```text
Id                  PK
Name                required
Email               required
JobPositionId       FK → JobPositions, required
IsActive            required
CreatedAt           required
UpdatedAt           required
```

## JobPositions

```text
Id                  PK
Name                required
```

The JobPosition catalog is predefined and managed independently from SystemRole.

## SystemRoles

```text
Id                  PK
Name                required
```

The SystemRole catalog is predefined; users cannot create roles.

## UserSystemRoles

```text
UserId              PK, FK → Users
SystemRoleId        PK, FK → SystemRoles
```

The pair `(UserId, SystemRoleId)` is unique, implementing the confirmed many-to-many association.

## StoredFiles

```text
Id                  PK
OriginalFileName    required
ContentType         required
FileExtension       required
FileSize            required
StorageKey          required
UploadedAt          required
UploadedById        FK → Users
```

---

# 11. Data Integrity and Constraints

## 11.1 Identity

Core Entities use stable system IDs.

Names such as Brand name or Mall name are business/display data and are not used as relational identifiers.

---

## 11.2 Unique Constraints

```text
Projects
UNIQUE(ProjectNumber)

Users
UNIQUE(Email)

DrawingRevisions
UNIQUE(ProjectId, Stage, RevisionNumber)

ProductionOrders
UNIQUE(ProjectId, OrderNumber)

ProductionStageProgress
UNIQUE(ProductionOrderId, WorkshopId)

StoredFiles
UNIQUE(StorageKey)
```

---

## 11.3 Current Drawing Revision

For each:

```text
ProjectId + DrawingStage
```

there may be at most one:

```text
IsCurrent = true
```

Drawing history remains retained when the current revision changes.

---

## 11.4 Project Status Rules

```text
Paused
→ Reason required

Cancelled
→ Reason required

Cancelled Project
→ retained
```

---

## 11.5 Survey Rules

Both SurveyWorkOrder and SurveyVisit use `Planned`, `InProgress`, `Completed`, and `Cancelled`.

`ConfirmWithoutResult` requires a non-empty reason, retained as `NoResultReason`; this does not introduce an `Incomplete` status.

A cancelled SurveyVisit cannot subsequently be confirmed.

---

## 11.6 Drawing Rules

```text
DrawingChangeScope = Partial
→ AffectedPagesOrAreas required

DrawingRevision
→ at least one DrawingRevisionFile is required
```

---

## 11.7 Production Drawing Rules

Both:

```text
InitialDrawingRevisionId
CurrentDrawingRevisionId
```

must reference DrawingRevisions:

```text
belonging to the same Project
and
Stage = ProductionOrder
```

---

## 11.8 Payment Rules

```text
BrandPaymentPercentage + MallPaymentPercentage = 100

Each percentage
→ between 0 and 100
```

---

## 11.9 SavedContact Organisation Rule

A SavedContact belongs to either:

```text
Brand
OR
Mall
```

but not both.

Exactly one of:

```text
BrandId
MallId
```

must contain a value.

---

## 11.10 Historical Record Protection

Important operational history should be retained.

This includes:

```text
Project
ProjectStatusChange
SurveyWorkOrder
SurveyVisit
DrawingRevision
ProductionOrder
StoredFile
```

Reference or reusable records may be deactivated rather than removed from historical relationships:

```text
Brand
Mall
Store
CounterType
Workshop
User
SavedContact
```

---

# 12. Key Domain Rules Summary

```text
STORE

One Store represents one Brand location in one Mall.

One Mall may contain multiple Stores belonging to
different Brands handled by the company.

One Store may have multiple Projects over time.


PROJECT

ProjectNature = New / Renovation.

ProjectCategory = PermanentCounter / IndependentBeautyRoom.

PermanentCounter Projects require one CounterType: FullIsland, HalfIsland, CombinedCounter or WallCounter.

Project payment shares are structured Brand/Mall percentages totaling 100%; they are not instalments.

Project may be Active, Paused, Cancelled or Completed.

ResponsibleDesignerId is optional; assignment changes are retained.

Paused / Cancelled requires a reason.

Status changes record previous status, new status, time and changing User.

Once a responsible Designer is assigned, assignment cannot be cancelled; changes end the old ProjectAssignment and add a new one.

Cancelled Projects remain in the system.


SURVEY

One SurveyWorkOrder may require multiple SurveyVisits.

Survey personnel may differ between visits; each visit records its own name/phone SurveyPersonnel, not Users.

Each visit retains its own source files and report.

Final Survey Report belongs to SurveyWorkOrder.


DRAWING

Every formal drawing submission creates a DrawingRevision.

All four DrawingStages retain revision history.

Previous revisions are never overwritten.

Each Project + Stage has at most one current revision.

Every revision records why it changed.

Partial revisions identify affected pages / areas.


PRODUCTION

Each ProductionOrder has a required OrderNumber, unique within its Project; MVP numbering is manual.

Each ProductionOrder has at most one ProductionStageProgress per Workshop.

Workshop progress may begin independently and overlap.

The fixed MVP Workshop catalog contains Carpentry, Painting, Glass, Metal, Electrical, Pre-assembly, and Packing & Shipping; arbitrary entries cannot be created or renamed.

ProductionOrder status is Planned, InProduction, Completed, or Cancelled; ProductionStageProgress also supports Blocked.


HISTORY AND AUDIT

Retain Project status changes, responsible Designer assignment changes, DrawingRevision updates and ProductionStageProgress changes.

Do not record ordinary page views or button clicks as operational history.

Existing generic audit fields, such as `CreatedByUserId` and `UpdatedByUserId`, are retained on Domain entities where they already exist; they are not required on every Domain entity. Audit metadata population and handling will be centralized in the Application/Infrastructure layers in a later implementation stage. This does not remove existing Domain audit fields. Actor IDs in explicit business-history records remain separate and unchanged.

One Project may have multiple ProductionOrders.

Each ProductionOrder has an explicit Scope.

Each order retains its initial drawing basis.

Each order also identifies its current drawing basis.

Different orders may reference different DrawingRevisions.

Workshop progress may start independently and overlap.

Drawing-change history belongs to the Drawing domain.


FILES

StoredFile represents file metadata used by business records.

File records remain linked to historical business records.


IDENTITY

Only internal staff are ProjectNest Users.

External Client and Mall contacts are not Users.

Inactive Users remain available for historical references.
```

---

# 13. ERD

The following ERD represents the logical data model for the ProjectNest MVP.

```mermaid
erDiagram

    BRAND ||--o{ STORE : has
    BRAND ||--o{ SAVED_CONTACT : has

    MALL ||--o{ STORE : contains
    MALL ||--o{ SAVED_CONTACT : has

    STORE ||--o{ PROJECT : has

    USER o|--o{ PROJECT : responsible_designer
    USER ||--o{ PROJECT : created_by
    USER ||--o{ PROJECT : updated_by

    PROJECT ||--o{ PROJECT_STATUS_CHANGE : has
    USER ||--o{ PROJECT_STATUS_CHANGE : changed_by
    PROJECT ||--o{ PROJECT_ASSIGNMENT : assignment_history
    USER ||--o{ PROJECT_ASSIGNMENT : assigned_designer
    USER ||--o{ PROJECT_ASSIGNMENT : assigned_by

    JOB_POSITION ||--o{ USER : assigned_position
    USER ||--o{ USER_SYSTEM_ROLE : has_role
    SYSTEM_ROLE ||--o{ USER_SYSTEM_ROLE : assigned_to


    PROJECT ||--o{ SURVEY_WORK_ORDER : has
    USER ||--o{ SURVEY_WORK_ORDER : created_by

    SURVEY_WORK_ORDER ||--o{ SURVEY_VISIT : contains
    STORED_FILE o|--o| SURVEY_WORK_ORDER : final_report

    SURVEY_VISIT ||--o{ SURVEY_PERSONNEL : has

    SURVEY_VISIT ||--o{ SURVEY_VISIT_FILE : has
    STORED_FILE ||--o{ SURVEY_VISIT_FILE : referenced_by
    STORED_FILE o|--o| SURVEY_VISIT : visit_report

    PROJECT ||--o{ DRAWING_REVISION : has
    USER ||--o{ DRAWING_REVISION : created_by

    DRAWING_REVISION ||--|{ DRAWING_REVISION_FILE : contains
    STORED_FILE ||--o{ DRAWING_REVISION_FILE : referenced_by

    PROJECT ||--o{ PRODUCTION_ORDER : has

    DRAWING_REVISION ||--o{ PRODUCTION_ORDER : initial_drawing
    DRAWING_REVISION ||--o{ PRODUCTION_ORDER : current_drawing

    USER ||--o{ PRODUCTION_ORDER : created_by
    USER ||--o{ PRODUCTION_ORDER : updated_by

    PRODUCTION_ORDER ||--o{ PRODUCTION_STAGE_PROGRESS : has
    WORKSHOP ||--o{ PRODUCTION_STAGE_PROGRESS : defines

    USER ||--o{ PRODUCTION_STAGE_PROGRESS : updated_by
    PRODUCTION_STAGE_PROGRESS ||--o{ PRODUCTION_PROGRESS_HISTORY : status_history
    USER ||--o{ PRODUCTION_PROGRESS_HISTORY : changed_by
    PRODUCTION_STAGE_PROGRESS ||--o{ REWORK_RECORD : rework_history
    USER ||--o{ REWORK_RECORD : recorded_by

    USER ||--o{ STORED_FILE : uploaded_by


    BRAND {
        uuid Id PK
        string Name
        bool IsActive
        datetime CreatedAt
        uuid CreatedById FK
        datetime UpdatedAt
        uuid UpdatedById FK
    }

    MALL {
        uuid Id PK
        string Name
        string City
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    STORE {
        uuid Id PK
        uuid BrandId FK
        uuid MallId FK
        string Status
        datetime CreatedAt
        datetime UpdatedAt
    }

    PROJECT {
        uuid Id PK
        string ProjectNumber
        uuid StoreId FK
        string ProjectNature
        string ProjectCategory
        date OpeningDate
        decimal InitialArea
        decimal MeasuredArea
        string Floor
        string ShopNumber
        string InitialRequirements
        uuid ResponsibleDesignerId FK, optional
        string Status

        decimal MallPaymentPercentage
        decimal BrandPaymentPercentage

        string BrandContactName
        string BrandContactPosition
        string BrandContactPhone
        string BrandContactEmail
        string MallContactName
        string MallContactPosition
        string MallContactPhone
        string MallContactEmail

        bool RequiresTemporaryCounter
        string TemporaryCounterNotes
        string CounterType
        bool HasIntegratedBeautyRoom
        bool HasExistingCounter
        string ExistingCounterLocationRelation
        bool RequiresExistingCounterRemoval
        string ExistingCounterRemovalReason

        datetime CreatedAt
        uuid CreatedById FK
        datetime UpdatedAt
        uuid UpdatedById FK
    }

    PROJECT_STATUS_CHANGE {
        uuid Id PK
        uuid ProjectId FK
        string PreviousStatus
        string NewStatus
        string Reason
        datetime ChangedAt
        uuid ChangedByUserId FK
    }

    PROJECT_ASSIGNMENT {
        uuid Id PK
        uuid ProjectId FK
        uuid DesignerUserId FK
        datetime AssignedAt
        datetime EndedAt
        uuid AssignedByUserId FK
    }

    SAVED_CONTACT {
        uuid Id PK
        uuid BrandId FK
        uuid MallId FK
        string Name
        string Position
        string Phone
        string Email
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    SURVEY_WORK_ORDER {
        uuid Id PK
        uuid ProjectId FK
        string SurveyType
        string Status
        date PlannedSurveyDate
        string Notes
        uuid FinalSurveyReportFileId FK
        datetime CreatedAt
        uuid CreatedById FK
        datetime UpdatedAt
        uuid UpdatedById FK
    }

    SURVEY_VISIT {
        uuid Id PK
        uuid SurveyWorkOrderId FK
        date PlannedDate
        date ActualVisitDate
        string Status
        string NoResultReason
        string Findings
        string Notes
        uuid VisitReportFileId FK
        datetime CreatedAt
        datetime UpdatedAt
    }

    SURVEY_PERSONNEL {
        uuid Id PK
        uuid SurveyVisitId FK
        string Name
        string Phone
    }

    SURVEY_VISIT_FILE {
        uuid Id PK
        uuid SurveyVisitId FK
        uuid StoredFileId FK
        string Category
    }

    DRAWING_REVISION {
        uuid Id PK
        uuid ProjectId FK
        string DrawingStage
        int RevisionNumber
        bool IsCurrent
        string ChangeScope
        string ChangeReason
        string ChangeSummary
        string FeedbackOrChangeRequest
        string AffectedPagesOrAreas
        datetime CreatedAt
        uuid CreatedById FK
    }

    DRAWING_REVISION_FILE {
        uuid Id PK
        uuid DrawingRevisionId FK
        uuid StoredFileId FK
        string FileRole
    }

    PRODUCTION_ORDER {
        uuid Id PK
        uuid ProjectId FK
        string OrderNumber
        string Scope
        uuid InitialDrawingRevisionId FK
        uuid CurrentDrawingRevisionId FK
        string Status
        date PlannedStartDate
        date PlannedCompletionDate
        date ActualStartDate
        date ActualCompletionDate
        string Notes
        datetime CreatedAt
        uuid CreatedById FK
        datetime UpdatedAt
        uuid UpdatedById FK
    }

    WORKSHOP {
        uuid Id PK
        string Name
        bool IsActive
    }

    PRODUCTION_STAGE_PROGRESS {
        uuid Id PK
        uuid ProductionOrderId FK
        uuid WorkshopId FK
        string Status
        string Notes
        datetime CreatedAt
        uuid CreatedById FK
        datetime UpdatedAt
        uuid UpdatedById FK
    }

    PRODUCTION_PROGRESS_HISTORY {
        uuid Id PK
        uuid ProductionStageProgressId FK
        string PreviousStatus
        string NewStatus
        datetime ChangedAt
        uuid ChangedByUserId FK
    }

    REWORK_RECORD {
        uuid Id PK
        uuid ProductionStageProgressId FK
        string ReasonType
        string Note
        uuid RecordedByUserId FK
        datetime RecordedAt
    }

    JOB_POSITION {
        uuid Id PK
        string Name
    }

    SYSTEM_ROLE {
        uuid Id PK
        string Name
    }

    USER_SYSTEM_ROLE {
        uuid UserId PK, FK
        uuid SystemRoleId PK, FK
    }

    USER {
        uuid Id PK
        string Name
        string Email
        uuid JobPositionId FK
        bool IsActive
        datetime CreatedAt
        datetime UpdatedAt
    }

    STORED_FILE {
        uuid Id PK
        string OriginalFileName
        string ContentType
        string FileExtension
        long FileSize
        string StorageKey
        datetime UploadedAt
        uuid UploadedById FK
    }
```

## 13.1 Key Cardinalities

```text
Brand 1 ───── 0..* Store

Mall 1 ───── 0..* Store

Store 1 ───── 0..* Project

Project 1 ───── 0..* ProjectStatusChange

Project 1 ───── 0..* ProjectAssignment
User 1 ───── 0..* ProjectAssignment (DesignerUserId)
User 1 ───── 0..* ProjectAssignment (AssignedByUserId)

JobPosition 1 ───── 0..* User
User 1 ───── 0..* UserSystemRole
SystemRole 1 ───── 0..* UserSystemRole

Project 1 ───── 0..* SurveyWorkOrder

SurveyWorkOrder 1 ───── 0..* SurveyVisit

SurveyVisit 1 ───── 0..* SurveyPersonnel

Project 1 ───── 0..* DrawingRevision

DrawingRevision 1 ───── 1..* DrawingRevisionFile

Project 1 ───── 0..* ProductionOrder

ProductionOrder 1 ───── 0..* ProductionStageProgress

Workshop 1 ───── 0..* ProductionStageProgress

ProductionStageProgress 1 ───── 0..* ProductionProgressHistory
ProductionStageProgress 1 ───── 0..* ReworkRecord

User 1 ───── 0..* StoredFile
```

## 13.2 Important ERD Notes

* `Mall.Id`, `Brand.Id`, `Store.Id` and other entity IDs are the relational identifiers; display names are not used as foreign keys.
* `SavedContact` belongs to either a Brand or a Mall, but not both.
* `SurveyPersonnel` belongs to a SurveyVisit and stores name and phone only; it has no User relationship.
* `SurveyVisitFile` stores the category of each survey source file.
* Every `DrawingRevision` must have at least one associated `DrawingRevisionFile`; a revision may have multiple files, such as PDF and CAD files. The minimum-child requirement needs application-level enforcement; a foreign key alone cannot ensure it.
* `DrawingRevision.IsCurrent` identifies the current revision within each Project and DrawingStage.
* `ProductionOrder.InitialDrawingRevisionId` preserves the drawing originally issued to Factory.
* `ProductionOrder.CurrentDrawingRevisionId` identifies the drawing Factory should currently use.
* Different ProductionOrders within one Project may reference different ProductionOrder-stage DrawingRevisions.
* ProductionStageProgress links each ProductionOrder to a Workshop; `(ProductionOrderId, WorkshopId)` is unique so each Workshop has at most one progress record per order.
* `ProjectAssignment` records ProjectId, DesignerUserId, AssignedAt, EndedAt, and AssignedByUserId; `Project.ResponsibleDesignerId` remains the current-assignee reference.
* `ProductionProgressHistory` records ProductionStageProgressId, PreviousStatus, NewStatus, ChangedAt, and ChangedByUserId. `ProductionStageProgress.Status` remains current, and `ReworkRecord` remains independent.
* `JobPosition` is a predefined catalog with exactly one required JobPosition per User. `SystemRole` is a separate predefined catalog with a many-to-many User association; users cannot create roles.