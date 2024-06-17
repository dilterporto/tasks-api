using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Abstractions;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;
using Tasks.Persistence.Events;
using Tasks.Persistence.Projections;
using Tasks.Persistence.Projections.EventCommitters;
using Tasks.Persistence.Repositories;

namespace Tasks.Persistence;

public static class DependencyExtensions
{
  public static IServiceCollection ConfigurePersistence(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<EventsDbContext>(o =>
      o.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    services.AddDbContext<ProjectionsDbContext>(o =>
      o.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
    
    services.AddScoped<ITaskRepository, TaskRepository>();

    services.AddScoped<IEventCommiters, EventCommitters>();
    services.AddScoped<IEventCommitter<TaskCreatedEvent>, TaskCreatedEventCommitter>();
    services.AddScoped<IEventCommitter<TaskChangedEvent>, TaskChangedEventCommitter>();
    services.AddScoped<IEventCommitter<TaskDeletedEvent>, TaskDeletedEventCommitter>();
    
    return services;
  }
}
