using Microsoft.EntityFrameworkCore;
using ProjectNest.Domain.Brands;
using ProjectNest.Domain.Contacts;
using ProjectNest.Domain.Drawings;
using ProjectNest.Domain.Files;
using ProjectNest.Domain.Identity;
using ProjectNest.Domain.Malls;
using ProjectNest.Domain.Production;
using ProjectNest.Domain.Projects;
using ProjectNest.Domain.Stores;
using ProjectNest.Domain.Surveys;

namespace ProjectNest.Infrastructure.Persistence;

public class ProjectNestDbContext : DbContext
{
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Mall> Malls => Set<Mall>();
    public DbSet<Store> Stores => Set<Store>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectAssignment> ProjectAssignments => Set<ProjectAssignment>();
    public DbSet<ProjectStatusChange> ProjectStatusChanges => Set<ProjectStatusChange>();
    public DbSet<SavedContact> SavedContacts => Set<SavedContact>();
    public DbSet<SurveyWorkOrder> SurveyWorkOrders => Set<SurveyWorkOrder>();
    public DbSet<SurveyVisit> SurveyVisits => Set<SurveyVisit>();
    public DbSet<SurveyVisitFile> SurveyVisitFiles => Set<SurveyVisitFile>();
    public DbSet<DrawingRevision> DrawingRevisions => Set<DrawingRevision>();
    public DbSet<DrawingRevisionFile> DrawingRevisionFiles => Set<DrawingRevisionFile>();
    public DbSet<ProductionOrder> ProductionOrders => Set<ProductionOrder>();
    public DbSet<Workshop> Workshops => Set<Workshop>();
    public DbSet<ProductionStageProgress> ProductionStageProgresses => Set<ProductionStageProgress>();
    public DbSet<ProductionProgressHistory> ProductionProgressHistories => Set<ProductionProgressHistory>();
    public DbSet<ReworkRecord> ReworkRecords => Set<ReworkRecord>();
    public DbSet<User> Users => Set<User>();
    public DbSet<SystemRole> SystemRoles => Set<SystemRole>();
    public DbSet<JobPosition> JobPositions => Set<JobPosition>();
    public DbSet<StoredFile> StoredFiles => Set<StoredFile>();

    public ProjectNestDbContext(DbContextOptions<ProjectNestDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProjectNestDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
