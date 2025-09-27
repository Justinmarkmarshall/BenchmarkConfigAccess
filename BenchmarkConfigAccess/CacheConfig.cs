public sealed class CacheConfig
{
    public CacheProvidersConfig CacheProviders { get; set; } = new();
}

public sealed class CacheProvidersConfig
{
    public PersistedCacheConfig PersistedCache { get; set; } = new();
}

public sealed class PersistedCacheConfig
{
    public bool Enable { get; set; }
}