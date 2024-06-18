namespace Tasks.Abstractions.Domain;

public interface IEntity : IEntity<Guid>
{

}

public interface IEntity<TId>
{
  TId Id { get; set; }
}
