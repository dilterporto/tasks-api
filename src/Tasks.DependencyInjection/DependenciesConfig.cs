using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Persistence;

namespace Tasks.DependencyInjection;

public static class DependenciesConfig
{
  public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
  {
    services.ConfigurePersistence(configuration);

    var postgresConnection = configuration.GetConnectionString("DefaultConnection")!;
    var redisConnection = configuration.GetSection("Redis:Server").Value!;

    services.AddHealthChecks()
      .AddNpgSql(postgresConnection, name: "postgresql-events", tags: ["readiness"])
      .AddNpgSql(postgresConnection, name: "postgresql-projections", tags: ["readiness"])
      .AddRedis(redisConnection, name: "redis", tags: ["readiness"]);
  }
}
