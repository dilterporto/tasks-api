﻿using AutoMapper;
using Tasks.Application.Contracts;
using Tasks.Application.UseCases.ChangeTask;
using Tasks.Application.UseCases.CreateTask;
using Tasks.Domain.Aggregates.Tasks;

namespace Tasks.Application.Mappings;

public class TaskProfile : Profile
{
  public TaskProfile()
  {
    CreateMap<TaskAggregateState, TaskResponse>();
    CreateMap<CreateTaskCommand, TaskAggregateState>()
      .ForMember(x =>
        x.At, opt => opt.MapFrom(_ => DateTime.UtcNow));
    CreateMap<ChangeTaskCommand, TaskAggregateState>();
  }
}
