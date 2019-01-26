using System;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Caching.Distributed;
using Polly.Caching.Distributed;
using Polly.Caching;
using Xunit;

namespace Polly.Specs.Caching.Distributed.Unit
{
    public class TtlStrategyHelperTests
    {
        private Context noContext = new Context(Guid.NewGuid().ToString());

        [Fact]
        public void Can_render_AbsoluteExpiration_as_ttlstrategy()
        {
            TimeSpan forwardTimeSpan = TimeSpan.FromDays(1);
            DateTime date = DateTime.Now.Add(forwardTimeSpan);
            DistributedCacheEntryOptions entryOptions = new DistributedCacheEntryOptions() { AbsoluteExpiration = date };

            ITtlStrategy ttlStrategy = entryOptions.AsTtlStrategy();

            ttlStrategy.Should().BeOfType<AbsoluteTtl>();

            Ttl ttl = ttlStrategy.GetTtl(noContext, null);
            ttl.SlidingExpiration.Should().BeFalse();
            ttl.Timespan.Should().BeCloseTo(forwardTimeSpan, 10000);
        }

        [Fact]
        public void Can_render_AbsoluteExpirationRelativeToNow_as_ttlstrategy()
        {
            TimeSpan forwardTimeSpan = TimeSpan.FromDays(1);
            DistributedCacheEntryOptions entryOptions = new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = forwardTimeSpan };

            ITtlStrategy ttlStrategy = entryOptions.AsTtlStrategy();

            ttlStrategy.Should().BeOfType<RelativeTtl>();

            Ttl ttl = ttlStrategy.GetTtl(noContext, null);
            ttl.SlidingExpiration.Should().BeFalse();
            ttl.Timespan.Should().BeCloseTo(forwardTimeSpan, 10000);
        }

        [Fact]
        public void Can_render_SlidingExpiration_as_ttlstrategy()
        {
            TimeSpan forwardTimeSpan = TimeSpan.FromDays(1);
            DistributedCacheEntryOptions entryOptions = new DistributedCacheEntryOptions() { SlidingExpiration = forwardTimeSpan };

            ITtlStrategy ttlStrategy = entryOptions.AsTtlStrategy();

            ttlStrategy.Should().BeOfType<SlidingTtl>();

            Ttl ttl = ttlStrategy.GetTtl(noContext, null);
            ttl.SlidingExpiration.Should().BeTrue();
            ttl.Timespan.Should().BeCloseTo(forwardTimeSpan);
        }
    }
}