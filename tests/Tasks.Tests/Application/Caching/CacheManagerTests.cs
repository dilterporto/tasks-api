using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;
using Tasks.Abstractions.Caching;
using Xunit;

namespace Tasks.Tests.Application.Caching;

public class CacheManagerTests
{
  private readonly Mock<IDatabase> _database = new();
  private readonly Mock<ILogger<CacheManager>> _logger = new();

  private CacheManager BuildManager() => new(_database.Object, _logger.Object);

  // AC-5 sensor: ContainsKey returns false (not throws) when Redis throws
  [Fact]
  public async Task ContainsKey_WhenRedisThrows_ReturnsFalse()
  {
    _database.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
             .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis unavailable"));

    var result = await BuildManager().ContainsKey("any-key");

    Assert.False(result);
  }

  // AC-5 sensor: ContainsKey uses async KeyExistsAsync (not synchronous StringGet)
  [Fact]
  public async Task ContainsKey_WhenKeyExists_UsesKeyExistsAsync()
  {
    _database.Setup(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
             .ReturnsAsync(true);

    var result = await BuildManager().ContainsKey("existing-key");

    Assert.True(result);
    _database.Verify(x => x.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Once);
    _database.Verify(x => x.StringGet(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()), Times.Never);
  }

  // AC-2 sensor: Get returns Maybe.None (not throws) when Redis throws
  [Fact]
  public async Task Get_WhenRedisThrows_ReturnsMaybeNone()
  {
    _database.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
             .Returns(Task.FromException<RedisValue>(
               new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis unavailable")));

    var result = await BuildManager().Get<string>("any-key");

    Assert.True(result.HasNoValue);
  }

  // AC-3 sensor: Set does not throw when Redis throws
  [Fact]
  public async Task Set_WhenRedisThrows_DoesNotThrow()
  {
    _database.Setup(x => x.StringSetAsync(
        It.IsAny<RedisKey>(), It.IsAny<RedisValue>(),
        It.IsAny<TimeSpan?>(), It.IsAny<bool>(),
        It.IsAny<When>(), It.IsAny<CommandFlags>()))
      .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis unavailable"));

    var exception = await Record.ExceptionAsync(() => BuildManager().Set("any-key", "value"));

    Assert.Null(exception);
  }

  [Fact]
  public async Task Invalidate_WhenRedisThrows_DoesNotThrow()
  {
    _database.Setup(x => x.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
             .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Redis unavailable"));

    var exception = await Record.ExceptionAsync(() => BuildManager().Invalidate("any-key"));

    Assert.Null(exception);
  }
}
