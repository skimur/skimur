using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using Skimur;
using Skimur.Tests;
using Subs.ReadModel;
using Subs.Services;

namespace Subs.Tests
{
    [TestFixture]
    public class PostServiceTests : DataTestBase
    {
        IPostService _postService;

        [Test]
        public void Can_get_posts_by_hot()
        {
            // arrange
            var posts = JsonConvert.DeserializeObject<List<HotTests.JsonPost>>(
                File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sorting_hot.json"))).Select(
                    x =>
                    {
                        x.Created = DateTime.SpecifyKind(x.Created, DateTimeKind.Utc);
                        return x;
                    }).ToList();
            foreach (var post in posts)
            {
                _postService.InsertPost(new Post
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = post.Created,
                    Title = post.Title,
                    VoteUpCount = post.Score
                });
            }

            // act
            var sortedPosts = _postService.GetPosts(sortby: PostsSortBy.Hot);

            // assert
            // TODO: assert
            foreach (var post in sortedPosts)
            {
                Console.WriteLine((post.VoteUpCount - post.VoteDownCount + "-" + post.DateCreated.ToLongDateString() + " " + post.DateCreated.ToLongTimeString()));
            }
        }

        [Test]
        public void Can_get_posts_by_score()
        {
            foreach (var post in JsonConvert.DeserializeObject<List<HotTests.JsonPost>>(
                File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sorting_hot.json"))).Select(
                    x =>
                    {
                        x.Created = DateTime.SpecifyKind(x.Created, DateTimeKind.Utc);
                        return x;
                    }))
            {
                _postService.InsertPost(new Post
                {
                    Id = GuidUtil.NewSequentialId(),
                    DateCreated = post.Created,
                    Title = post.Title,
                    VoteUpCount = post.Score
                });
            }

            // act
            var sortedPosts = _postService.GetPosts(sortby: PostsSortBy.Top);

            // assert
            Assert.That(sortedPosts.Select(x => x.VoteUpCount), Is.Ordered.Descending);
        }

        [Test]
        public void Can_get_posts_by_controversy()
        {
            _postService.InsertPost(new Post
            {
                Id = GuidUtil.NewSequentialId(),
                DateCreated = Common.CurrentTime(),
                Title = "post 1",
                VoteUpCount = 50,
                VoteDownCount = 50
            });
            _postService.InsertPost(new Post
            {
                Id = GuidUtil.NewSequentialId(),
                DateCreated = Common.CurrentTime(),
                Title = "post 2",
                VoteUpCount = 500,
                VoteDownCount = 500
            });
            _postService.InsertPost(new Post
            {
                Id = GuidUtil.NewSequentialId(),
                DateCreated = Common.CurrentTime(),
                Title = "post 3",
                VoteUpCount = 5,
                VoteDownCount = 5
            });

            // act
            var sortedPosts = _postService.GetPosts(sortby: PostsSortBy.Controversial);

            // assert
            Assert.That(sortedPosts[0].Title, Is.EqualTo("post 2"));
            Assert.That(sortedPosts[1].Title, Is.EqualTo("post 1"));
            Assert.That(sortedPosts[2].Title, Is.EqualTo("post 3"));
        }

        [Test]
        public void Can_get_posts_by_new()
        {
            _postService.InsertPost(new Post
            {
                Id = GuidUtil.NewSequentialId(),
                DateCreated = Common.CurrentTime().Subtract(TimeSpan.FromDays(5)),
                Title = "post 1",
                VoteUpCount = 50,
                VoteDownCount = 50
            });
            _postService.InsertPost(new Post
            {
                Id = GuidUtil.NewSequentialId(),
                DateCreated = Common.CurrentTime().Subtract(TimeSpan.FromDays(10)),
                Title = "post 2",
                VoteUpCount = 500,
                VoteDownCount = 500
            });
            _postService.InsertPost(new Post
            {
                Id = GuidUtil.NewSequentialId(),
                DateCreated = Common.CurrentTime().Subtract(TimeSpan.FromDays(7)),
                Title = "post 3",
                VoteUpCount = 5,
                VoteDownCount = 5
            });

            // act
            var sortedPosts = _postService.GetPosts();

            // assert
            Assert.That(sortedPosts[0].Title, Is.EqualTo("post 1"));
            Assert.That(sortedPosts[1].Title, Is.EqualTo("post 3"));
            Assert.That(sortedPosts[2].Title, Is.EqualTo("post 2"));
        }

        protected override void Setup()
        {
            base.Setup();
            _postService = _container.GetInstance<IPostService>();
        }

        protected override List<IRegistrar> GetRegistrars()
        {
            var result = base.GetRegistrars();

            // ReSharper disable RedundantNameQualifier
            result.Add(new Subs.Registrar());
            // ReSharper restore RedundantNameQualifier

            return result;
        }
    }
}
