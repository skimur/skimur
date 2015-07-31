using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Comment> _comments;

        [Test]
        public void Can_build_comment_tree()
        {
            // arrange
            var tree = _commentTreeBuilder.GetCommentTree(null);
            var builder = new TestCommentBuilder(tree, _comments);

            // act
            var result = builder.GetTopLevelComments(null, int.MaxValue);
        }

        [SetUp]
        public void Setup()
        {
            _commentService = new Mock<ICommentService>();
            _commentTreeBuilder = new CommentTreeBuilder(_commentService.Object);
            CreateTestComments();
        }

        private void CreateTestComments()
        {
            _comments = new List<Comment>();
            for (var x = 0; x < 10; x++)
            {
                var comment = new Comment();
                comment.Id = GuidUtil.NewSequentialId();
                comment.VoteUpCount = x % 10;
                comment.VoteDownCount = x % 5;
                comment.DateCreated = Common.CurrentTime();
                _comments.Add(comment);
                for (var y = 0; y < 10; y++)
                {
                    var child = new Comment();
                    child.Id = GuidUtil.NewSequentialId();
                    child.VoteUpCount = y % 10;
                    child.VoteDownCount = y % 5;
                    child.ParentId = comment.Id;
                    child.DateCreated = Common.CurrentTime();
                    _comments.Add(child);

                    for (var z = 0; z < 10; z++)
                    {
                        var grandChild = new Comment();
                        grandChild.Id = GuidUtil.NewSequentialId();
                        grandChild.VoteUpCount = z % 10;
                        grandChild.VoteDownCount = z % 5;
                        grandChild.ParentId = child.Id;
                        grandChild.DateCreated = Common.CurrentTime();
                        _comments.Add(grandChild);
                    }
                }
            }

            _comments = _comments.OrderByDescending(x => Sorting.Confidence(x.VoteUpCount, x.VoteDownCount)).ThenByDescending(x => x.DateCreated).ToList();

            _commentService.Setup(x => x.GetAllCommentsForPost(It.IsAny<string>(), It.IsAny<CommentSortBy?>())).Returns(_comments);
        }

        public class TestCommentBuilder : CommentBuilder<TestCommentBuilder.TestCommentNode>
        {
            private readonly List<Comment> _comments;

            public TestCommentBuilder(CommentTree commentTree, List<Comment> comments)
                : base(commentTree)
            {
                _comments = comments;
            }

            protected override List<ICommentTreeNode<TestCommentNode>> BuildNode(List<Guid> commentIds)
            {
                return commentIds.Select(commentId => (ICommentTreeNode<TestCommentNode>)new TestCommentNode(_comments.Single(comment => comment.Id == commentId))).ToList();
            }

            public class TestCommentNode : ICommentTreeNode<TestCommentNode>
            {
                public TestCommentNode(Comment comment)
                {
                    Comment = comment;
                }

                public Guid Id { get { return Comment.Id; } }

                public Comment Comment { get; set; }

                public List<ICommentTreeNode<TestCommentNode>> Children { get; set; }
            }
        }
    }
}
