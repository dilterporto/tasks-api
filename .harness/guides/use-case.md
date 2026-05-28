# Guide: Adding a Use Case

This guide covers adding a command (write) or query (read) use case following the established CQRS pattern.

## When to use this guide

- Adding a new endpoint that mutates state → **Command**
- Adding a new endpoint that reads state → **Query**

---

## Command use case

### 1. Define the command

`src/Tasks.Application/UseCases/<Name>/<Name>Command.cs`

```csharp
public record <Name>Command(...) : ICommand<Result<TResponse>>;
```

`ICommand<T>` is defined in `Tasks.Abstractions.CQRS` and extends `IRequest<T>`.

### 2. Implement the handler

`src/Tasks.Application/UseCases/<Name>/<Name>CommandHandler.cs`

```csharp
public class <Name>CommandHandler(ITaskRepository taskRepository, IMapper mapper)
  : ICommandHandler<<Name>Command, Result<TResponse>>
{
  public Task<Result<TResponse>> Handle(<Name>Command command, CancellationToken cancellationToken) =>
    taskRepository.LoadByIdAsync(command.TaskId)
      .ToResult($"Task {command.TaskId} not found.")
      .Tap(task => task.<Method>(...))
      .Check(taskRepository.SaveAsync)
      .Map(task => mapper.Map<TResponse>(task.State));
}
```

Rules:
- Return `Result<T>` — no exceptions for flow control
- Load the aggregate via `ITaskRepository.LoadByIdAsync`
- Mutate via an aggregate method (never mutate `State` directly)
- Persist via `ITaskRepository.SaveAsync`
- Map to response via `IMapper`

### 3. Register Mapster mapping

`src/Tasks.Application/Mappings/TaskProfile.cs`

```csharp
config.NewConfig<InputType, OutputType>();
```

### 4. Add the endpoint

`src/Tasks.Api/Apis/Tasks/<Name>Endpoint.cs` — use `FastEndpoints` pattern:

```csharp
public class <Name>Endpoint(ISender sender, IMapper mapper)
  : Endpoint<<Name>Request, <Name>Response>
{
  public override void Configure() => ...;
  public override async Task HandleAsync(<Name>Request req, CancellationToken ct) =>
    await sender.Send(mapper.Map<<Name>Command>(req), ct)
      .Match(response => SendOkAsync(response, ct), err => SendAsync(new { error = err }, 400, ct));
}
```

### 5. Update CLAUDE.md

Add the new use case to the use case table in `CLAUDE.md`.

---

## Query use case

### 1. Define the query

`src/Tasks.Application/UseCases/<Name>/<Name>Query.cs`

```csharp
public record <Name>Query(...) : IQuery<Result<TResponse>>;
```

### 2. Implement the handler

```csharp
public class <Name>QueryHandler(IProjectionsReader<TaskProjection> reader, IMapper mapper)
  : IQueryHandler<<Name>Query, Result<TResponse>>
{
  public async Task<Result<TResponse>> Handle(<Name>Query query, CancellationToken ct)
  {
    var projection = await reader.GetByIdAsync(query.Id);
    return projection is null
      ? Result.Failure<TResponse>("Not found.")
      : Result.Success(mapper.Map<TResponse>(projection));
  }
}
```

Rules:
- Read from `IProjectionsReader<TaskProjection>` — never from `ITaskRepository`
- For cached queries, check `ICacheManager` before the DB (see `GetUpcomingTasksQueryHandler` for pattern)

### 3. Add endpoint and update CLAUDE.md

Same as steps 4 and 5 for commands above.

---

## Pipeline behaviors (automatic)

All use cases go through these behaviors in order:

1. `UnitOfWorkPipelineBehavior` — opens EF Core transaction (commands only)
2. `LoggingBehavior` — logs command/query name and result
3. `CacheValidationPipelineBehavior` — invalidates Redis after successful command

No registration needed — behaviors are registered globally in `DependenciesConfig.cs`.
