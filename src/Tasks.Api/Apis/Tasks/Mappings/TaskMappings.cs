using AutoMapper;
using Tasks.Api.Apis.Tasks.Messages;
using Tasks.Application.UseCases.ChangeTask;
using Tasks.Application.UseCases.CreateTask;

namespace Tasks.Api.Apis.Tasks.Mappings;

public class TaskMappings : Profile
{
  public TaskMappings()
  {
    CreateMap<CreateTaskRequest, CreateTaskCommand>();
    CreateMap<ChangeTaskRequest, ChangeTaskCommand>();
  }
}
