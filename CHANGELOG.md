# Polly.Caching.Distributed change log

## 3.0.1
- No functional changes
- Updated Polly dependency to latest, v7.1.1
- Consolidated solution and fixed build
- Add SourceLink support
- Added NetStandard 2.1 target (for .NET Core3.0 consumption)
- Added test runs in netcoreapp3.0; .NET Framework 4.6.1; and .NET Framework 4.7.2
- Updated FluentAssertions and xUnit dependencies

## 3.0.0
- Allow caching of `default(TResult)`
- Compatible with Polly &gt;= v7

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
