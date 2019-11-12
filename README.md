# Polly.Caching.Distributed

This repo contains the `Microsoft.Extensions.Caching.Distributed.IDistributedCache` provider for the [Polly](https://github.com/App-vNext/Polly) [Cache policy](https://github.com/App-vNext/Polly/wiki/Cache).  The current version targets .NET Standard 1.1, .NET Standard 2.0 and .NET Standard 2.1.

[![NuGet version](https://badge.fury.io/nu/Polly.Caching.Distributed.svg)](https://badge.fury.io/nu/Polly.Caching.Distributed) [![Build status](https://ci.appveyor.com/api/projects/status/pgd89nfdr9u4ig8m?svg=true)](https://ci.appveyor.com/project/joelhulen/polly-caching-Distributed) [![Slack Status](http://www.pollytalk.org/badge.svg)](http://www.pollytalk.org)

## What is Polly?

[Polly](https://github.com/App-vNext/Polly) is a .NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker, Timeout, Bulkhead Isolation, Cache aside and Fallback in a fluent and thread-safe manner. 

Polly is a member of the [.NET Foundation](https://www.dotnetfoundation.org/about).

**Keep up to date with new feature announcements, tips & tricks, and other news through [www.thepollyproject.org](http://www.thepollyproject.org)**

![](https://raw.github.com/App-vNext/Polly/master/Polly-Logo.png)

## What is Polly.Caching.Distributed?

This project, Polly.Caching.Distributed, allows you to use Polly's `CachePolicy` with [implementations of](https://docs.microsoft.com/en-us/aspnet/core/performance/caching/distributed) .Net Standard's [`IDistributedCache`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache). 

# Installing Polly.Caching.Distributed via NuGet

    Install-Package Polly.Caching.Distributed

# Supported targets

Polly.Caching.Distributed &gt;= v3.0.1 supports .NET Standard 1.1, .NET Standard 2.0 and .NET Standard 2.1.

Polly.Caching.Distributed &lt; v3.0.1 supports .NET Standard 1.1 and .NET Standard 2.0.

## Dependency compatibility with Polly

Polly.Caching.Distributed &gt;=v3.0.1 requires:

+ [Polly](https://nuget.org/packages/polly) >= v7.1.1.

Polly.Caching.Distributed v3.0.0 requires:

+ [Polly](https://nuget.org/packages/polly) >= v7.0.0.

Polly.Caching.Distributed &gt;=v2.0 and &lt;v3 requires:

+ [Polly](https://nuget.org/packages/polly) >= v6.0.1 and &lt;v7.

Polly.Caching.IDistributedCache &lt;v2.0 requires:

+ [Polly](https://nuget.org/packages/polly) v5.4.0 or above.


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

## Configuration via DI in ASPNET Core:

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

        services.AddSingleton<Polly.Registry.IReadOnlyPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
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
public MyController(IReadOnlyPolicyRegistry<string> policyRegistry)
{
    var _cachePolicy = policyRegistry.Get<IAsyncPolicy<string>>("myCachePolicy"); 
    // ...
}
```

## Automatically serializing more complex type

The raw cache provider `Polly.Caching.IDistributedCache` allows you to cache items of type `byte[]` or `string` as those are the native formats supported by [`Microsoft.Extensions.Caching.Distributed.IDistributedCache`](https://docs.microsoft.com/en-gb/dotnet/api/microsoft.extensions.caching.distributed.idistributedcache).  However, Polly also allows you to automatically serialize more complex types.

The package `Polly.Caching.Serialization.Json` ([github](https://github.com/App-vNext/Polly.Caching.Serialization.Json); [nuget](https://www.nuget.org/packages/Polly.Caching.Serialization.Json)) is a Polly [`ICacheItemSerializer<TResult, string>`](https://github.com/App-vNext/Polly/wiki/Implementing-cache-serializers#using-a-serializer-with-the-polly-cachepolicy) to serialize any type for use with `Polly.Caching.IDistributedCache`.  

Configuration in .NET Core:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDistributedRedisCache(options =>
        {
            options.Configuration = "localhost"; // or whatever
            options.InstanceName = "SampleInstance";
        });

        // Obtain a Newtonsoft.Json.JsonSerializerSettings defining any settings to use for serialization
        // (could alternatively be obtained from a factory by DI)
        var serializerSettings = new JsonSerializerSettings()
        {
            // Any configuration options
        };

        // Register a Polly cache provider for caching ProductDetails entities, using the IDistributedCache instance and a Polly.Caching.Serialization.Json.JsonSerializer.
        // (ICacheItemSerializer<ProductDetails, string> could alternatively be obtained from a factory by DI)
        services.AddSingleton<Polly.Caching.IAsyncCacheProvider<ProductDetails>>(serviceProvider =>
            serviceProvider
                .GetRequiredService<IDistributedCache>()
                .AsAsyncCacheProvider<string>()
                .WithSerializer<ProductDetails, string>(
                    new Polly.Caching.Serialization.Json.JsonSerializer<ProductDetails>(serializerSettings)
                );

        // Register a Polly cache policy for caching ProductDetails entities, using that IDistributedCache instance.
        services.AddSingleton<Polly.Registry.IReadOnlyPolicyRegistry<string>, Polly.Registry.PolicyRegistry>((serviceProvider) =>
        {
            PolicyRegistry registry = new PolicyRegistry();
            registry.Add("productsCachePolicy", Policy.CacheAsync<ProductDetails>(serviceProvider.GetRequiredService<IAsyncCacheProvider<ProductDetails>>(), TimeSpan.FromMinutes(5)));

            return registry;
        });

        // ...
    }
}

// In a controller, inject the policyRegistry and retrieve the policy:
// (magic string "productsCachePolicy" hard-coded here only to keep the example simple) 
public MyController(IReadOnlyPolicyRegistry<string> policyRegistry)
{
    var _cachePolicy = policyRegistry.Get<IAsyncPolicy<ProductDetails>>("productsCachePolicy"); 
    // ...
}
```

## Usage at the point of consumption

```csharp
string productId = // ... from somewhere
string productDescription = await _cachePolicy.ExecuteAsync(context => getProductDescription(productId), 
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
* [@reisenberger](https://github.com/reisenberger) - Update to Polly v7.0.0

# Instructions for Contributing

Please check out our [Wiki](https://github.com/App-vNext/Polly/wiki/Git-Workflow) for contributing guidelines. We are following the excellent GitHub Flow process, and would like to make sure you have all of the information needed to be a world-class contributor!

Since Polly is part of the .NET Foundation, we ask our contributors to abide by their [Code of Conduct](https://www.dotnetfoundation.org/code-of-conduct).

Also, we've stood up a [Slack](http://www.pollytalk.org) channel for easier real-time discussion of ideas and the general direction of Polly as a whole. Be sure to [join the conversation](http://www.pollytalk.org) today!

# License

Licensed under the terms of the [New BSD License](http://opensource.org/licenses/BSD-3-Clause)
