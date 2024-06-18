using System.Reflection;
using FastEndpoints;
using FastEndpoints.Swagger;
using MediatR;
using Serilog;
using StackExchange.Redis;
using Tasks.Abstractions.Caching;
using Tasks.Abstractions.Logging;
using Tasks.Application.Behaviors;
using Tasks.Application.UseCases.CreateTask;
using Tasks.DependencyInjection;
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
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly(), typeof(CreateTaskCommand).Assembly);

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

app.Run();
