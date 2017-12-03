using System;

namespace Polly.Caching.IDistributedCache
{
    /// <summary>
    /// Provides helper methods for creating a Polly <see cref="ISyncCacheProvider"/> or <see cref="IAsyncCacheProvider"/>  implementation from a <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>.
    /// </summary>
    public static class CacheProviderHelper
    {
        /// <summary>
        /// Returns a Polly <see cref="ISyncCacheProvider{TCache}"/> wrapping a given <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>.  TCache much be either <see cref="string"/> or an array of <see cref="byte"/> />.
        /// </summary>
        /// <param name="iDistributedCache">The <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/> instance to use.</param>
        /// <returns>A corresponding Polly <see cref="ISyncCacheProvider{TCache}"/>.</returns>
        public static ISyncCacheProvider<TCache> AsSyncCacheProvider<TCache>(this Microsoft.Extensions.Caching.Distributed.IDistributedCache iDistributedCache)
        {
            if (typeof(TCache) == typeof(byte[]))
            {
                return (ISyncCacheProvider<TCache>) new NetStandardIDistributedCacheByteArrayProvider(iDistributedCache);
            }
            else if (typeof(TCache) == typeof(string))
            {
                return (ISyncCacheProvider<TCache>)new NetStandardIDistributedCacheStringProvider(iDistributedCache);
            }
            else
            {
                throw new ArgumentException($"{nameof(TCache)} must be either {typeof(string).Name} or {typeof(byte).Name}[]", nameof(TCache));
            }

        }

        /// <summary>
        /// Returns a Polly <see cref="IAsyncCacheProvider{TCache}"/> wrapping a given <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>, where TCache is byte[].
        /// </summary>
        /// <param name="iDistributedCache">The <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/> instance to use.</param>
        /// <returns>A corresponding Polly <see cref="IAsyncCacheProvider{TCache}"/>.</returns>
        public static IAsyncCacheProvider<TCache> AsAsyncCacheProvider<TCache>(this Microsoft.Extensions.Caching.Distributed.IDistributedCache iDistributedCache)
        {
            if (typeof(TCache) == typeof(byte[]))
            {
                return (IAsyncCacheProvider<TCache>)new NetStandardIDistributedCacheByteArrayProvider(iDistributedCache);
            }
            else if (typeof(TCache) == typeof(string))
            {
                return (IAsyncCacheProvider<TCache>)new NetStandardIDistributedCacheStringProvider(iDistributedCache);
            }
            else
            {
                throw new ArgumentException($"{nameof(TCache)} must be either {typeof(string).Name} or {typeof(byte).Name}[]", nameof(TCache));
            }

        }
    }
}