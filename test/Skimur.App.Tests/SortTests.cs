
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Skimur.App.Tests
{
    public class SortTests
    {
        [Fact]
        public void Can_calculate_hot()
        {
            Sorting.Hot(Sorting.Score(4, 3), 1437872723).Should().Be(6752.1048889);
        }

        [Fact]
        public void Can_calculate_controversy()
        {
            Sorting.Controversy(4, 3).Should().Be(4.303517070658851);
        }

        [Fact]
        public void Can_calculate_confidence()
        {
            Sorting.Confidence(4, 3).Should().Be(0.34169705362084496);
            Sorting.CachedConfidence(4, 3).Should().Be(0.34169705362084496);

            Sorting.Confidence(400, 100).Should().Be(0.7761092039709403);
            Sorting.CachedConfidence(400, 100).Should().Be(0.7761092039709403);

            Sorting.Confidence(399, 99).Should().Be(0.7773119937342073);
            Sorting.CachedConfidence(399, 99).Should().Be(0.7773119937342073);
        }

        [Fact]
        public void Can_calculate_qa()
        {
            Sorting.Qa(4, 30, 30, 500).Should().Be(34.544855173920155);
            Sorting.Qa(4, 30, 40, 500).Should().Be(44.544855173920155);
            Sorting.Qa(4, 30, 50, 500).Should().Be(54.544855173920155);
            Sorting.Qa(4, 4, 4, new List<Comment> {new Comment {VoteUpCount = 5, VoteDownCount = 5, Body = "12345"}}).Should().Be(0.7966983754040223);
        }
    }
}
