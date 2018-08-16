# Polly.Caching.Distributed

This repo contains the `Microsoft.Extensions.Caching.Distributed.IDistributedCache` provider for the [Polly](https://github.com/App-vNext/Polly) [Cache policy](https://github.com/App-vNext/Polly/wiki/Cache).  The current version targets .NET Standard 1.1 and .NET Standard 2.0.

[![NuGet version](https://badge.fury.io/nu/Polly.Caching.Distributed.svg)](https://badge.fury.io/nu/Polly.Caching.Distributed) [![Build status](https://ci.appveyor.com/api/projects/status/pgd89nfdr9u4ig8m?svg=true)](https://ci.appveyor.com/project/joelhulen/polly-caching-Distributed) [![Slack Status](http://www.pollytalk.org/badge.svg)](http://www.pollytalk.org)

## What is Polly?

[Polly](https://github.com/App-vNext/Polly) is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, Cache aside and Fallback in a fluent and thread-safe manner. Polly targets .NET Standard 1.1 and .NET Standard 2.0. 

Polly is a member of the [.NET Foundation](https://www.dotnetfoundation.org/about).

**Keep up to date with new feature announcements, tips & tricks, and other news through [www.thepollyproject.org](http://www.thepollyproject.org)**

![](https://raw.github.com/App-vNext/Polly/master/Polly-Logo.png)

## What is Polly.Caching.Distributed?

This project, Polly.Caching.Distributed, allows you to use Polly's `CachePolicy` with [implementations of](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed) .Net Standard's [`IDistributedCache`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache). 

# Installing Polly.Caching.Distributed via NuGet

    Install-Package Polly.Caching.Distributed

# Supported targets

Polly.Caching.Distributed supports .NET Standard 1.1 and .NET Standard 2.0.

## Dependencies

Polly.Caching.IDistributedCache &lt;v2.0 requires:

+ [Polly](nuget.org/packages/polly) v5.4.0 or above.
+ [Microsoft.Extensions.Caching.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Abstractions/) v1.1.2 or above.

Polly.Caching.Distributed &gt;=v2.0 requires:

+ [Polly](nuget.org/packages/polly) v6.0.1 or above.
+ [Microsoft.Extensions.Caching.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.Caching.Abstractions/) v2.0.2 or above.

# How to use the Polly.Caching.Distributed plugin

These notes assume you are familiar with using the .Net Standard `IDistributedCache` implementations.  For information, see: https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed .  As described on that page, Microsoft provide a Redis implementation and an SQL server implementation for `IDistributedCache`.

Assuming you have an instance `IDistributedCache distributedCache` in hand (perhaps just configured and instantiated, perhaps provided to local code by Dependency Injection):


```csharp
// Create a Polly cache policy for caching string results, using that IDistributedCache  instance.
var cachePolicy = Policy.Cache<string>(distributedCache.AsSyncCacheProvider<string>(), TimeSpan.FromMinutes(5));

// Create a Polly cache policy for caching byte[] results, using that IDistributedCache  instance.
var cachePolicy = Policy.Cache<byte[]>(distributedCache.AsSyncCacheProvider<byte[]>(), TimeSpan.FromMinutes(5));

// Or similarly for async executions returning string results:
var cachePolicy = Policy.CacheAsync<string>(distributedCache.AsAsyncCacheProvider<string>(), TimeSpan.FromMinutes(5));

// Or similarly for async executions returning  byte[] results:
var cachePolicy = Policy.CacheAsync<byte[]>(distributedCache.AsAsyncCacheProvider<byte[]>(), TimeSpan.FromMinutes(5));

// You can also use ASP.NET Core's DistributedCacheEntryOptions for specifying cache item time-to-live, as shown below. 
// All time-to-live functionality represented by DistributedCacheEntryOptions is supported.
DistributedCacheEntryOptions entryOptions = // ...
var cachePolicy = Policy.CacheAsync<byte[]>(distributedCache.AsAsyncCacheProvider<byte[]>(), entryOptions.AsTtlStrategy());
 

```

Configuration via DI in ASPNET Core:

```csharp
// In this example we choose to pass a whole PolicyRegistry by dependency injection rather than the individual policy, on the assumption the webapp will probably use multiple policies across the app.

// For example: 
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDistributedRedisCache(options =>
        {
            options.Configuration = "localhost"; // or whatever
            options.InstanceName = "SampleInstance";
        });

        services.AddSingleton<Polly.Caching.IAsyncCacheProvider<string>>(serviceProvider => serviceProvider.GetRequiredService<IDistributedCache>().AsAsyncCacheProvider<string>());

        services.AddSingleton<Polly.Registry.IPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
        {
            PolicyRegistry registry = new PolicyRegistry();
            registry.Add("myCachePolicy", Policy.CacheAsync<string>(serviceProvider.GetRequiredService<IAsyncCacheProvider<string>>(), TimeSpan.FromMinutes(5)));

            return registry;
            });

        // ...
    }
}

// In a controller, inject the policyRegistry and retrieve the policy:
// (magic string "myCachePolicy" hard-coded here only to keep the example simple) 
public MyController(IPolicyRegistry<string> policyRegistry)
{
    var _cachePolicy = policyRegistry.Get<IAsyncPolicy<string>>("myCachePolicy"); 
    // ...
}
```

Usage:

```csharp
string productId = // ... from somewhere
string productDescription = await cachePolicy.ExecuteAsync(context => getProductDescription(productId), 
    new Context(productId) // productId will also be the cache key used in this execution.
); 
```

For many more configuration options and usage examples of the main Polly `CachePolicy`, see the [main Polly readme](https://github.com/App-vNext/Polly#cache) and [deep doco on the Polly wiki](https://github.com/App-vNext/Polly/wiki/Cache).  Additional overloads allow attaching delegates for cache errors, cache hits/misses etc, for logging and telemetry.

`CachePolicy` can of course also be combined with other policies in a [`PolicyWrap`](https://github.com/App-vNext/Polly/wiki/PolicyWrap).

# Release notes

For details of changes by release see the [change log](CHANGELOG.md).  


# Acknowledgements

* [@seanfarrow](https://github.com/seanfarrow) and [@reisenberger](https://github.com/reisenberger) - Initial caching architecture in the main Polly repo
* [@reisenberger](https://github.com/reisenberger) - `IDistributedCache` implementation
* [@seanfarrow](https://github.com/seanfarrow) - v2.0 update to Signed packages only to correspond with Polly v6.0.1

# Instructions for Contributing

Please check out our [Wiki](https://github.com/App-vNext/Polly/wiki/Git-Workflow) for contributing guidelines. We are following the excellent GitHub Flow process, and would like to make sure you have all of the information needed to be a world-class contributor!

Since Polly is part of the .NET Foundation, we ask our contributors to abide by their [Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).

Also, we've stood up a [Slack](http://www.pollytalk.org) channel for easier real-time discussion of ideas and the general direction of Polly as a whole. Be sure to [join the conversation](http://www.pollytalk.org) today!

# License

Licensed under the terms of the [New BSD License](http://opensource.org/licenses/BSD-3-Clause)
