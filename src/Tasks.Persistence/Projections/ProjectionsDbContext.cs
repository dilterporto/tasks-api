using Microsoft.EntityFrameworkCore;
using Tasks.Persistence.Projections.ModelConfiguration;

namespace Tasks.Persistence.Projections;

public class ProjectionsDbContext : DbContext
{
  public ProjectionsDbContext(DbContextOptions<ProjectionsDbContext> options) 
    : base(options)
  {
    
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new TaskModelConfiguration());
  }
}
