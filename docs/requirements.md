# ProjectNest Requirements

## MVP Scope

The MVP focuses on the core operational workflow from project creation through design and factory production visibility.

The MVP includes:

* Authentication and role-based access
* Internal User accounts and simple RBAC using a predefined SystemRole catalog
* A predefined JobPosition catalog; every User has exactly one JobPosition, independent of their SystemRoles
* Project creation and assignment
* Site survey workflow and survey documents
* Project-scoped drawing revisions and drawing changes; a DrawingSet is not a separate entity
* Production order and production progress
* Core project information view
* Key operational history and audit metadata
* `StoredFile` metadata for survey and drawing files
* At least one StoredFile association for each DrawingRevision; revisions may have multiple files

The following areas are outside the initial MVP:

* Engineering and site construction management
* Procurement and third-party supplier tracking
* Logistics
* Final acceptance
* Warranty and maintenance
* Finance
* Client portal
* Analytics and reporting

## Core Business Requirements

### Project Management

* Internal users can create and maintain projects using the basic information provided by the client.
* ProjectNature (`New`, `Renovation`) and ProjectCategory (`PermanentCounter`, `IndependentBeautyRoom`) are separate dimensions.
* PermanentCounter projects require a CounterType (`FullIsland`, `HalfIsland`, `CombinedCounter`, `WallCounter`); temporary counters are optional configuration, not separate Projects or categories.
* Independent beauty-room Projects are separate Projects and have no direct Project-to-Project relationship.
* Project payment allocation is represented by structured Brand and Mall percentages totaling 100%; this is not contract instalment scheduling.
* A project may be created without a responsible Designer; a Designer can be assigned later.
* Changes to the responsible Designer are retained as history.
* After first assignment, a Project cannot be left unassigned; changing assignment ends the prior history record and adds a new one.
* Internal teams can view the current project status and key project information.
* Pausing or cancelling a Project requires a reason; other status changes do not require one.
* Store remains a separate Brand-in-Mall location with lifecycle `PreOpening`, `Open`, `Closed` and a one-to-many relationship to Project; counter classification belongs to Project.
* SavedContact is an MVP reusable contact directory associated with either a Brand or a Mall.

### Site Survey

* A Site Survey Work Order can be created for a project.
* SurveyType is `InitialSurvey`, `ReMeasurement`, or `FollowUpSurvey`.
* SurveyWorkOrder and SurveyVisit use `Planned`, `InProgress`, `Completed`, and `Cancelled` statuses.
* A SurveyWorkOrder may contain multiple SurveyVisits; each visit may link multiple StoredFiles, while the work order may link at most one final report StoredFile.
* Survey personnel are recorded per SurveyVisit using name and phone; they are not Users.
* The signed survey pack and related survey documents can be stored with the project.

### Drawing Management

* Drawing revisions are retained under a Project; "drawing set" describes the drawing package, not a separate entity.
* DrawingStage is `InitialDesign`, `MallSigned`, `Construction`, or `ProductionOrder`; `MallSigned` represents Mall approval.
* Users can clearly identify the current drawing used for production.
* DrawingRevision retains ChangeReason, ChangeSummary, FeedbackOrChangeRequest, and AffectedPagesOrAreas as text; affected pages or areas are not separate entities.
* Each DrawingRevision has at least one StoredFile, and may have several files (for example, PDF and CAD).
* Previous drawing revisions and change records remain accessible.

### Production Visibility

* Production information is linked to the relevant project and production drawing.
* Each ProductionOrder has an OrderNumber, unique within its Project; numbering is entered manually in the MVP.
* ProductionOrder status is `Planned`, `InProduction`, `Completed`, or `Cancelled`.
* Each ProductionOrder has at most one ProductionStageProgress per Workshop.
* Workshop progress can start independently and overlap with progress in other Workshops.
* Workshop is a fixed catalogue of seven MVP operations: Carpentry, Painting, Glass, Metal, Electrical, Pre-assembly, and Packing & Shipping; arbitrary entries cannot be created or renamed. ProductionStageProgress status includes `Blocked`.
* Whether an existing counter requires demolition is a Project-level Boolean option for Engineering work planning; no reason is required when demolition is not selected.
* Factory users can update Workshop progress.
* Internal users can view the latest production status without relying on manual communication.

## Important Business Rules

* A project represents a specific piece of work for a store, such as a new store or renovation. A store may have multiple projects over time.
* ProjectNature and ProjectCategory are independent; a temporary counter is not a Project.
* A project may initially have no responsible Designer and can be assigned one later.
* Responsible Designer assignment changes are retained as history.
* A Site Survey Work Order belongs to one project.
* SurveyType values are `InitialSurvey`, `ReMeasurement`, and `FollowUpSurvey`; SurveyWorkOrder and SurveyVisit share the four confirmed lifecycle statuses.
* SurveyPersonnel belongs to a SurveyVisit and records only name and phone; it has no User relationship.
* The signed survey pack is retained as part of the project record.
* The final mall-signed drawing revisions and their associated files are retained as part of the project record; there is no separate DrawingSet entity.
* Production drawings are developed based on the final mall-signed drawing set.
* The current production drawing must be clearly identifiable.
* Previous drawing revisions must remain accessible and must not be overwritten.
* Drawing changes must record the affected sheets, pages or areas.
* Each DrawingRevision must retain ChangeReason, ChangeSummary, FeedbackOrChangeRequest, and AffectedPagesOrAreas.
* Factory production information must be linked to the relevant project and production drawing.
* Production progress is tracked per ProductionOrder and Workshop, not at individual fixture level.
* Changes to measured area require a recorded reason.
* Users can be deactivated and reactivated; deactivation does not delete a User.
* Each Project's Brand/Mall payment percentages total 100%; the values describe cost allocation, not instalments.
* Job Position is distinct from System Role. The MVP uses simple role-based access control (RBAC).
* SystemRole is a predefined catalog; users cannot create roles. A User may have multiple SystemRoles, and User-SystemRole is many-to-many.
* JobPosition is a separate predefined catalog. Every User has exactly one required JobPosition; it does not itself grant permissions.
* Key operational history records Project status changes, responsible Designer assignment changes, DrawingRevision updates, and ProductionStageProgress changes.
* Project assignment history records ProjectId, DesignerUserId, AssignedAt, EndedAt and AssignedByUserId; Project.ResponsibleDesignerId continues to identify the current responsible Designer.
* ProductionProgressHistory records ProductionStageProgressId, PreviousStatus, NewStatus, ChangedAt and ChangedByUserId. ProductionStageProgress retains the current status; ReworkRecord remains a separate rework history.
* Existing generic audit fields, such as `CreatedByUserId` and `UpdatedByUserId`, are retained on Domain entities where they already exist and are not required on every Domain entity. Audit metadata population and handling will be managed centrally in the Application/Infrastructure layers in a later implementation stage. Explicit actor IDs in business-history records remain separate and unchanged.
* Ordinary page views and button clicks are not recorded as operational history.
* A drawing version update is recorded as a new DrawingRevision; previous revisions are not overwritten.

## File Storage

`StoredFile` stores file metadata and can be associated with records such as `DrawingRevision` and `SurveyVisit`. File binaries are stored in the file system or equivalent storage; no specific storage provider is required by the MVP.

Each DrawingRevision must be associated with at least one StoredFile and may have multiple associated files, such as PDF and CAD files.
