using FastEndpoints;
using FluentValidation;
using Tasks.Api.Apis.Tasks.Messages;

namespace Tasks.Api.Apis.Tasks.Validation;

public class CreateTaskValidator : Validator<CreateTaskRequest>
{
  public CreateTaskValidator()
  {
    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("UserId is required");
    
    RuleFor(x => x.Subject)
      .NotEmpty()
      .WithMessage("Subject is required");

    RuleFor(x => x.Description)
      .NotEmpty()
      .WithMessage("Description is required")
      .MaximumLength(500)
      .WithMessage("Description must be less than 500 characters");

    RuleFor(x => x.DueAt)
      .NotEmpty()
      .WithMessage("DueAt is required")
      .GreaterThan(DateTime.UtcNow)
      .WithMessage("DueAt must be in the future");
  }
}
