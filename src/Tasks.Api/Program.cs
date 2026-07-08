using System.Reflection;
using System.Text.Json;
using FastEndpoints;
using FastEndpoints.Swagger;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using StackExchange.Redis;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.Logging;
using Tasks.Application.Behaviors;
using Tasks.Application.UseCases.CreateTask;
using Tasks.DependencyInjection;
using Mapster;
using MapsterMapper;
using Tasks.Persistence;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;



Log.Logger = new LoggerConfiguration()
  .WriteTo.Console()
  .WriteTo.Seq(configuration.GetSection("Seq:ServerUrl").Value!)
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

var mapsterConfig = TypeAdapterConfig.GlobalSettings;
mapsterConfig.Scan(Assembly.GetExecutingAssembly(), typeof(CreateTaskCommand).Assembly);
builder.Services.AddSingleton(mapsterConfig);
builder.Services.AddScoped<MapsterMapper.IMapper, ServiceMapper>();

builder.Services.AddSerilog();

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configuration.GetSection("Redis:Server").Value!));
builder.Services.AddScoped<IDatabase>(o =>
{
  var muxer = o.GetRequiredService<IConnectionMultiplexer>();
  return muxer.GetDatabase();
});

builder.Services.AddScoped<ICacheManager, CacheManager>();

var app = builder.Build();

app
  .UseFastEndpoints()
  .UseSwaggerGen(o =>
  {

  });

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
  Predicate = _ => false
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
  Predicate = r => r.Tags.Contains("readiness"),
  ResponseWriter = async (context, report) =>
  {
    context.Response.ContentType = "application/json; charset=utf-8";
    var result = JsonSerializer.Serialize(new
    {
      status = report.Status.ToString(),
      results = report.Entries.ToDictionary(
        e => e.Key,
        e => new { status = e.Value.Status.ToString() })
    });
    await context.Response.WriteAsync(result);
  }
});

app.Run();
