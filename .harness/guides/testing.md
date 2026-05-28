# Guide: Testing

## Test project

```
tests/Tasks.Tests/
├── Application/UseCases/   ← handler unit tests
├── Domain/                 ← aggregate unit tests
└── ...
```

Run all tests:

```bash
dotnet test tests/Tasks.Tests/
```

Run a single test class:

```bash
dotnet test tests/Tasks.Tests/ --filter "FullyQualifiedName~<ClassName>"
```

---

## What to test

### Domain layer — aggregate behavior

Test that aggregate methods emit the correct events and mutate state correctly.

```csharp
[Fact]
public void Start_WhenTaskIsCreated_SetsStatusToStarted()
{
  var state = new TaskAggregateState { ... };
  var aggregate = new TaskAggregate(state);

  aggregate.Start(userId);

  Assert.Equal(TaskStatus.Started, aggregate.State.Status);
  Assert.Contains(aggregate.UncommittedChanges, e => e is TaskStartedEvent);
}
```

### Application layer — handler behavior

Test handlers with mocked dependencies. Do **not** mock the aggregate itself.

```csharp
[Fact]
public async Task Handle_WhenValid_ReturnsTaskResponse()
{
  var taskRepo = new Mock<ITaskRepository>();
  var mapper = new Mock<IMapper>();
  // ... arrange mocks ...

  var handler = new CreateTaskCommandHandler(taskRepo.Object, mapper.Object);
  var result = await handler.Handle(command, CancellationToken.None);

  Assert.True(result.IsSuccess);
}
```

Rules:
- Mock `ITaskRepository`, `IProjectionsReader<T>`, `ICacheManager`, `IMapper`, `ILogger<T>`
- Do not mock `IMediator` — test handlers directly
- Do not mock the aggregate — instantiate it with test state
- Do not use in-memory EF Core — handlers should not reference `DbContext` directly

---

## Test naming

```
<ClassName>_<Scenario>_<ExpectedOutcome>
```

Examples:
- `Handle_WhenNoUpcomingTasks_ReturnsEmptyResponse`
- `Start_WhenTaskAlreadyStarted_ReturnsFailure`

---

## Fixtures

Use `AutoFixture` for value objects and state:

```csharp
var fixture = new Fixture();
var state = fixture.Create<TaskAggregateState>();
```

Use `Moq` for interfaces:

```csharp
var repo = new Mock<ITaskRepository>();
repo.Setup(x => x.LoadByIdAsync(id)).ReturnsAsync(Maybe<TaskAggregate>.From(aggregate));
```

---

## What NOT to test

- `IMapper` mapping rules — these are verified by Mapster's own validation at startup
- EF Core queries — use integration tests (not currently in scope)
- MediatR pipeline behavior ordering — that is a framework concern
