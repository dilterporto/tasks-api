﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.EventSourcing;
using Tasks.Domain.Aggregates.Tasks;
using Tasks.Persistence.Events;
using Tasks.Persistence.Reading;
using Tasks.Persistence.Reading.Projections;
using Tasks.Persistence.Reading.Projections.EventCommitters;
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
    services.AddScoped<IProjectionsReader<TaskProjection>, ProjectionsReader<TaskProjection>>();
    
    services.AddScoped<IEventCommiters, EventCommitters>();
    services.AddScoped<IEventCommitter<TaskCreatedEvent>, TaskCreatedEventCommitter>();
    services.AddScoped<IEventCommitter<TaskChangedEvent>, TaskChangedEventCommitter>();
    services.AddScoped<IEventCommitter<TaskDeletedEvent>, TaskDeletedEventCommitter>();
    services.AddScoped<IEventCommitter<TaskStartedEvent>, TaskStartedEventCommitter>();

    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
    services.AddScoped<IDatabase>(o =>
    {
      var muxer = o.GetRequiredService<IConnectionMultiplexer>();
      return muxer.GetDatabase();
    });

    services.AddScoped<ICacheManager, CacheManager>();
    
    return services;
  }
}
