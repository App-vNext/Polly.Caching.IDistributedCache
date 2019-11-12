using Microsoft.Extensions.Caching.Distributed;

namespace Polly.Caching.Distributed
{
    internal static class InternalTtlHelper
    {
        internal static DistributedCacheEntryOptions ToDistributedCacheEntryOptions(this Ttl ttl)
        {
            return ttl.SlidingExpiration
                ? new DistributedCacheEntryOptions { SlidingExpiration = ttl.Timespan }
                : new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ttl.Timespan };
        }
    }
}
