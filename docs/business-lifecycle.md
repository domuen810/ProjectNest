ProjectNest — Full Business Lifecycle
│
├── Business Context
│   │
│   └── 1. Annual Client / Supplier Arrangement
│       ├── Brand annual store opening and renovation plan
│       ├── Brand completes early coordination with Mall
│       │   ├── Commercial negotiation
│       │   ├── Store / Counter location selection
│       │   └── Contract arrangement
│       ├── Regional supplier tender / qualification
│       └── Supplier appointed for the Brand, region and year
│
├── MVP — Current ProjectNest Scope
│   │
│   ├── 2. Project Initiation
│   │   ├── Client issues project to Team Manager via email
│   │   ├── Project information may include:
│   │   │   ├── Brand
│   │   │   ├── City
│   │   │   ├── Mall
│   │   │   ├── Mall Type: New / Existing (descriptive only; not currently modeled in the Domain or used by MVP workflows)
│   │   │   ├── Store Location
│   │   │   ├── Area
│   │   │   ├── Opening Date
│   │   │   ├── Client Contact
│   │   │   ├── Mall Contact
│   │   │   ├── Project Nature: New / Renovation
│   │   │   ├── Project Category: Permanent Counter / Independent Beauty Room
│   │   │   ├── Counter Type for Permanent Counter: Full Island / Half Island / Combined Counter / Wall Counter
│   │   │   ├── Temporary Counter: optional configuration of a Permanent Counter Project
│   │   │   ├── Brand and Mall payment percentages, totaling 100%
│   │   │   ├── Temporary Counter Requirement: Yes / No
│   │   │   ├── Temporary Counter Notes
│   │   │   ├── Initial Requirements
│   │   │   └── Mall Drawings / Reference Files
│   │   ├── Team Manager receives project
│   │   ├── Team Manager Assistant creates project
│   │   ├── Project may initially have no responsible Designer
│   │   ├── Responsible Designer can be assigned later; assignment changes are retained
│   │   │   ├── Project.ResponsibleDesignerId stores the current responsible Designer
│   │   │   └── ProjectAssignment records ProjectId, DesignerUserId, AssignedAt, EndedAt and AssignedByUserId
│   │
│   ├── 3. Initial Planning & Coordination
│   │   ├── Opening Date drives initial schedule
│   │   ├── Design, Engineering and Factory review project
│   │   ├── Site Survey timing planned
│   │   └── Factory starts general preparation that does not depend
│   │       on detailed drawings
│   │
│   ├── 4. Initial Site Survey Preparation
│   │   ├── Designer reviews Mall drawings and available project information
│   │   ├── Designer identifies missing or unclear information to verify on site
│   │   ├── Team Manager Assistant creates Site Survey Work Order
│   │   ├── Mall Contact is contacted to arrange survey
│   │   └── Engineering coordinates Survey Personnel for the visit
│   │
│   ├── 5. Initial Site Survey
│   │   ├── Initial Survey may require one or multiple Survey Visits
│   │   │
│   │   ├── Each Survey Visit records:
│   │   │   ├── Visit Date
│   │   │   ├── Survey Personnel (name and phone; not linked to User)
│   │   │   ├── Completion Status
│   │   │   ├── Findings / Notes
│   │   │   ├── Survey Source Materials
│   │   │   │   ├── Site Photos
│   │   │   │   ├── Measurement Sketches
│   │   │   │   ├── Mall-provided Files
│   │   │   │   ├── Survey Form
│   │   │   │   └── Other Supporting Files
│   │   │   └── Survey Visit Report
│   │   │
│   │   ├── Survey Work Order and Visit statuses: Planned, InProgress, Completed, Cancelled
│   │   ├── Survey Personnel may differ between Survey Visits
│   │   ├── Additional Survey Visits arranged when required
│   │   ├── Each Survey Visit Report retained
│   │   │   └── Important site changes can be traced across visits
│   │   ├── Initial Survey completed when required site information is complete
│   │   ├── Designer prepares Final Survey Report
│   │   │   └── Final client-facing PDF retained
│   │   └── Final Survey Report sent to Client
│   │
│   ├── 6. Concept / Initial Design
│   │   ├── Designer develops design based on:
│   │   │   ├── Client Requirements
│   │   │   ├── Brand Guidelines
│   │   │   ├── Survey Information
│   │   │   └── Mall Conditions
│   │   └── Initial Design Package produced
│   │       ├── Layouts
│   │       ├── Elevations
│   │       └── 3D Visualisations
│   │
│   ├── 7. Client Design Review
│   │   ├── Designer communicates directly with Client Design Team
│   │   ├── Client provides feedback
│   │   ├── Designer revises design
│   │   └── Client confirms design
│   │
│   ├── 8. Mall Submission & Sign-off
│   │   ├── Design submitted to Mall
│   │   ├── Mall reviews design
│   │   ├── Designer makes required revisions
│   │   ├── Design resubmitted when required
│   │   └── Final Mall-signed DrawingRevision package retained (no separate DrawingSet entity)
│   │
│   ├── 9. Construction Drawing Development
│   │   ├── Mall-signed drawings returned to Brand for review
│   │   ├── Brand confirms detailed drawing development
│   │   ├── Designer develops Construction Drawings
│   │   ├── Brand reviews Construction Drawings
│   │   ├── Designer makes required revisions
│   │   └── Construction Drawings confirmed
│   │
│   ├── 10. Production / Order Drawing Development
│   │   ├── Construction Drawings further detailed for production
│   │   ├── Manufacturing-specific details added
│   │   └── Production / Order Drawing prepared
│   │
│   ├── 11. Re-measurement / Verification Survey
│   │   ├── Verification Survey arranged before Factory Order
│   │   ├── Verification Survey may require one or multiple Survey Visits
│   │   ├── Survey Visits follow the same visit and file-recording structure
│   │   │   as the Initial Survey
│   │   ├── Initial Survey and Production / Order Drawing checked
│   │   │   against current site conditions
│   │   ├── Site-condition changes and discrepancies recorded
│   │   └── Production / Order Drawing updated when required
│   │
│   ├── 12. Final Order Drawing & Factory Order
│   │   ├── Designer performs final check
│   │   ├── Final Order Drawing confirmed
│   │   ├── Final drawing revision and associated files uploaded to ProjectNest
│   │   ├── Final drawings issued to Factory
│   │   └── Factory uses Final Order Drawing as production basis
│   │
│   ├── 13. Factory Production
│   │   ├── Factory Manager schedules production
│   │   ├── Production stages may include:
│   │   │   ├── Carpentry
│   │   │   ├── Painting
│   │   │   ├── Glass
│   │   │   ├── Metal
│   │   │   ├── Electrical
│   │   │   ├── Pre-assembly
│   │   │   └── Packing & Shipping
│   │   ├── Each ProductionOrder has one progress record per Workshop
│   │   ├── Workshop progress can start independently and may overlap
│   │   ├── ProductionProgressHistory records previous/new status, time and changing User
│   │   ├── ProductionStageProgress retains the current status
│   │   └── ReworkRecord independently retains rework history
│   │
│   └── 14. Production Drawing Changes
│       ├── Drawing changes may result from:
│       │   ├── Design Changes
│       │   ├── Site Condition Changes
│       │   ├── Client Changes
│       │   └── Manufacturing Issues
│       ├── Local / independent change
│       │   ├── Revised sheets / pages / areas uploaded
│       │   └── Affected areas recorded
│       ├── Wider / related change
│       │   └── Updated CAD + PDF drawing package uploaded as DrawingRevision files
│       ├── Each DrawingRevision has at least one StoredFile; multiple files are allowed
│       ├── Previous revisions retained
│       └── Current Production Drawing maintained
│
└── Phase 2 — Future ProjectNest Scope
    │
    ├── 15. Third-party & Procured Items
    │   ├── Company-procured items tracked
    │   ├── Brand-appointed supplier items tracked
    │   └── Delivery coordinated with project schedule
    │
    ├── 16. Logistics
    │   ├── Packing completed
    │   ├── Shipment contents confirmed
    │   ├── Dispatch arranged
    │   ├── Dispatch Time and ETA recorded
    │   ├── Vehicle / Driver Contact recorded
    │   └── Goods delivered to site
    │
    ├── 17. Site Preparation
    │   ├── Engineering plans backwards from Opening Date
    │   ├── Mall entry requirements managed
    │   ├── Construction permits / procedures managed
    │   ├── Internal workers / local contractors arranged
    │   ├── Site preparation completed
    │   └── Existing fixtures removed when required
    │
    ├── 18. Temporary Counter Work
    │   ├── Existing counter removed when required
    │   ├── Temporary counter installed
    │   ├── Temporary counter operates during project works
    │   └── Temporary counter removed
    │
    ├── 19. Site Construction & Installation
    │   ├── Factory products arrive
    │   ├── Site construction and installation performed
    │   ├── Finishing / adjustment completed
    │   └── Engineering coordinates site issues
    │
    ├── 20. Final Inspection, Rectification & Acceptance
    │   ├── Internal inspection
    │   ├── Client inspection
    │   ├── Mall inspection
    │   ├── Issues recorded
    │   ├── Rectification completed
    │   ├── Re-inspection when required
    │   └── Signed Inspection / Acceptance Record retained
    │
    ├── 21. Mall Exit & Deposit Administration
    │   ├── Site exit procedures completed
    │   ├── Required documents submitted
    │   ├── Mall deposit followed up
    │   └── Deposit refund completed
    │
    ├── 22. Store Opening
    │   └── Store opens for operation
    │
    ├── 23. Warranty & Maintenance
    │   ├── Warranty period tracked
    │   ├── Maintenance request received
    │   ├── Issue assessed
    │   ├── Repair / replacement arranged
    │   ├── Internal staff / local contractor arranged
    │   └── Maintenance record retained
    │
    └── Cross-lifecycle Quality Control (QC)
        ├── Factory Production
        │   └── Production Quality Inspection
        ├── Pre-assembly
        │   └── Internal Inspection
        ├── Site Installation
        │   └── Installation Quality Inspection
        ├── Before Opening
        │   └── Pre-opening Inspection
        └── Project Completion
            └── Project Review / Lessons Learned