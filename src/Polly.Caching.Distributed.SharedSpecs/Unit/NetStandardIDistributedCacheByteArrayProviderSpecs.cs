using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Polly.Caching;
using Polly.Caching.Distributed;
using Xunit;

namespace Polly.Specs.Caching.Distributed.Unit
{
    public class NetStandardIDistributedCacheByteArrayProviderSpecs
    {
        #region Configuration

        [Fact]
        public void Should_throw_when_IDistributedCache_is_null()
        {
           Action configure = () => ((IDistributedCache)null).AsSyncCacheProvider<byte[]>();

            configure.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("iDistributedCache");

        }

        [Fact]
        public void Should_not_throw_when_IDistributedCache_is_not_null()
        {
            IDistributedCache mockDistributedCache = new Mock<IDistributedCache>().Object;
            Action configure = () => mockDistributedCache.AsSyncCacheProvider<byte[]>();

            configure.ShouldNotThrow();
        }

        #endregion

        #region Get

        [Fact]
        public void Get_should_return_instance_previously_stored_in_cache()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var cachedValue = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k == key))).Returns(cachedValue).Verifiable();

            ISyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();
            (bool got, byte[] fromCache) = provider.TryGet(key);

            got.Should().BeTrue();
            mockDistributedCache.Verify(v => v.Get(key), Times.Once);
            fromCache.Should().BeSameAs(cachedValue);
        }

        [Fact]
        public void Get_should_return_false_on_unknown_key()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var cachedValue = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k == key))).Returns(cachedValue).Verifiable();
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k != key))).Returns((byte[])null).Verifiable();

            ISyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsSyncCacheProvider<byte[]>();
            string someOtherKey = Guid.NewGuid().ToString();
            (bool got, byte[] fromCache) = provider.TryGet(someOtherKey);

            got.Should().BeFalse();
            mockDistributedCache.Verify(v => v.Get(someOtherKey), Times.Once);
            fromCache.Should().BeNull();
        }

        #endregion

        #region Put
        
        [Fact]
        public void Put_should_put_item_using_passed_nonsliding_ttl()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var valueToCache = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

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
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var valueToCache = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

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
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var cachedValue = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k == key)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.FromResult(cachedValue));

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();
            (bool got, byte[] fromCache) = await provider.TryGetAsync(key, CancellationToken.None, false);

            got.Should().BeTrue();
            mockDistributedCache.Verify(v => v.GetAsync(key
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ), Times.Once);
            fromCache.Should().BeSameAs(cachedValue);
        }

        [Fact]
        public async Task GetAsync_should_return_false_on_unknown_key()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var cachedValue = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k == key)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
            )).Returns(Task.FromResult(cachedValue));

            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k != key)
#if NETCOREAPP2_0
                , It.IsAny<CancellationToken>()
#endif
            )).Returns(Task.FromResult<byte[]>(null)).Verifiable();


            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();
            string someOtherKey = Guid.NewGuid().ToString();
            (bool got, byte[] fromCache) = await provider.TryGetAsync(someOtherKey, CancellationToken.None, false);

            got.Should().BeFalse();
            mockDistributedCache.Verify(v => v.GetAsync(someOtherKey
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ), Times.Once);
            fromCache.Should().BeNull();
        }

        [Fact]
        public void GetAsync_should_throw_for_cancellation()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();
            Func<Task> action = () => provider.TryGetAsync(key, new CancellationToken(true), false);
            action.ShouldThrow<OperationCanceledException>();
        }

        #endregion

        #region Put async

        [Fact]
        public async Task PutAsync_should_put_item_using_passed_nonsliding_ttl()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var valueToCache = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            mockDistributedCache.Setup(idc => idc.SetAsync(It.Is<string>(k => k == key), It.Is<byte[]>(v => v == valueToCache), It.IsAny<DistributedCacheEntryOptions>()
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.CompletedTask).Verifiable();

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);

            await provider.PutAsync(key, valueToCache, ttl, CancellationToken.None, false);

            mockDistributedCache.Verify(idc => idc.SetAsync(key, valueToCache, It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == timespan)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ));
        }

        [Fact]
        public async Task PutAsync_should_put_item_using_passed_sliding_ttl()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var valueToCache = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            mockDistributedCache.Setup(idc => idc.SetAsync(It.Is<string>(k => k == key), It.Is<byte[]>(v => v == valueToCache), It.IsAny<DistributedCacheEntryOptions>()
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.CompletedTask).Verifiable();

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, true);
            await provider.PutAsync(key, valueToCache, ttl, CancellationToken.None, false);

            mockDistributedCache.Verify(idc => idc.SetAsync(key, valueToCache, It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == timespan)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ));
        }

        [Fact]
        public void PutAsync_should_throw_for_cancellation()
        {
            Mock<IDistributedCache> mockDistributedCache = new Mock<IDistributedCache>();
            string key = "anything";
            var valueToCache = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);

            IAsyncCacheProvider<byte[]> provider = mockDistributedCache.Object.AsAsyncCacheProvider<byte[]>();

            Func<Task> action = () => provider.PutAsync(key, valueToCache, ttl, new CancellationToken(true), false);
            action.ShouldThrow<OperationCanceledException>();
        }
        #endregion
    }
}