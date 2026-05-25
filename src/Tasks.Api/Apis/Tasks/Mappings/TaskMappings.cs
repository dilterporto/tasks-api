using Mapster;
using Tasks.Api.Apis.Tasks.Messages;
using Tasks.Application.UseCases.ChangeTask;
using Tasks.Application.UseCases.CreateTask;

namespace Tasks.Api.Apis.Tasks.Mappings;

public class TaskMappings : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<CreateTaskRequest, CreateTaskCommand>();
    config.NewConfig<ChangeTaskRequest, ChangeTaskCommand>();
  }
}
