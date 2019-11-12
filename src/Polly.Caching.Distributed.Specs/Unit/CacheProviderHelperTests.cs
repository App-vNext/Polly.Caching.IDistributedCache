using System;
using FluentAssertions;
using Moq;
using Xunit;

namespace Polly.Caching.Distributed.Specs.Unit
{
    public class CacheProviderHelperTests
    {
        [Fact]
        public void Should_configure_sync_string_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsSyncCacheProvider<string>();

            configure.Should().NotThrow();
        }

        [Fact]
        public void Should_configure_sync_bytearray_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();

            configure.Should().NotThrow();
        }

        [Fact]
        public void Should_configure_async_string_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsAsyncCacheProvider<string>();

            configure.Should().NotThrow();
        }

        [Fact]
        public void Should_configure_async_bytearray_provider()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            configure.Should().NotThrow();
        }

        [Fact]
        public void Should_throw_for_unsupported_type_of_results_to_cache_sync()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsSyncCacheProvider<object>();

            configure.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Should_throw_for_unsupported_type_of_results_to_cache_async()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();

            Action configure = () => mockDistributedCache.Object.AsAsyncCacheProvider<object>();

            configure.Should().Throw<ArgumentException>();
        }

    }
}