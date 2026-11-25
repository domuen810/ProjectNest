using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectNest.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: true, computedColumnSql: "lower(regexp_replace(\"Name\", '^[[:space:]]+|[[:space:]]+$', '', 'g'))", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Malls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Malls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    JobPositionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_JobPositions_JobPositionId",
                        column: x => x.JobPositionId,
                        principalTable: "JobPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: true),
                    MallId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedContacts", x => x.Id);
                    table.CheckConstraint("CK_SavedContacts_ExactlyOneOwner", "(\"BrandId\" IS NOT NULL AND \"MallId\" IS NULL) OR (\"BrandId\" IS NULL AND \"MallId\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_SavedContacts_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedContacts_Malls_MallId",
                        column: x => x.MallId,
                        principalTable: "Malls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BrandId = table.Column<Guid>(type: "uuid", nullable: false),
                    MallId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stores_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Malls_MallId",
                        column: x => x.MallId,
                        principalTable: "Malls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StoredFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: false),
                    FileExtension = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    StorageKey = table.Column<string>(type: "text", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploadedById = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoredFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoredFiles_Users_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSystemRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemRoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSystemRoles", x => new { x.UserId, x.SystemRoleId });
                    table.ForeignKey(
                        name: "FK_UserSystemRoles_SystemRoles_SystemRoleId",
                        column: x => x.SystemRoleId,
                        principalTable: "SystemRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSystemRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectNumber = table.Column<string>(type: "text", nullable: false),
                    StoreId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectNature = table.Column<int>(type: "integer", nullable: false),
                    ProjectCategory = table.Column<int>(type: "integer", nullable: false),
                    OpeningDate = table.Column<DateOnly>(type: "date", nullable: false),
                    InitialArea = table.Column<decimal>(type: "numeric", nullable: true),
                    MeasuredArea = table.Column<decimal>(type: "numeric", nullable: true),
                    Floor = table.Column<string>(type: "text", nullable: true),
                    ShopNumber = table.Column<string>(type: "text", nullable: true),
                    InitialRequirements = table.Column<string>(type: "text", nullable: true),
                    ResponsibleDesignerId = table.Column<Guid>(type: "uuid", nullable: true),
                    BrandPaymentPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    MallPaymentPercentage = table.Column<decimal>(type: "numeric", nullable: false),
                    BrandNameSnapshot = table.Column<string>(type: "text", nullable: false),
                    MallNameSnapshot = table.Column<string>(type: "text", nullable: false),
                    BrandContactName = table.Column<string>(type: "text", nullable: true),
                    BrandContactPosition = table.Column<string>(type: "text", nullable: true),
                    BrandContactPhone = table.Column<string>(type: "text", nullable: true),
                    BrandContactEmail = table.Column<string>(type: "text", nullable: true),
                    MallContactName = table.Column<string>(type: "text", nullable: true),
                    MallContactPosition = table.Column<string>(type: "text", nullable: true),
                    MallContactPhone = table.Column<string>(type: "text", nullable: true),
                    MallContactEmail = table.Column<string>(type: "text", nullable: true),
                    CounterType = table.Column<int>(type: "integer", nullable: true),
                    HasIntegratedBeautyRoom = table.Column<bool>(type: "boolean", nullable: true),
                    RequiresTemporaryCounter = table.Column<bool>(type: "boolean", nullable: false),
                    TemporaryCounterNotes = table.Column<string>(type: "text", nullable: true),
                    HasExistingCounter = table.Column<bool>(type: "boolean", nullable: false),
                    ExistingCounterLocationRelation = table.Column<int>(type: "integer", nullable: true),
                    RequiresExistingCounterRemoval = table.Column<bool>(type: "boolean", nullable: false),
                    ExistingCounterRemovalReason = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.CheckConstraint("CK_Projects_CounterTypeByCategory", "(\"ProjectCategory\" = 0 AND \"CounterType\" IS NOT NULL) OR (\"ProjectCategory\" <> 0 AND \"CounterType\" IS NULL)");
                    table.CheckConstraint("CK_Projects_PaymentShares", "\"BrandPaymentPercentage\" BETWEEN 0 AND 100 AND \"MallPaymentPercentage\" BETWEEN 0 AND 100 AND \"BrandPaymentPercentage\" + \"MallPaymentPercentage\" = 100");
                    table.ForeignKey(
                        name: "FK_Projects_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_ResponsibleDesignerId",
                        column: x => x.ResponsibleDesignerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrawingRevisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DrawingStage = table.Column<int>(type: "integer", nullable: false),
                    RevisionNumber = table.Column<string>(type: "text", nullable: false),
                    ChangeScope = table.Column<int>(type: "integer", nullable: false),
                    ChangeReason = table.Column<int>(type: "integer", nullable: false),
                    ChangeSummary = table.Column<string>(type: "text", nullable: true),
                    FeedbackOrChangeRequest = table.Column<string>(type: "text", nullable: true),
                    AffectedPagesOrAreas = table.Column<string>(type: "text", nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingRevisions", x => x.Id);
                    table.CheckConstraint("CK_DrawingRevisions_AffectedPagesForPartial", "\"ChangeScope\" <> 1 OR (\"AffectedPagesOrAreas\" IS NOT NULL AND regexp_replace(\"AffectedPagesOrAreas\", '^[[:space:]]+|[[:space:]]+$', '', 'g') <> '')");
                    table.ForeignKey(
                        name: "FK_DrawingRevisions_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DrawingRevisions_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MeasuredAreaChanges",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousValue = table.Column<decimal>(type: "numeric", nullable: true),
                    NewValue = table.Column<decimal>(type: "numeric", nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredAreaChanges", x => new { x.ProjectId, x.Id });
                    table.ForeignKey(
                        name: "FK_MeasuredAreaChanges_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasuredAreaChanges_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpeningDateChanges",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousValue = table.Column<DateOnly>(type: "date", nullable: false),
                    NewValue = table.Column<DateOnly>(type: "date", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpeningDateChanges", x => new { x.ProjectId, x.Id });
                    table.ForeignKey(
                        name: "FK_OpeningDateChanges_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpeningDateChanges_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DesignerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AssignedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectAssignments_Users_DesignerUserId",
                        column: x => x.DesignerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatusChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousStatus = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatusChanges", x => x.Id);
                    table.CheckConstraint("CK_ProjectStatusChanges_ReasonForPausedOrCancelled", "\"NewStatus\" NOT IN (1, 2) OR (\"Reason\" IS NOT NULL AND regexp_replace(\"Reason\", '^[[:space:]]+|[[:space:]]+$', '', 'g') <> '')");
                    table.ForeignKey(
                        name: "FK_ProjectStatusChanges_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectStatusChanges_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyWorkOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    SurveyType = table.Column<int>(type: "integer", nullable: false),
                    PlannedSurveyDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    FinalSurveyReportFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyWorkOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyWorkOrders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyWorkOrders_StoredFiles_FinalSurveyReportFileId",
                        column: x => x.FinalSurveyReportFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyWorkOrders_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyWorkOrders_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DrawingRevisionFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DrawingRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoredFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileRole = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawingRevisionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrawingRevisionFiles_DrawingRevisions_DrawingRevisionId",
                        column: x => x.DrawingRevisionId,
                        principalTable: "DrawingRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DrawingRevisionFiles_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    InitialDrawingRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentDrawingRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionOrders_DrawingRevisions_CurrentDrawingRevisionId",
                        column: x => x.CurrentDrawingRevisionId,
                        principalTable: "DrawingRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionOrders_DrawingRevisions_InitialDrawingRevisionId",
                        column: x => x.InitialDrawingRevisionId,
                        principalTable: "DrawingRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionOrders_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionOrders_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionOrders_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyVisits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SurveyWorkOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlannedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ActualVisitDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    VisitReportFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    Findings = table.Column<string>(type: "text", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    NoResultReason = table.Column<string>(type: "text", nullable: true),
                    ConfirmedByDesignerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyVisits_StoredFiles_VisitReportFileId",
                        column: x => x.VisitReportFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyVisits_SurveyWorkOrders_SurveyWorkOrderId",
                        column: x => x.SurveyWorkOrderId,
                        principalTable: "SurveyWorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyVisits_Users_ConfirmedByDesignerUserId",
                        column: x => x.ConfirmedByDesignerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionStageProgress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkshopId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionStageProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionStageProgress_ProductionOrders_ProductionOrderId",
                        column: x => x.ProductionOrderId,
                        principalTable: "ProductionOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionStageProgress_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurveyPersonnel",
                columns: table => new
                {
                    SurveyVisitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyPersonnel", x => new { x.SurveyVisitId, x.Id });
                    table.ForeignKey(
                        name: "FK_SurveyPersonnel_SurveyVisits_SurveyVisitId",
                        column: x => x.SurveyVisitId,
                        principalTable: "SurveyVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SurveyVisitFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SurveyVisitId = table.Column<Guid>(type: "uuid", nullable: false),
                    StoredFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyVisitFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyVisitFiles_StoredFiles_StoredFileId",
                        column: x => x.StoredFileId,
                        principalTable: "StoredFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyVisitFiles_SurveyVisits_SurveyVisitId",
                        column: x => x.SurveyVisitId,
                        principalTable: "SurveyVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductionProgressHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionStageProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousStatus = table.Column<int>(type: "integer", nullable: false),
                    NewStatus = table.Column<int>(type: "integer", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionProgressHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionProgressHistory_ProductionStageProgress_Productio~",
                        column: x => x.ProductionStageProgressId,
                        principalTable: "ProductionStageProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductionProgressHistory_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReworkRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductionStageProgressId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReasonType = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    RecordedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReworkRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReworkRecords_ProductionStageProgress_ProductionStageProgre~",
                        column: x => x.ProductionStageProgressId,
                        principalTable: "ProductionStageProgress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReworkRecords_Users_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Brands_NormalizedName",
                table: "Brands",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DrawingRevisionFiles_DrawingRevisionId",
                table: "DrawingRevisionFiles",
                column: "DrawingRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingRevisionFiles_StoredFileId",
                table: "DrawingRevisionFiles",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingRevisions_CreatedByUserId",
                table: "DrawingRevisions",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingRevisions_ProjectId_DrawingStage",
                table: "DrawingRevisions",
                columns: new[] { "ProjectId", "DrawingStage" },
                unique: true,
                filter: "\"IsCurrent\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_DrawingRevisions_ProjectId_DrawingStage_RevisionNumber",
                table: "DrawingRevisions",
                columns: new[] { "ProjectId", "DrawingStage", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Malls_City_Name",
                table: "Malls",
                columns: new[] { "City", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredAreaChanges_ChangedByUserId",
                table: "MeasuredAreaChanges",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpeningDateChanges_ChangedByUserId",
                table: "OpeningDateChanges",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_CreatedByUserId",
                table: "ProductionOrders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_CurrentDrawingRevisionId",
                table: "ProductionOrders",
                column: "CurrentDrawingRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_InitialDrawingRevisionId",
                table: "ProductionOrders",
                column: "InitialDrawingRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_ProjectId_OrderNumber",
                table: "ProductionOrders",
                columns: new[] { "ProjectId", "OrderNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionOrders_UpdatedByUserId",
                table: "ProductionOrders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionProgressHistory_ChangedByUserId",
                table: "ProductionProgressHistory",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionProgressHistory_ProductionStageProgressId",
                table: "ProductionProgressHistory",
                column: "ProductionStageProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionStageProgress_ProductionOrderId_WorkshopId",
                table: "ProductionStageProgress",
                columns: new[] { "ProductionOrderId", "WorkshopId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductionStageProgress_WorkshopId",
                table: "ProductionStageProgress",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_AssignedByUserId",
                table: "ProjectAssignments",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_DesignerUserId",
                table: "ProjectAssignments",
                column: "DesignerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectAssignments_ProjectId",
                table: "ProjectAssignments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedByUserId",
                table: "Projects",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectNumber",
                table: "Projects",
                column: "ProjectNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ResponsibleDesignerId",
                table: "Projects",
                column: "ResponsibleDesignerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StoreId",
                table: "Projects",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UpdatedByUserId",
                table: "Projects",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatusChanges_ChangedByUserId",
                table: "ProjectStatusChanges",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatusChanges_ProjectId",
                table: "ProjectStatusChanges",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReworkRecords_ProductionStageProgressId",
                table: "ReworkRecords",
                column: "ProductionStageProgressId");

            migrationBuilder.CreateIndex(
                name: "IX_ReworkRecords_RecordedByUserId",
                table: "ReworkRecords",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedContacts_BrandId",
                table: "SavedContacts",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedContacts_MallId",
                table: "SavedContacts",
                column: "MallId");

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_StorageKey",
                table: "StoredFiles",
                column: "StorageKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoredFiles_UploadedById",
                table: "StoredFiles",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BrandId",
                table: "Stores",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_MallId",
                table: "Stores",
                column: "MallId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyVisitFiles_StoredFileId",
                table: "SurveyVisitFiles",
                column: "StoredFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyVisitFiles_SurveyVisitId",
                table: "SurveyVisitFiles",
                column: "SurveyVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyVisits_ConfirmedByDesignerUserId",
                table: "SurveyVisits",
                column: "ConfirmedByDesignerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyVisits_SurveyWorkOrderId",
                table: "SurveyVisits",
                column: "SurveyWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyVisits_VisitReportFileId",
                table: "SurveyVisits",
                column: "VisitReportFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyWorkOrders_CreatedByUserId",
                table: "SurveyWorkOrders",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyWorkOrders_FinalSurveyReportFileId",
                table: "SurveyWorkOrders",
                column: "FinalSurveyReportFileId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyWorkOrders_ProjectId",
                table: "SurveyWorkOrders",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyWorkOrders_UpdatedByUserId",
                table: "SurveyWorkOrders",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_JobPositionId",
                table: "Users",
                column: "JobPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSystemRoles_SystemRoleId",
                table: "UserSystemRoles",
                column: "SystemRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawingRevisionFiles");

            migrationBuilder.DropTable(
                name: "MeasuredAreaChanges");

            migrationBuilder.DropTable(
                name: "OpeningDateChanges");

            migrationBuilder.DropTable(
                name: "ProductionProgressHistory");

            migrationBuilder.DropTable(
                name: "ProjectAssignments");

            migrationBuilder.DropTable(
                name: "ProjectStatusChanges");

            migrationBuilder.DropTable(
                name: "ReworkRecords");

            migrationBuilder.DropTable(
                name: "SavedContacts");

            migrationBuilder.DropTable(
                name: "SurveyPersonnel");

            migrationBuilder.DropTable(
                name: "SurveyVisitFiles");

            migrationBuilder.DropTable(
                name: "UserSystemRoles");

            migrationBuilder.DropTable(
                name: "ProductionStageProgress");

            migrationBuilder.DropTable(
                name: "SurveyVisits");

            migrationBuilder.DropTable(
                name: "SystemRoles");

            migrationBuilder.DropTable(
                name: "ProductionOrders");

            migrationBuilder.DropTable(
                name: "Workshops");

            migrationBuilder.DropTable(
                name: "SurveyWorkOrders");

            migrationBuilder.DropTable(
                name: "DrawingRevisions");

            migrationBuilder.DropTable(
                name: "StoredFiles");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Malls");

            migrationBuilder.DropTable(
                name: "JobPositions");
        }
    }
}
