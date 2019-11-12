#if !NETCOREAPP1_1

namespace Polly.Caching.Distributed.Specs.Integration
{
    public class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Async_String : CacheRoundTripSpecsAsyncBase<string> {
        public CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_Async_String() : base(new MemoryDistributedCachePolicyFactory())
        {
        }
    }
}

#endif