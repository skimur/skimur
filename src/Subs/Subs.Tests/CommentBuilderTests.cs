using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Utils;
using Microsoft.ClearScript;
using Moq;
using NUnit.Framework;
using Skimur;
using Skimur.Tests;
using Subs.ReadModel;
using Subs.Services;

namespace Subs.Tests
{
    [TestFixture]
    public class CommentBuilderTests
    {
        private Mock<ICommentService> _commentService;
        private ICommentTreeBuilder _commentTreeBuilder;
        private ICommentTreeContextBuilder _commentTreeContextBuilder;

        [Test]
        public void Can_build_comment_tree()
        {
            // arrange
            var comments = CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(null);
            var sorter = comments.ToDictionary(x => x.Id, x => (double)comments.IndexOf(comments.Single(y => y.Id == x.Id)));

            // act
            var result = _commentTreeContextBuilder.Build(tree, sorter);

            // assert
            Assert.That(result.Comments, Has.Count.EqualTo(1110));
            Assert.That(result.TopLevelComments, Has.Count.EqualTo(10));
        }

        [Test]
        public void Can_get_children_count()
        {
            // arrange
            var comment = new Comment();
            comment.Id = Guid.NewGuid();
            var commentChild1 = new Comment();
            commentChild1.Id = Guid.NewGuid();
            commentChild1.ParentId = comment.Id;
            var commentChild2 = new Comment();
            commentChild2.Id = Guid.NewGuid();
            commentChild2.ParentId = comment.Id;
            var grandChild1 = new Comment();
            grandChild1.Id = Guid.NewGuid();
            grandChild1.ParentId = commentChild2.Id;
            SetupComments(new List<Comment> { comment, commentChild1, commentChild2, grandChild1 });
            var tree = _commentTreeBuilder.GetCommentTree(null);

            // act
            var result = _commentTreeContextBuilder.Build(tree, new Dictionary<Guid, double>());

            // assert
            Assert.That(result.CommentsChildrenCount[comment.Id], Is.EqualTo(3));
            Assert.That(result.CommentsChildrenCount[commentChild1.Id], Is.EqualTo(0));
            Assert.That(result.CommentsChildrenCount[commentChild2.Id], Is.EqualTo(1));
            Assert.That(result.CommentsChildrenCount[grandChild1.Id], Is.EqualTo(0));
        }

        [Test]
        public void Can_get_recursive_children()
        {
            // arrange
            var comment = new Comment();
            comment.Id = Guid.NewGuid();
            var commentChild1 = new Comment();
            commentChild1.Id = Guid.NewGuid();
            commentChild1.ParentId = comment.Id;
            var commentChild2 = new Comment();
            commentChild2.Id = Guid.NewGuid();
            commentChild2.ParentId = comment.Id;
            var grandChild1 = new Comment();
            grandChild1.Id = Guid.NewGuid();
            grandChild1.ParentId = commentChild2.Id;
            SetupComments(new List<Comment> { comment, commentChild1, commentChild2, grandChild1 });
            var tree = _commentTreeBuilder.GetCommentTree(null);

            // act
            var result = _commentTreeContextBuilder.Build(tree, new Dictionary<Guid, double>(), maxDepth: 1);

            // assert
            Assert.That(result.MoreRecursion, Has.Count.EqualTo(1));
            Assert.IsTrue(result.MoreRecursion.Contains(comment.Id));

            // act
            result = _commentTreeContextBuilder.Build(tree, new Dictionary<Guid, double>(), maxDepth: 2);

            // assert
            Assert.That(result.MoreRecursion, Has.Count.EqualTo(1));
            Assert.IsTrue(result.MoreRecursion.Contains(commentChild2.Id));
        }

        [Test]
        public void Can_get_specific_children()
        {
            // arrange
            CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(null);
            var comment1 = tree.Tree[Guid.Empty][0];
            var comment2 = tree.Tree[Guid.Empty][2];

            // act
            var result = _commentTreeContextBuilder.Build(tree, null, new List<Guid> { comment1, comment2});

            // assert
            Assert.That(result.TopLevelComments, Has.Count.EqualTo(2));
            CollectionAssert.Contains(result.TopLevelComments, comment1);
            CollectionAssert.Contains(result.TopLevelComments, comment2);
        }

        [SetUp]
        public void Setup()
        {
            _commentService = new Mock<ICommentService>();
            _commentTreeBuilder = new CommentTreeBuilder(_commentService.Object);
            _commentTreeContextBuilder = new CommentTreeContextBuilder();
        }

        private List<Comment> CreateTreeComments()
        {
            var comments = new List<Comment>();
            for (var x = 0; x < 10; x++)
            {
                var comment = new Comment();
                comment.Id = GuidUtil.NewSequentialId();
                comment.VoteUpCount = x % 10;
                comment.VoteDownCount = x % 5;
                comment.DateCreated = Common.CurrentTime();
                comments.Add(comment);
                for (var y = 0; y < 10; y++)
                {
                    var child = new Comment();
                    child.Id = GuidUtil.NewSequentialId();
                    child.VoteUpCount = y % 10;
                    child.VoteDownCount = y % 5;
                    child.ParentId = comment.Id;
                    child.DateCreated = Common.CurrentTime();
                    comments.Add(child);

                    for (var z = 0; z < 10; z++)
                    {
                        var grandChild = new Comment();
                        grandChild.Id = GuidUtil.NewSequentialId();
                        grandChild.VoteUpCount = z % 10;
                        grandChild.VoteDownCount = z % 5;
                        grandChild.ParentId = child.Id;
                        grandChild.DateCreated = Common.CurrentTime();
                        comments.Add(grandChild);
                    }
                }
            }

            comments = comments.OrderByDescending(x => Sorting.Confidence(x.VoteUpCount, x.VoteDownCount)).ThenByDescending(x => x.DateCreated).ToList();

            SetupComments(comments);

            return comments;
        }

        private void SetupComments(List<Comment> comments)
        {
            _commentService.Setup(x => x.GetAllCommentsForPost(It.IsAny<string>(), It.IsAny<CommentSortBy?>())).Returns(comments);
        }
    }
}
