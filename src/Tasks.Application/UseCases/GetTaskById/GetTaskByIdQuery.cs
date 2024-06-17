using CSharpFunctionalExtensions;
using MediatR;
using Tasks.Application.Contracts;

namespace Tasks.Application.UseCases.GetTaskById;

public record GetTaskByIdQuery(Guid TaskId) : IRequest<Result<TaskResponse>>;
