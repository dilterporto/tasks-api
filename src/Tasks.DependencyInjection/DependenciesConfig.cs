using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tasks.Persistence;

namespace Tasks.DependencyInjection;

public static class DependenciesConfig
{
  public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
  {
    // configure persistence
    services.ConfigurePersistence(configuration);
  }
}
