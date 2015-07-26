using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using Skimur;

namespace Subs.Tests
{
    [TestFixture]
    public class SortTests
    {
        public class JsonPost
        {
            [JsonProperty("title")]
            public string Title { get; set; }

            [JsonProperty("score")]
            public int Score { get; set; }

            [JsonProperty("created")]
            public DateTime Created { get; set; }

            [JsonIgnore]
            public double EffectiveScore { get; set; }
        }

        [Test]
        public void Can_calculate_hot()
        {
            Assert.That(Sorting.Hot(Sorting.Score(4, 3), 1437872723), Is.EqualTo(6752.1048889));
        }

        [Test]
        public void Can_calculate_controversy()
        {
            Assert.That(Sorting.Controversy(4, 3), Is.EqualTo(4.303517070658851));
        }

        [Test]
        public void Can_calculate_confidence()
        {
            Assert.That(Sorting.Confidence(4, 3), Is.EqualTo(0.34169705362084496));
            Assert.That(Sorting.CachedConfidence(4, 3), Is.EqualTo(0.34169705362084496));

            Assert.That(Sorting.Confidence(400, 100), Is.EqualTo(0.7761092039709403));
            Assert.That(Sorting.CachedConfidence(400, 100), Is.EqualTo(0.7761092039709403));

            Assert.That(Sorting.Confidence(399, 99), Is.EqualTo(0.7773119937342073));
            Assert.That(Sorting.CachedConfidence(399, 99), Is.EqualTo(0.7773119937342073));
        }

        [Test]
        public void Can_calculate_qa()
        {
            Assert.That(Sorting.Qa(4, 30, 30, 500), Is.EqualTo(34.544855173920155));
            Assert.That(Sorting.Qa(4, 30, 40, 500), Is.EqualTo(44.544855173920155));
            Assert.That(Sorting.Qa(4, 30, 50, 500), Is.EqualTo(54.544855173920155));
            Assert.That(Sorting.Qa(4, 4, 4, new List<Comment> { new Comment { VoteUpCount = 5, VoteDownCount = 5, Body = "12345" } }), Is.EqualTo(0.7966983754040223));
        }

        private List<JsonPost> SortPosts(DateTime now, List<JsonPost> posts)
        {
            foreach (var post in posts)
            {
                post.EffectiveScore = Sorting.Hot(post.Score, post.Created.ToUnixTime());
            }

            return posts.OrderByDescending(x => x.EffectiveScore).ToList();
        }
    }
}
