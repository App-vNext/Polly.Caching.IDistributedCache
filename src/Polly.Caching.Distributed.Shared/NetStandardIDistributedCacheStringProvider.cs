using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Polly.Caching.Distributed
{
    /// <summary>
    /// An implementation of <see cref="Polly.Caching.ISyncCacheProvider{TResult}"/> and <see cref="Polly.Caching.IAsyncCacheProvider{TResult}"/> for Dot Net Core/Standard distributed caching implemented with <see cref="Microsoft.Extensions.Caching.Distributed.IDistributedCache"/>.  TResult is string.
    /// </summary>
    public class NetStandardIDistributedCacheStringProvider : NetStandardIDistributedCacheProvider<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetStandardIDistributedCacheStringProvider"/> class.
        /// </summary>
        /// <param name="iDistributedCache">An IDistributedCache implementation with which to store cached items.</param>
        public NetStandardIDistributedCacheStringProvider(Microsoft.Extensions.Caching.Distributed.IDistributedCache iDistributedCache) : base(iDistributedCache)
        {
        }
        
        /// <summary>
        /// Gets a value from cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <returns>
        /// A tuple whose first element is a bool indicating whether the key was found in the cache,
        /// and whose second element is the value from the cache.
        /// </returns>
        public override (bool, string) TryGet(string key)
        {
            string fromCache = _cache.GetString(key);
            return (fromCache != null, fromCache);
        }

        /// <summary>
        /// Puts the specified value in the cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to put into the cache.</param>
        /// <param name="ttl">The time-to-live for the cache entry.</param>
        public override void Put(string key, string value, Ttl ttl)
        {
            _cache.SetString(key, value, ttl.ToDistributedCacheEntryOptions());
        }

        /// <summary>
        /// Gets a value from the memory cache as part of an asynchronous execution.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.  </param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="NetStandardIDistributedCacheProvider{TCache}"/>, this parameter is irrelevant and is ignored, as the Microsoft.Extensions.Caching.Distributed.IDistributedCache interface does not support it.</remarks></para></param>
        /// <returns>
        /// A <see cref="Task{TResult}" /> promising as Result a tuple whose first element is a value indicating whether
        /// the key was found in the cache, and whose second element is the value from the cache.
        /// </returns>
        public override async Task<(bool, string)> TryGetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string fromCache = await _cache.GetStringAsync(key
#if NETSTANDARD2_0
                , cancellationToken
#endif
                );
            return (fromCache != null, fromCache);
        }

        /// <summary>
        /// Puts the specified value in the cache as part of an asynchronous execution.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to put into the cache.</param>
        /// <param name="ttl">The time-to-live for the cache entry.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="NetStandardIDistributedCacheProvider{TCache}"/>, this parameter is irrelevant and is ignored, as the Microsoft.Extensions.Caching.Distributed.IDistributedCache interface does not support it.</remarks></para></param>
        /// <returns>A <see cref="Task" /> which completes when the value has been cached.</returns>
        public override Task PutAsync(string key, string value, Ttl ttl, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();

            return _cache.SetStringAsync(key, value, ttl.ToDistributedCacheEntryOptions()
#if NETSTANDARD2_0
                , cancellationToken
#endif
                );
        }

    }
}
