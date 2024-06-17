using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Tasks.Abstractions.EventSourcing;

namespace Tasks.Persistence.Projections;

public class ProjectionsReader<TProjection>(ProjectionsDbContext projectionsDbContext) : IProjectionsReader<TProjection> where TProjection : Projection
{
  public async Task<Maybe<TProjection>> GetByIdAsync(Guid id)
  {
    var projection = await projectionsDbContext
      .Set<TProjection>()
      .TagWith($"GetByIdAsync - {typeof(TProjection).Name}")
      .AsNoTracking()
      .FirstOrDefaultAsync(x => x.Id == id);

    return projection == null ? Maybe<TProjection>.None : Maybe<TProjection>.From(projection);
  }

  public Task<IEnumerable<TProjection>> GetAllAsync()
  {
    throw new NotImplementedException();
  }
}
