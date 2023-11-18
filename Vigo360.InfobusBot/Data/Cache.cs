namespace Vigo360.InfobusBot.Data;

public class Cache<T> where T : class
{
    private readonly Dictionary<string, CacheItem<T>> _cache = new();
    private readonly TimeSpan _expiration;

    public Cache()
    {
        _expiration = TimeSpan.FromSeconds(60);
    }

    public Cache(TimeSpan expiration)
    {
        _expiration = expiration;
    }

    public void Insert(string key, T value)
    {
        CacheItem<T> item = new(key, value, DateTime.Now.Add(_expiration));

        _cache.Add(key, item);
    }

    public T? Get(string key)
    {
        if (!_cache.ContainsKey(key))
        {
            return null;
        }

        if (_cache[key].Expiration >= DateTime.Now) return _cache[key].Value;
        
        _cache.Remove(key);
        return null;

    }
    
    public void Remove(string key)
    {
        _cache.Remove(key);
    }
    
    public void Clear()
    {
        _cache.Clear();
    }
}

public record CacheItem<T>(string Key, T Value, DateTime Expiration);