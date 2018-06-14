using System;
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
        /// <returns>The value from cache; or null, if none was found.</returns>
        public override string Get(String key)
        {
            string returned = _cache.GetString(key);
            return returned == null || returned.Length == 0 ? null : returned;
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
        /// Gets a value from the memory cache as part of an asynchronous execution.  <para><remarks>The implementation is synchronous as there is no advantage to an asynchronous implementation for an in-memory cache.</remarks></para>
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="cancellationToken">The cancellation token.  </param>
        /// <param name="continueOnCapturedContext">Whether async calls should continue on a captured synchronization context. <para><remarks>For <see cref="NetStandardIDistributedCacheProvider{TCache}"/>, this parameter is irrelevant and is ignored, as the Microsoft.Extensions.Caching.Distributed.IDistributedCache interface does not support it.</remarks></para></param>
        /// <returns>A <see cref="Task{TResult}" /> promising as Result the value from cache; or null, if none was found.</returns>
        public override async Task<string> GetAsync(string key, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string returned = await _cache.GetStringAsync(key);
            return returned == null || returned.Length == 0 ? null : returned; 
        }

        /// <summary>
        /// Puts the specified value in the cache as part of an asynchronous execution.
        /// <para><remarks>The implementation is synchronous as there is no advantage to an asynchronous implementation for an in-memory cache.</remarks></para>
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

            return _cache.SetStringAsync(key, value, ttl.ToDistributedCacheEntryOptions());
        }

    }
}
