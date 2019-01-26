#if !NETCOREAPP1_1

using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly.Caching.Serialization.Json;

namespace Polly.Caching.Distributed.Specs.Integration
{
    public class CachePolicyFactory
    {
        public static (ISyncCacheProvider<TResult>, ISyncPolicy<TResult>) CreateSyncCachePolicy<TCache, TResult>()
        {
            var memoryIDistributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            ISyncCacheProvider<TResult> memoryIDistributedCacheProvider;
            if (typeof(TCache) == typeof(string))
            {
                memoryIDistributedCacheProvider = memoryIDistributedCache.AsSyncCacheProvider<string>().WithSerializer<TResult, string>(new JsonSerializer<TResult>(new JsonSerializerSettings()));
            }
            else if (typeof(TCache) == typeof(byte[]))
            {
                memoryIDistributedCacheProvider = memoryIDistributedCache.AsSyncCacheProvider<byte[]>().WithSerializer(new ByteArraySerializer()).WithSerializer<TResult, string>(new JsonSerializer<TResult>(new JsonSerializerSettings()));
            }
            else
            {
                throw new ArgumentException($"{nameof(TCache)} must be either {typeof(string).Name} or {typeof(byte).Name}[]", nameof(TCache));
            }

            var policy = Policy.Cache<TResult>(memoryIDistributedCacheProvider, TimeSpan.FromHours(1));
            return (memoryIDistributedCacheProvider, policy);
        }

        public static (IAsyncCacheProvider<TResult>, IAsyncPolicy<TResult>) CreateAsyncCachePolicy<TCache, TResult>()
        {
            var memoryIDistributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            IAsyncCacheProvider<TResult> memoryIDistributedCacheProvider;
            if (typeof(TCache) == typeof(string))
            {
                memoryIDistributedCacheProvider = memoryIDistributedCache.AsAsyncCacheProvider<string>().WithSerializer<TResult, string>(new JsonSerializer<TResult>(new JsonSerializerSettings()));
            }
            else if (typeof(TCache) == typeof(byte[]))
            {
                memoryIDistributedCacheProvider = memoryIDistributedCache.AsAsyncCacheProvider<byte[]>().WithSerializer(new ByteArraySerializer()).WithSerializer<TResult, string>(new JsonSerializer<TResult>(new JsonSerializerSettings()));
            }
            else
            {
                throw new ArgumentException($"{nameof(TCache)} must be either {typeof(string).Name} or {typeof(byte).Name}[]", nameof(TCache));
            }

            var policy = Policy.CacheAsync<TResult>(memoryIDistributedCacheProvider, TimeSpan.FromHours(1));
            return (memoryIDistributedCacheProvider, policy);
        }
    }
}

#endif