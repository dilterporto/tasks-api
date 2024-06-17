using CSharpFunctionalExtensions;

namespace Tasks.Abstractions.EventSourcing;

public interface IProjectionsReader<TProjection> where TProjection : Projection
{
  Task<Maybe<TProjection>> GetByIdAsync(Guid id);
  Task<IQueryable<TProjection>> GetAllAsync();
}
