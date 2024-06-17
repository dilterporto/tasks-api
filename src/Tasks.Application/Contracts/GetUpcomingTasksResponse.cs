namespace Tasks.Application.Contracts;

public record UpcomingTasksResponse(List<GroupedTaskResponse> Tasks);

public record TaskResponseWithDue(
  string Due,
  Guid Id,
  string? Status,
  DateTime DueAt,
  DateTime At,
  Guid UserId,
  string Description,
  string Subject,
  DateTime? StartedAt,
  DateTime? CompletedAt) : TaskResponse(Id, Status, DueAt, At, UserId, Description, Subject, StartedAt, CompletedAt)
{
  public TaskResponseWithDue() : this(
    default!, 
    default, 
    default, 
    default, 
    default, 
    default, 
    default!, 
    default!, 
    default, 
    default)
  {
    
  }
}

public record GroupedTaskResponse(string? Group, List<DateGroupedTaskResponse> Tasks);

public record DateGroupedTaskResponse(DateTime Date, List<TaskResponse> Tasks);
