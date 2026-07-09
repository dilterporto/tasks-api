using CSharpFunctionalExtensions;

namespace Tasks.Abstractions.Caching;

public class NullCacheManager : ICacheManager
{
    public Task Invalidate(string key) => Task.CompletedTask;
    public bool ContainsKey(string key) => false;
    public Task Set<T>(string key, T value, TimeSpan? expiration = null) => Task.CompletedTask;
    public Task<Maybe<T>> Get<T>(string key) => Task.FromResult(Maybe<T>.None);
}
