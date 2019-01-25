# Polly.Caching.Distributed change log

## 2.0.1
- No functional changes
- Indicate compatibility with Polly &lt; v7

## 2.0.0
- Provide a single signed package only.
- Update Polly to V6.0.1.
- Support .net Standard 2.0.
- Rename package, and change namespaces, from Polly.Caching.IDistributedCache to Polly.Caching.Distributed, to avoid clashes.

## 1.0-RC

- Allows [Polly](https://github.com/App-vNext/Polly)'s [CachePolicy](https://github.com/App-vNext/Polly/wiki/Cache) to be used with [`Microsoft.Extensions.Caching.Distributed.IDistributedCache`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache) [implementations](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed).
- Release candidate
