//NetStandardIDistributedCacheProvider.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polly.Caching.Distributed
{
    /// <summary>
    /// Abstract base class for implementation of <see cref="Polly.Caching.ISyncCacheProvider{TResult}"/> and <see cref="Polly.Caching.IAsyncCacheProvider{TResult}"/> for Dot Net Core/Standard distributed caching implemented with <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>.  
    /// </summary>
    public abstract class NetStandardIDistributedCacheProvider<TCache> : ISyncCacheProvider<TCache>,
        IAsyncCacheProvider<TCache>
    {
        /// <summary>
        /// The <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/> implementation underlying this cache provider.
        /// </summary>
        protected internal readonly Microsoft.Extensions.Caching.Distributed.IDistributedCache _cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetStandardIDistributedCacheByteArrayProvider"/> class.
        /// </summary>
        /// <param name="iDistributedCache">An IDistributedCache implementation with which to store cached items.</param>
        protected NetStandardIDistributedCacheProvider(
            Microsoft.Extensions.Caching.Distributed.IDistributedCache iDistributedCache)
        {
            _cache = iDistributedCache ?? throw new ArgumentNullException(nameof(iDistributedCache));
        }

        /// <summary>
        /// Gets a value from cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>
        /// A tuple whose first element is a value indicating whether the key was found in the cache,
        /// and whose second element is the value from the cache (default(TResult) if not found).
        /// </returns>
        public abstract (bool, TCache) TryGet(string key);

        /// <summary>
        /// Gets a value from the memory cache as part of an asynchronous execution.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.  </param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="NetStandardIDistributedCacheProvider{TCache}"/>, this parameter is irrelevant and is ignored, as the Microsoft.Extensions.Caching.Distributed.IDistributedCache interface does not support it.</remarks></para></param>
        /// <returns>
        /// A <see cref="Task{TResult}" /> promising as Result a tuple whose first element is a value indicating whether
        /// the key was found in the cache, and whose second element is the value from the cache (default(TResult) if not found).
        /// </returns>
        public abstract Task<(bool, TCache)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext);

        /// <summary>
        /// Puts the specified value in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to put into the cache.</param>
        /// <param name="ttl">The time-to-live for the cache entry.</param>
        public abstract void Put(string key, TCache value, Ttl ttl);

        /// <summary>
        /// Puts the specified value in the cache as part of an asynchronous execution.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to put into the cache.</param>
        /// <param name="ttl">The time-to-live for the cache entry.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="NetStandardIDistributedCacheProvider{TCache}"/>, this parameter is irrelevant and is ignored, as the Microsoft.Extensions.Caching.Distributed.IDistributedCache interface does not support it.</remarks></para></param>
        /// <returns>A <see cref="Task" /> which completes when the value has been cached.</returns>
        public abstract Task PutAsync(string key, TCache value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext);
    }
}