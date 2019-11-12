#if !NETCOREAPP1_1

namespace Polly.Caching.Distributed.Specs.Integration
{
    public class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Sync_String : CacheRoundTripSpecsSyncBase<string> {
        public CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Sync_String() : base(new MemoryDistributedCachePolicyFactory())
        {
        }
    }
}

#endif