# ProjectNest Requirements

## MVP Scope

The MVP focuses on the core operational workflow from project creation through design and factory production visibility.

The MVP includes:

* Authentication and role-based access
* Project creation and assignment
* Site survey workflow and survey documents
* Drawing sets, revisions and drawing changes
* Production order and production progress
* Core project information view
* Operational history and audit metadata

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
* Each project has a responsible Designer.
* Internal teams can view the current project status and key project information.

### Site Survey

* A Site Survey Work Order can be created for a project.
* Survey personnel can be assigned to the work order.
* The signed survey pack and related survey documents can be stored with the project.

### Drawing Management

* Drawing sets and their revisions are retained as part of the project history.
* Users can clearly identify the current drawing used for production.
* Drawing changes can record the affected sheets, pages or areas.
* Previous drawing revisions and change records remain accessible.

### Production Visibility

* Production information is linked to the relevant project and production drawing.
* Factory users can update the current production stage and progress.
* Internal users can view the latest production status without relying on manual communication.

## Important Business Rules

* A project represents a specific piece of work for a store, such as a new store or renovation. A store may have multiple projects over time.
* Each project has one responsible Designer.
* A Site Survey Work Order belongs to one project.
* The signed survey pack is retained as part of the project record.
* The final mall-signed drawing set must be retained as part of the project record.
* Production drawings are developed based on the final mall-signed drawing set.
* The current production drawing must be clearly identifiable.
* Previous drawing revisions must remain accessible and must not be overwritten.
* Drawing changes must record the affected sheets, pages or areas.
* Factory production information must be linked to the relevant project and production drawing.
* Production progress is tracked at the project or production-order level rather than at individual fixture level.
