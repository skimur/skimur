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
    public class HotTests
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
        public void Can_calculate_hot_factor()
        {
            // arrange
            var currentTime = DateTime.SpecifyKind(DateTime.Parse("7/12/2015 7:48:12 PM"), DateTimeKind.Utc);
            var posts = JsonConvert.DeserializeObject<List<JsonPost>>(
                    File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sorting_hot.json"))).Select(
                        x =>
                        {
                            x.Created = DateTime.SpecifyKind(x.Created, DateTimeKind.Utc);
                            return x;
                        })
                        .OrderBy(x => Guid.NewGuid())
                        .ToList();

            // act
            var sortedPosts = SortPosts(currentTime, posts);

            // assert
            // TODO: assert
            foreach (var post in sortedPosts)
            {
                Console.WriteLine(post.Score + "-" + post.EffectiveScore);
                Console.WriteLine("     " + (currentTime - post.Created).TotalHours);
            }
        }

        private List<JsonPost> SortPosts(DateTime now, List<JsonPost> posts)
        {
            foreach (var post in posts)
            {
                post.EffectiveScore = Hot.GetHot(post.Score, post.Created.ToUnixTime());
            }

            return posts.OrderByDescending(x => x.EffectiveScore).ToList();
        }
    }
}
