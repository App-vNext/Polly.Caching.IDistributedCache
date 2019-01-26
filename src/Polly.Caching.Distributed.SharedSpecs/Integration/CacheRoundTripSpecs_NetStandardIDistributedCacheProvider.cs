#if !NETCOREAPP1_1

using System;
using FluentAssertions;
using Polly.Caching.Distributed.Specs.Integration;
using Xunit;

namespace Polly.Specs.Caching.Distributed.Integration
{
    public class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_String : CacheRoundTripSpecs_NetStandardIDistributedCacheProvider<string> { }

    public class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider_ByteArray : CacheRoundTripSpecs_NetStandardIDistributedCacheProvider<byte[]> { }

    public abstract class CacheRoundTripSpecs_NetStandardIDistributedCacheProvider<TCache> : CacheRoundTripSpecsBase
    {
        private void Should_roundtrip_this_variant_of<TResult>(TResult testValue)
        {
            // Arrange
            var (memoryIDistributedCacheProvider, cache) = CachePolicyFactory.CreateCachePolicy<TCache, TResult>();

            // Assert - should not be in cache
            (bool cacheHit1, TResult fromCache1) = memoryIDistributedCacheProvider.TryGet(OperationKey);
            cacheHit1.Should().BeFalse();
            fromCache1.Should().Be(default(TResult));

            // Act - should execute underlying delegate and place in cache
            int underlyingDelegateExecuteCount = 0;
            cache.Execute(ctx =>
                {
                    underlyingDelegateExecuteCount++;
                    return testValue;
                }, new Context(OperationKey))
                .ShouldBeEquivalentTo(testValue);

            // Assert - should have executed underlying delegate
            underlyingDelegateExecuteCount.Should().Be(1);

            // Assert - should be in cache
            (bool cacheHit2, TResult fromCache2) = memoryIDistributedCacheProvider.TryGet(OperationKey);
            cacheHit2.Should().BeTrue();
            fromCache2.ShouldBeEquivalentTo(testValue);

            // Act - should execute underlying delegate and place in cache
            cache.Execute(ctx =>
            {
                underlyingDelegateExecuteCount++;
                throw new Exception("Cache should be used so this should not get invoked.");
            }, new Context(OperationKey))
                .ShouldBeEquivalentTo(testValue);
            underlyingDelegateExecuteCount.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(SampleClassData))]
        public void Should_roundtrip_all_variants_of_reference_type(SampleClass testValue)
        {
            Should_roundtrip_this_variant_of<SampleClass>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleStringData))]
        public void Should_roundtrip_all_variants_of_string(String testValue)
        {
            Should_roundtrip_this_variant_of<String>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleNumericData))]
        public void Should_roundtrip_all_variants_of_numeric(int testValue)
        {
            Should_roundtrip_this_variant_of<int>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleEnumData))]
        public void Should_roundtrip_all_variants_of_enum(SampleEnum testValue)
        {
            Should_roundtrip_this_variant_of<SampleEnum>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleBoolData))]
        public void Should_roundtrip_all_variants_of_bool(bool testValue)
        {
            Should_roundtrip_this_variant_of<bool>(testValue);
        }

        [Theory]
        [MemberData(nameof(SampleNullableBoolData))]
        public void Should_roundtrip_all_variants_of_nullable_bool(bool? testValue)
        {
            Should_roundtrip_this_variant_of<bool?>(testValue);
        }
    }
}

#endif