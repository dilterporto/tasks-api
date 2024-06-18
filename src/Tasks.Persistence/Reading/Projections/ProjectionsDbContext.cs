using Microsoft.EntityFrameworkCore;
using Tasks.Persistence.Reading.Projections.ModelConfiguration;

namespace Tasks.Persistence.Reading.Projections;

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
