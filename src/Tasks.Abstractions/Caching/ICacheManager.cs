using CSharpFunctionalExtensions;

namespace Tasks.Abstractions.Caching;

public interface ICacheManager
{
  Task Invalidate(string key);
  bool ContainsKey(string key);
  Task Set<T>(string key, T value, TimeSpan? expiration = null);
  Task<Maybe<T>> Get<T>(string key);
}
