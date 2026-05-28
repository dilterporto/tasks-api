using Mapster;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.ChangeTask;
using Tasks.Application.UseCases.CreateTask;
using Tasks.Domain.Aggregates.Tasks;
using Tasks.Domain.Projections;

namespace Tasks.Application.Mappings;

public class TaskProfile : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<TaskAggregateState, TaskResponse>();
    config.NewConfig<CreateTaskCommand, TaskAggregateState>()
      .Map(dest => dest.At, _ => DateTime.UtcNow);
    config.NewConfig<ChangeTaskCommand, TaskAggregateState>();
    config.NewConfig<TaskProjection, TaskResponse>();
    config.NewConfig<TaskProjection, TaskResponseWithDue>()
      .Map(dest => dest.Due,
        src => src.DueAt.Date == DateTime.UtcNow.Date ? "today" : "upcoming");
  }
}
