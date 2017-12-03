using Microsoft.Extensions.Caching.Distributed;
using System;

namespace Polly.Caching.IDistributedCache
{
    /// <summary>
    /// Provides helper methods for creating a <see cref="Polly.Caching.ITtlStrategy"/> implementation from a <see cref="Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions"/>.
    /// </summary>
    public static class TtlStrategyHelper
    {
        /// <summary>
        /// Returns an equivalent Polly <see cref="ITtlStrategy"/> implementation for a given <see cref="DistributedCacheEntryOptions"/>.
        /// </summary>
        /// <param name="entryOptions">The <see cref="DistributedCacheEntryOptions"/> instance to convert.</param>
        /// <returns>A corresponding <see cref="Ttl"/> instance.</returns>
        public static ITtlStrategy AsTtlStrategy(this DistributedCacheEntryOptions entryOptions)
        {
            if (entryOptions.AbsoluteExpiration != null)
            {
                return new AbsoluteTtl(entryOptions.AbsoluteExpiration.Value);
            }
            else if (entryOptions.AbsoluteExpirationRelativeToNow != null)
            {
                return new RelativeTtl(entryOptions.AbsoluteExpirationRelativeToNow.Value);
            }
            else if (entryOptions.SlidingExpiration != null)
            {
                return new SlidingTtl(entryOptions.SlidingExpiration.Value);
            }
            else
            {
                throw new ArgumentException($"The passed {typeof(DistributedCacheEntryOptions)} could not be understood.", nameof(entryOptions));
            }
        }
    }
}
