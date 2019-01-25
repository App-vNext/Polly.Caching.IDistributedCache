#if !NETCOREAPP1_1

using System;
using System.Text;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly.Caching;
using Polly.Caching.Distributed;
using Polly.Caching.Serialization.Json;
using Xunit;

namespace Polly.Specs.Caching.DistributedNew
{
    public class NetStandardIDistributedCacheProvider_CacheRoundTripSpecs_String : NetStandardIDistributedCacheProvider_CacheRoundTripSpecs<string> { }

    public class NetStandardIDistributedCacheProvider_CacheRoundTripSpecs_ByteArray : NetStandardIDistributedCacheProvider_CacheRoundTripSpecs<byte[]> { }

    public abstract class NetStandardIDistributedCacheProvider_CacheRoundTripSpecs<TCache>
    {

        const string OperationKey = "SomeOperationKey";

        #region Round-trip

        private class ByteArraySerializer : ICacheItemSerializer<string, byte[]>
        {
            public string Deserialize(byte[] objectToDeserialize)
            {
                return objectToDeserialize == null ? null : Encoding.UTF8.GetString(objectToDeserialize);
            }

            public byte[] Serialize(string objectToSerialize)
            {
                return objectToSerialize == null ? null : Encoding.UTF8.GetBytes(objectToSerialize);
            }
        }

        private (ISyncCacheProvider<TResult>, ISyncPolicy<TResult>) GetFreshCachePolicy<TResult>()
        {
            var memoryIDistributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            ISyncCacheProvider<TResult> memoryIDistributedCacheProvider;
            if (typeof(TCache) == typeof(string))
            {
                memoryIDistributedCacheProvider = memoryIDistributedCache.AsSyncCacheProvider<string>().WithSerializer<TResult, string>(new JsonSerializer<TResult>(new JsonSerializerSettings()));
            }
            else if (typeof(TCache) == typeof(byte[]))
            {
                memoryIDistributedCacheProvider = memoryIDistributedCache.AsSyncCacheProvider<byte[]>().WithSerializer(new ByteArraySerializer()).WithSerializer<TResult, string>(new JsonSerializer<TResult>(new JsonSerializerSettings()));
            }
            else
            {
                throw new ArgumentException($"{nameof(TCache)} must be either {typeof(string).Name} or {typeof(byte).Name}[]", nameof(TCache));
            }

            var policy = Policy.Cache<TResult>(memoryIDistributedCacheProvider, TimeSpan.FromHours(1));
            return (memoryIDistributedCacheProvider, policy);
        }

        private void Should_roundtrip_this_variant_of<TResult>(TResult testValue)
        {
            // Arrange
            var (memoryIDistributedCacheProvider, cache) = GetFreshCachePolicy<TResult>();

            // Assert - should not be in cache
            /* v7.0 (bool cacheHit1, object fromCache1) = memoryIDistributedCacheProvider.TryGet(OperationKey);
             cacheHit1.Should().BeFalse();*/
            TResult fromCache1 = memoryIDistributedCacheProvider.Get(OperationKey);
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
            /* v7.0 (bool cacheHit2, object fromCache2) = memoryIDistributedCacheProvider.TryGet(OperationKey);
             cacheHit2.Should().BeTrue();*/
            TResult fromCache2 = memoryIDistributedCacheProvider.Get(OperationKey);
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

        public static TheoryData<SampleClass> SampleClassData =>
            new TheoryData<SampleClass>
            {
               new SampleClass(),
               new SampleClass()
               {
                   StringProperty = "<html></html>",
                   IntProperty = 1
               },
               (SampleClass)null,
               default(SampleClass)
            };

        public class SampleClass
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
        }

        [Theory]
        [MemberData(nameof(SampleStringData))]
        public void Should_roundtrip_all_variants_of_string(String testValue)
        {
            Should_roundtrip_this_variant_of<String>(testValue);
        }

        public static TheoryData<String> SampleStringData =>
            new TheoryData<String>
            {
                "some string",
                "",
                null,
                default(string),
                "null"
            };

        [Theory]
        [MemberData(nameof(SampleNumericData))]
        public void Should_roundtrip_all_variants_of_numeric(int testValue)
        {
            Should_roundtrip_this_variant_of<int>(testValue);
        }


        public static TheoryData<int> SampleNumericData =>
            new TheoryData<int>
            {
               -1,
               0,
               1,
               default(int)
            };

        [Theory]
        [MemberData(nameof(SampleEnumData))]
        public void Should_roundtrip_all_variants_of_enum(SampleEnum testValue)
        {
            Should_roundtrip_this_variant_of<SampleEnum>(testValue);
        }


        public static TheoryData<SampleEnum> SampleEnumData =>
            new TheoryData<SampleEnum>
            {
              SampleEnum.FirstValue,
              SampleEnum.SecondValue,
              default(SampleEnum),
            };

        public enum SampleEnum
        {
            FirstValue,
            SecondValue,
        }

        [Theory]
        [MemberData(nameof(SampleBoolData))]
        public void Should_roundtrip_all_variants_of_bool(bool testValue)
        {
            Should_roundtrip_this_variant_of<bool>(testValue);
        }

        public static TheoryData<bool> SampleBoolData =>
            new TheoryData<bool>
            {
               true,
               false,
               default(bool),
            };

        [Theory]
        [MemberData(nameof(SampleNullableBoolData))]
        public void Should_roundtrip_all_variants_of_nullable_bool(bool? testValue)
        {
            Should_roundtrip_this_variant_of<bool?>(testValue);
        }

        public static TheoryData<bool?> SampleNullableBoolData =>
            new TheoryData<bool?>
            {
                true,
                false,
                null,
                default(bool?),
            };

        #endregion
    }
}

#endif