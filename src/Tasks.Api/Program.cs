using System.Reflection;
using FastEndpoints;
using FastEndpoints.Swagger;
using MediatR;
using Serilog;
using Tasks.Abstractions.Logging;
using Tasks.Application.Behaviors;
using Tasks.Application.UseCases.CreateTask;
using Tasks.DependencyInjection;
using Tasks.Persistence;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
  .WriteTo.Console()
  .WriteTo.Seq("http://localhost:5341")
  .CreateLogger();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDependencies(builder.Configuration);
builder.Services
  .AddFastEndpoints()
  .SwaggerDocument(o =>
  {
    o.DocumentSettings = s =>
    {
      s.Title = "Tasks API";
      s.Version = "v1";
    };
  });

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkPipelineBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CacheValidationPipelineBehavior<,>));
builder.Services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(CreateTaskCommand).Assembly);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(CreateTaskCommand).Assembly);

builder.Services.AddSerilog();

var app = builder.Build();

app
  .UseFastEndpoints()
  .UseSwaggerGen(o =>
  {

  });

app.Run();
