namespace Tasks.Application.Contracts;

public record TaskResponse(
  Guid Id,
  string? Status,
  DateTime DueAt,
  DateTime At,
  Guid UserId,
  string Description,
  string Subject,
  DateTime? StartedAt,
  DateTime? CompletedAt);
  

