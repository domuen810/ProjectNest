using Microsoft.EntityFrameworkCore;

namespace ProjectNest.Infrastructure.Persistence;

public class ProjectNestDbContext : DbContext
{
    public ProjectNestDbContext(DbContextOptions<ProjectNestDbContext> options)
        : base(options)
    {
    }
}
