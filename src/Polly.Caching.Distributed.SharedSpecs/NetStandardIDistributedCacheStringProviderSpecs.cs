using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Polly.Caching;
using Polly.Caching.Distributed;
using Xunit;

namespace Polly.Specs.Caching.IDistributedCache
{
    public class NetStandardIDistributedCacheStringProviderSpecs
    {
        #region Configuration

        [Fact]
        public void Should_throw_when_IDistributedCache_is_null()
        {
            Action configure = () => ((Microsoft.Extensions.Caching.Distributed.IDistributedCache)null).AsSyncCacheProvider<string>();

            configure.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("iDistributedCache");

        }

        [Fact]
        public void Should_not_throw_when_IDistributedCache_is_not_null()
        {
            Microsoft.Extensions.Caching.Distributed.IDistributedCache mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>().Object;
            Action configure = () => mockDistributedCache.AsSyncCacheProvider<string>();

            configure.ShouldNotThrow();
        }

        #endregion

        #region Get

        [Fact]
        public void Get_should_return_instance_previously_stored_in_cache()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k == key))).Returns(new byte[0]{}).Verifiable(); // Because GetString() is an extension method, we cannot mock it.  We mock Get() instead.  

            ISyncCacheProvider<string> provider = mockDistributedCache.Object.AsSyncCacheProvider<string>();
            string got = provider.Get(key);

            mockDistributedCache.Verify(v => v.Get(key), Times.Once);
        }

        [Fact]
        public void Get_should_return_null_on_unknown_key()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            mockDistributedCache.Setup(idc => idc.Get(It.Is<string>(k => k == key))).Verifiable(); // Because GetString() is an extension method, we cannot mock it.  We mock Get() instead.  

            ISyncCacheProvider<string> provider = mockDistributedCache.Object.AsSyncCacheProvider<string>();
            string someOtherKey = Guid.NewGuid().ToString();
            string got = provider.Get(someOtherKey);

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
            var valueToCache = "something to cache";

            ISyncCacheProvider<string> provider = mockDistributedCache.Object.AsSyncCacheProvider<string>();

            mockDistributedCache.Setup(idc => idc.Set(It.Is<string>(k => k == key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>())).Verifiable(); // Because SetString() is an extension method, we cannot mock it.  We mock Set() instead.  

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);
            provider.Put(key, valueToCache, ttl);

            mockDistributedCache.Verify(idc => idc.Set(key, It.IsAny<byte[]>(), It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == timespan)));
        }

        [Fact]
        public void Put_should_put_item_using_passed_sliding_ttl()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = "something to cache";

            ISyncCacheProvider<string> provider = mockDistributedCache.Object.AsSyncCacheProvider<string>();

            mockDistributedCache.Setup(idc => idc.Set(It.Is<string>(k => k == key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>())).Verifiable(); // Because SetString() is an extension method, we cannot mock it.  We mock Set() instead.  

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, true);
            provider.Put(key, valueToCache, ttl);

            mockDistributedCache.Verify(idc => idc.Set(key, It.IsAny<byte[]>(), It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == timespan)));
        }

        #endregion


        #region Get async

        [Fact]
        public async Task GetAsync_should_return_instance_previously_stored_in_cache()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k == key)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.FromResult(new byte[0] {  })).Verifiable(); // Because GetStringAsync() is an extension method, we cannot mock it.  We mock GetAsync() instead.  

            IAsyncCacheProvider<string> provider = mockDistributedCache.Object.AsAsyncCacheProvider<string>();
            string got = await provider.GetAsync(key, CancellationToken.None, false);

            mockDistributedCache.Verify(v => v.GetAsync(key
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ), Times.Once);
        }

        [Fact]
        public async Task GetAsync_should_return_null_on_unknown_key()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            mockDistributedCache.Setup(idc => idc.GetAsync(It.Is<string>(k => k == key)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.FromResult(new byte[0] { })); // Because GetStringAsync() is an extension method, we cannot mock it.  We mock GetAsync() instead.  

            IAsyncCacheProvider<string> provider = mockDistributedCache.Object.AsAsyncCacheProvider<string>();
            string someOtherKey = Guid.NewGuid().ToString();
            string got = await provider.GetAsync(someOtherKey, CancellationToken.None, false);

            mockDistributedCache.Verify(v => v.GetAsync(someOtherKey
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ), Times.Once);
            got.Should().BeNull();
        }

        [Fact]
        public void GetAsync_should_throw_for_cancellation()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";

            IAsyncCacheProvider<string> provider = mockDistributedCache.Object.AsAsyncCacheProvider<string>();
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
            var valueToCache = "something to cache";

            IAsyncCacheProvider<string> provider = mockDistributedCache.Object.AsAsyncCacheProvider<string>();

            mockDistributedCache.Setup(idc => idc.SetAsync(It.Is<string>(k => k == key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.CompletedTask).Verifiable(); // Because SetStringAsync() is an extension method, we cannot mock it.  We mock SetAsync() instead.  

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);

            await provider.PutAsync(key, valueToCache, ttl, CancellationToken.None, false);

            mockDistributedCache.Verify(idc => idc.SetAsync(key, It.IsAny<byte[]>(), It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == timespan)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                ));
        }

        [Fact]
        public async Task PutAsync_should_put_item_using_passed_sliding_ttl()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = "something to cache";

            IAsyncCacheProvider<string> provider = mockDistributedCache.Object.AsAsyncCacheProvider<string>();

            mockDistributedCache.Setup(idc => idc.SetAsync(It.Is<string>(k => k == key), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
                )).Returns(Task.CompletedTask); // Because SetStringAsync() is an extension method, we cannot mock it.  We mock SetAsync() instead.  

            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, true);
            await provider.PutAsync(key, valueToCache, ttl, CancellationToken.None, false);

            mockDistributedCache.Verify(idc => idc.SetAsync(key, It.IsAny<byte[]>(), It.Is<DistributedCacheEntryOptions>(o => o.SlidingExpiration == timespan)
#if NETCOREAPP2_0
, It.IsAny<CancellationToken>()
#endif
));
        }

        [Fact]
        public void PutAsync_should_throw_for_cancellation()
        {
            Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache> mockDistributedCache = new Mock<Microsoft.Extensions.Caching.Distributed.IDistributedCache>();
            string key = "anything";
            var valueToCache = "something to cache";
            TimeSpan timespan = TimeSpan.FromSeconds(10);
            Ttl ttl = new Ttl(timespan, false);

            IAsyncCacheProvider<string> provider = mockDistributedCache.Object.AsAsyncCacheProvider<string>();

            Func<Task> action = () => provider.PutAsync(key, valueToCache, ttl, new CancellationToken(true), false);
            action.ShouldThrow<OperationCanceledException>();
        }
#endregion
    }
    
}