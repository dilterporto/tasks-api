using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Tasks.Abstractions.Caching;

public class CacheManager(IDatabase database) : ICacheManager
{
  public async Task Invalidate(string key) 
    => await database.KeyDeleteAsync(key);

  public bool ContainsKey(string key) 
    => !database.StringGet(key).IsNullOrEmpty;

  public async Task Set<T>(string key, T value, TimeSpan? expiration = null)
  {
    var serializedValue = JsonConvert.SerializeObject(value, new JsonSerializerSettings
    {
      ContractResolver = new CamelCasePropertyNamesContractResolver()
    });
    await database.StringSetAsync(key, serializedValue, expiration);
  }

  public async Task<Maybe<T>> Get<T>(string key)
  {
    var value = await database.StringGetAsync(key);
    return value.IsNullOrEmpty ? 
      Maybe<T>.None : 
      Maybe<T>.From(JsonConvert.DeserializeObject<T>(value!)!);
  }
}
