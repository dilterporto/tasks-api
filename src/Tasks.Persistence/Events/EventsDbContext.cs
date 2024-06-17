using Microsoft.EntityFrameworkCore;

namespace Tasks.Persistence.Events;

public class EventsDbContext : DbContext
{
  public EventsDbContext(DbContextOptions<EventsDbContext> options)
    : base(options)
  {

  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new EventModelConfiguration());
  }
}
