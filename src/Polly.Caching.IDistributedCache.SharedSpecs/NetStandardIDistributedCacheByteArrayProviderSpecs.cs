using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Polly.Caching;
using Polly.Caching.IDistributedCache;
using Xunit;

namespace Polly.Specs.Caching.IDistributedCache
{
    public class NetStandardIDistributedCacheByteArrayProviderSpecs
    {
        #region Configuration

        [Fact]
        public void Should_throw_when_IDistributedCache_is_null()
        {
           Action configure = () => ((Microsoft.Extensions.Caching.Distributed.IDistributedCache)null).AsSyncCacheProvider<byte[]>();

            configure.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("iDistributedCache");

        }

        [Fact]
        public void Should_not_throw_when_IDistributedCache_is_not_null()
        {
            Microsoft.Extensions.Caching.Distributed.IDistributedCache mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>().Object;
            Action configure = () => mockDistributedCache.AsSyncCacheProvider<byte[]>();

            configure.ShouldNotThrow();
        }

        #endregion

        #region Get

        [Fact]
        public void Get_should_return_instance_previously_stored_in_cache()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var cachedValue = new byte[]{0};
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k == key))).Returns(cachedValue).Verifiable();

            ISyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();
            byte[] got = provider.Get(key);

            mockDistributedCache.Verify(v => v.Get(key), Times.Once);
            got.Should().BeSameAs(cachedValue);
        }

        [Fact]
        public void Get_should_return_null_on_unknown_key()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var cachedValue = new byte[] { 0 };
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k == key))).Returns(cachedValue).Verifiable();

            ISyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();
            string someOtherKey = Guid.NewGuid().ToString();
            byte[] got = provider.Get(someOtherKey);

            mockDistributedCache.Verify(v => v.Get(someOtherKey), Times.Once);
            got.Should().BeNull();
        }

        #endregion

        #region Put
        
        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = new byte[] { 0 };

            ISyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();

            mockDistributedCache.Setup(idc => idc.Set(It.Is<string>(k => k == key), It.Is<byte[]>(v => v == valueToCache), It.IsAny<DistributedCacheEntryOptions>())).Verifiable();

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);
            provider.Put(key, valueToCache, ttl);

            mockDistributedCache.Verify(idc => idc.Set(key, valueToCache, It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == timespan)));
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = new byte[] { 0 };

            ISyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();

            mockDistributedCache.Setup(idc => idc.Set(It.Is<string>(k => k == key), It.Is<byte[]>(v => v == valueToCache), It.IsAny<DistributedCacheEntryOptions>())).Verifiable();

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, true);
            provider.Put(key, valueToCache, ttl);

            mockDistributedCache.Verify(idc => idc.Set(key, valueToCache, It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == timespan)));
        }

        #endregion


        #region Get async

        [Fact]
        public async Task GetAsync_should_return_instance_previously_stored_in_cache()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var cachedValue = new byte[] { 0 };
            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k == key))).Returns(Task.FromResult(cachedValue)).Verifiable();

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();
            byte[] got = await provider.GetAsync(key, CancellationToken.None, false);

            mockDistributedCache.Verify(v => v.GetAsync(key), Times.Once);
            got.Should().BeSameAs(cachedValue);
        }

        [Fact]
        public async Task GetAsync_should_return_null_on_unknown_key()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var cachedValue = new byte[] { 0 };
            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k == key))).Returns(Task.FromResult(cachedValue)).Verifiable();

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();
            string someOtherKey = Guid.NewGuid().ToString();
            byte[] got = await provider.GetAsync(someOtherKey, CancellationToken.None, false);

            mockDistributedCache.Verify(v => v.GetAsync(someOtherKey), Times.Once);
            got.Should().BeNull();
        }

        [Fact]
        public void GetAsync_should_throw_for_cancellation()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();
            Func<Task> action = () => provider.GetAsync(key, new CancellationToken(true), false);
            action.ShouldThrow<OperationCanceledException>();
        }

        #endregion

        #region Put

        [Fact]
        public async Task PutAsync_should_put_item_using_passed_nonsliding_ttl()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = new byte[] { 0 };

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            mockDistributedCache.Setup(idc => idc.SetAsync(It.Is<string>(k => k == key), It.Is<byte[]>(v => v == valueToCache), It.IsAny<DistributedCacheEntryOptions>())).Returns(Task.CompletedTask).Verifiable();

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);

            await provider.PutAsync(key, valueToCache, ttl, CancellationToken.None, false);

            mockDistributedCache.Verify(idc => idc.SetAsync(key, valueToCache, It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == timespan)));
        }

        [Fact]
        public async Task PutAsync_should_put_item_using_passed_sliding_ttl()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = new byte[] { 0 };

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            mockDistributedCache.Setup(idc => idc.SetAsync(It.Is<string>(k => k == key), It.Is<byte[]>(v => v == valueToCache), It.IsAny<DistributedCacheEntryOptions>())).Returns(Task.CompletedTask).Verifiable();

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, true);
            await provider.PutAsync(key, valueToCache, ttl, CancellationToken.None, false);

            mockDistributedCache.Verify(idc => idc.SetAsync(key, valueToCache, It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == timespan)));
        }

        [Fact]
        public void PutAsync_should_throw_for_cancellation()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = new byte[] { 0 };
            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            Func<Task> action = () => provider.PutAsync(key, valueToCache, ttl, new CancellationToken(true), false);
            action.ShouldThrow<OperationCanceledException>();
        }
        #endregion
    }
}