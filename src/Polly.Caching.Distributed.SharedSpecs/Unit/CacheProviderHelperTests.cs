using System;
using FluentAssertions;
using Moq;
using Polly.Caching.Distributed;
using Xunit;

namespace Polly.Specs.Caching.Distributed.Unit
{
    public class CacheProviderHelperTests
    {
        [Fact]
        public void Should_configure_sync_string_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsSyncCacheProvider<string>();

            configure.ShouldNotThrow();
        }

        [Fact]
        public void Should_configure_sync_bytearray_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();

            configure.ShouldNotThrow();
        }

        [Fact]
        public void Should_configure_async_string_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsAsyncCacheProvider<string>();

            configure.ShouldNotThrow();
        }

        [Fact]
        public void Should_configure_async_bytearray_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            configure.ShouldNotThrow();
        }

        [Fact]
        public void Should_throw_for_unsupported_type_of_results_to_cache_sync()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsSyncCacheProvider<object>();

            configure.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Should_throw_for_unsupported_type_of_results_to_cache_async()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsAsyncCacheProvider<object>();

            configure.ShouldThrow<ArgumentException>();
        }

    }
}