using CSharpFunctionalExtensions;
using MediatR;
using Tasks.Application.Contracts;

namespace Tasks.Application.UseCases.GetUpcomingTasks;

public record GetUpcomingTasksQuery : IRequest<Result<UpcomingTasksResponse>>;
