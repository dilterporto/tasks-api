namespace Tasks.Api.Apis.Tasks.Messages;

public class ChangeTaskRequest
{
  public Guid UserId { get; set; }
  public required string Subject { get; set; }
  public required string Description { get; set; }
}
