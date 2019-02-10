#if !NETCOREAPP1_1

namespace Polly.Caching.Distributed.Specs.Integration
{
    public class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Sync_ByteArray : CacheRoundTripSpecsSyncBase<byte[]> {
        public CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Sync_ByteArray() : base(new MemoryDistributedCachePolicyFactory())
        {
        }
    }
}

#endif