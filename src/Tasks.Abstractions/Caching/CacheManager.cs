using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Tasks.Abstractions.Caching;

public class CacheManager(IDatabase database, ILogger<CacheManager> logger) : ICacheManager
{
  public async Task<bool> ContainsKey(string key)
  {
    try
    {
      return await database.KeyExistsAsync(key);
    }
    catch (RedisException ex)
    {
      logger.LogWarning(ex, "[Cache] Redis unavailable on ContainsKey for key {Key}, treating as cache miss.", key);
      return false;
    }
  }

  public async Task Invalidate(string key)
  {
    try
    {
      await database.KeyDeleteAsync(key);
    }
    catch (RedisException ex)
    {
      logger.LogWarning(ex, "[Cache] Redis unavailable on Invalidate for key {Key}.", key);
    }
  }

  public async Task Set<T>(string key, T value, TimeSpan? expiration = null)
  {
    try
    {
      var serializedValue = JsonConvert.SerializeObject(value, new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      });
      await database.StringSetAsync(key, serializedValue, expiration);
    }
    catch (RedisException ex)
    {
      logger.LogWarning(ex, "[Cache] Redis unavailable on Set for key {Key}, cache write skipped.", key);
    }
  }

  public async Task<Maybe<T>> Get<T>(string key)
  {
    try
    {
      var value = await database.StringGetAsync(key);
      return value.IsNullOrEmpty ?
        Maybe<T>.None :
        Maybe<T>.From(JsonConvert.DeserializeObject<T>(value!)!);
    }
    catch (RedisException ex)
    {
      logger.LogWarning(ex, "[Cache] Redis unavailable on Get for key {Key}, returning None.", key);
      return Maybe<T>.None;
    }
  }
}
