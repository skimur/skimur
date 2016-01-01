using FluentAssertions;
using Xunit;

namespace Skimur.Tests
{
    public class NsfwDetectionTests
    {
        [Fact]
        public void Can_detect_nsfw()
        {
            Common.IsNsfw("Test").Should().Be(false);
            Common.IsNsfw("NSFW test").Should().Be(true);
            Common.IsNsfw("NSFW: test").Should().Be(true);
            Common.IsNsfw("NSFL test").Should().Be(true);
            Common.IsNsfw("NSFL: test").Should().Be(true);
        }
    }
}
