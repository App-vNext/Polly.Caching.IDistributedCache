#if !NETCOREAPP1_1

namespace Polly.Caching.Distributed.Specs.Integration
{
    public class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Async_ByteArray : CacheRoundTripSpecsAsyncBase<byte[]> {

        public CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Async_ByteArray() : base(new MemoryDistributedCachePolicyFactory())
        {
        }
    }
}

#endif