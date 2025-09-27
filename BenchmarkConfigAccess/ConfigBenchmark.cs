using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

[MemoryDiagnoser]
[WarmupCount(5)]
[IterationCount(15)]
public class ConfigBenchmarks
{
    private IConfiguration _config = default!;
    private IOptions<CacheConfig> _options = default!;
    private bool _sink;

    [GlobalSetup]
    public void Setup()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // ?? key line
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var cfg = new CacheConfig();
        _config.Bind(cfg);
        _options = Options.Create(cfg);
    }

    [Benchmark(Baseline = true)]
    public void IOptions_Access()
    {
        bool v = _options.Value.CacheProviders.PersistedCache.Enable;
        _sink ^= v;
    }

    [Benchmark]
    public void IConfiguration_StringLookup_TryParse()
    {
        var str = _config["CacheProviders:PersistedCache:Enable"];
        bool parsed = bool.TryParse(str, out var v) && v;
        _sink ^= parsed;
    }

    private bool _cachedParsed;
    private bool _cachedInitialized;

    [Benchmark]
    public void IConfiguration_ParseOnce_CachedField()
    {
        if (!_cachedInitialized)
        {
            var str = _config["CacheProviders:PersistedCache:Enable"];
            _cachedParsed = bool.TryParse(str, out var v) && v;
            _cachedInitialized = true;
        }

        _sink ^= _cachedParsed;
    }
}
