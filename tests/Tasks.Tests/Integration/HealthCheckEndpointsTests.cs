using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tasks.Tests.Integration;

public class HealthCheckEndpointsTests
{
  [Fact]
  public async Task Liveness_Returns200()
  {
    await using var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();

    var response = await client.GetAsync("/health/live");

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task Readiness_Returns200_WhenAllHealthy()
  {
    await using var factory = new WebApplicationFactory<Program>();
    var client = factory.CreateClient();

    var response = await client.GetAsync("/health/ready");
    var body = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Contains("\"status\"", body);
  }

  [Fact]
  public async Task Readiness_Returns503_WhenPostgresDown()
  {
    await using var factory = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(builder =>
      {
        builder.UseSetting(
          "ConnectionStrings:DefaultConnection",
          "Host=127.0.0.2;Database=localdefaultdb;Username=appuser;Password=password123;Timeout=2");
      });
    var client = factory.CreateClient();

    var response = await client.GetAsync("/health/ready");
    var body = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    Assert.Contains("\"status\"", body);
  }

  [Fact]
  public async Task Readiness_Returns503_WhenRedisDown()
  {
    await using var factory = new WebApplicationFactory<Program>()
      .WithWebHostBuilder(builder =>
      {
        builder.UseSetting("Redis:Server", "127.0.0.2:6379,connectTimeout=500,abortConnect=false");
      });
    var client = factory.CreateClient();

    var response = await client.GetAsync("/health/ready");
    var body = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    Assert.Contains("\"status\"", body);
  }
}
