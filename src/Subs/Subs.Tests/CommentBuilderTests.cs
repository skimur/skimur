using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Utils;
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

        [Test]
        public void Can_build_comment_tree()
        {
            // arrange
            CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(null);
            var builder = new CommentBuilder(tree);

            // act
            builder.BuildForTopLevelComments(null, int.MaxValue);

            // assert
            Assert.That(builder.Comments, Has.Count.EqualTo(1110));
            Assert.That(builder.TopLevelComments, Has.Count.EqualTo(10));
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
            var builder = new CommentBuilder(tree);

            // act
            builder.BuildForTopLevelComments(null, int.MaxValue);

            // assert
            Assert.That(builder.CommentChildrenCount[comment.Id], Is.EqualTo(3));
            Assert.That(builder.CommentChildrenCount[commentChild1.Id], Is.EqualTo(0));
            Assert.That(builder.CommentChildrenCount[commentChild2.Id], Is.EqualTo(1));
            Assert.That(builder.CommentChildrenCount[grandChild1.Id], Is.EqualTo(0));
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
            var builder = new CommentBuilder(tree);

            // act
            builder.BuildForTopLevelComments(null, 1);

            // assert
            Assert.That(builder.MoreRecursion, Has.Count.EqualTo(1));
            Assert.IsTrue(builder.MoreRecursion.Contains(comment.Id));

            // act
            builder.BuildForTopLevelComments(null, 2);

            // assert
            Assert.That(builder.MoreRecursion, Has.Count.EqualTo(1));
            Assert.IsTrue(builder.MoreRecursion.Contains(commentChild2.Id));
        }

        [SetUp]
        public void Setup()
        {
            _commentService = new Mock<ICommentService>();
            _commentTreeBuilder = new CommentTreeBuilder(_commentService.Object);
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
