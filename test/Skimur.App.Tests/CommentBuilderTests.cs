using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Skimur;
using Skimur.App;
using Skimur.App.ReadModel;
using Skimur.App.ReadModel.Impl;
using Skimur.App.Services;
using Skimur.App.Services.Impl;
using Skimur.Utils;
using Xunit;

namespace Subs.Tests
{
    public class CommentBuilderTests
    {
        private Mock<ICommentService> _commentService;
        private Mock<ICommentDao> _commentDao;
        private Mock<IMembershipService> _membershipService;
        private Mock<ISubDao> _subDao;
        private Mock<IPermissionDao> _permissionDao;
        private Mock<IVoteDao> _voteDao;
        private Mock<IPostDao> _postDao;
        private Mock<IReportDao> _reportDao;
        private ICommentWrapper _commentWrapper;
        private ICommentTreeBuilder _commentTreeBuilder;
        private ICommentTreeContextBuilder _commentTreeContextBuilder;
        private ICommentNodeHierarchyBuilder _commentNodeHierarchyBuilder;

        public CommentBuilderTests()
        {
            _commentService = new Mock<ICommentService>();
            _commentDao = new Mock<ICommentDao>();
            _membershipService = new Mock<IMembershipService>();
            _subDao = new Mock<ISubDao>();
            _postDao = new Mock<IPostDao>();
            _permissionDao = new Mock<IPermissionDao>();
            _voteDao = new Mock<IVoteDao>();
            _reportDao = new Mock<IReportDao>();
            _commentWrapper = new CommentWrapper(_commentDao.Object, _membershipService.Object, _subDao.Object, _postDao.Object, _permissionDao.Object, _voteDao.Object, _reportDao.Object);
            _commentTreeBuilder = new CommentTreeBuilder(_commentService.Object);
            _commentTreeContextBuilder = new CommentTreeContextBuilder();
            _commentNodeHierarchyBuilder = new CommentNodeHierarchyBuilder(_commentWrapper);
        }

        [Fact]
        public void Can_build_comment_tree()
        {
            // arrange
            var comments = CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(Guid.Empty);
            var sorter = comments.ToDictionary(x => x.Id, x => (double)comments.IndexOf(comments.Single(y => y.Id == x.Id)));

            // act
            var result = _commentTreeContextBuilder.Build(tree, sorter);

            // assert
            result.Comments.Count.Should().Be(1110);
            result.TopLevelComments.Count.Should().Be(10);
        }

        [Fact]
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
            var tree = _commentTreeBuilder.GetCommentTree(Guid.Empty);

            // act
            var result = _commentTreeContextBuilder.Build(tree, new Dictionary<Guid, double>());

            // assert
            result.CommentsChildrenCount[comment.Id].Should().Be(3);
            result.CommentsChildrenCount[commentChild1.Id].Should().Be(0);
            result.CommentsChildrenCount[commentChild2.Id].Should().Be(1);
            result.CommentsChildrenCount[grandChild1.Id].Should().Be(0);
        }

        [Fact]
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
            var tree = _commentTreeBuilder.GetCommentTree(Guid.Empty);

            // act
            var result = _commentTreeContextBuilder.Build(tree, new Dictionary<Guid, double>(), maxDepth: 1);

            // assert
            result.MoreRecursion.Count.Should().Be(1);
            result.MoreRecursion.Contains(comment.Id).Should().BeTrue();

            // act
            result = _commentTreeContextBuilder.Build(tree, new Dictionary<Guid, double>(), maxDepth: 2);

            // assert
            result.MoreRecursion.Count.Should().Be(1);
            result.MoreRecursion.Contains(commentChild2.Id).Should().BeTrue();
        }

        [Fact]
        public void Can_get_specific_children()
        {
            // arrange
            CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(Guid.Empty);
            var comment1 = tree.Tree[Guid.Empty][0];
            var comment2 = tree.Tree[Guid.Empty][2];

            // act
            var result = _commentTreeContextBuilder.Build(tree, null, new List<Guid> { comment1, comment2 });

            // assert
            result.TopLevelComments.Count.Should().Be(2);
            result.TopLevelComments.Contains(comment1).Should().BeTrue();
            result.TopLevelComments.Contains(comment2).Should().BeTrue();
        }

        [Fact]
        public void Can_get_children_for_a_specific_comment()
        {
            // arrange
            CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(Guid.Empty);

            // act
            var result = _commentTreeContextBuilder.Build(tree, null, comment: tree.Tree[Guid.Empty][0], maxDepth: 1);

            // assert
            result.TopLevelComments.Count.Should().Be(1);
            foreach (var comment in result.Comments)
            {
                if (comment != tree.Tree[Guid.Empty][0])
                {
                    tree.Parents[comment].Should().Be(tree.Tree[Guid.Empty][0]);
                }
            }
        }

        [Fact]
        public void Can_build_node_sorted()
        {
            // arrange
            var sub = new Sub();
            sub.Id = GuidUtil.NewSequentialId();
            sub.Name = "test";
            var author = new User();
            author.Id = GuidUtil.NewSequentialId();
            author.UserName = "author";
            var comments = ConvertTestNodesToComments(new List<TestNodeTreeSorted>
            {
                TestNodeTreeSorted.Create(10,
                    TestNodeTreeSorted.Create(5),
                    TestNodeTreeSorted.Create(4,
                        TestNodeTreeSorted.Create(2),
                        TestNodeTreeSorted.Create(20))),
                TestNodeTreeSorted.Create(9,
                    TestNodeTreeSorted.Create(3,
                        TestNodeTreeSorted.Create(1)),
                    TestNodeTreeSorted.Create(12))
            });
            foreach (var comment in comments)
            {
                comment.SubId = sub.Id;
                comment.AuthorUserId = author.Id;
            }
            SetupComments(comments);
            _membershipService.Setup(x => x.GetUsersByIds(It.IsAny<List<Guid>>())).Returns(new List<User> { author });
            _subDao.Setup(x => x.GetSubsByIds(It.IsAny<List<Guid>>())).Returns(new List<Sub> { sub });

            // act
            var tree = _commentTreeBuilder.GetCommentTree(Guid.Empty);
            var treeContext = _commentTreeContextBuilder.Build(tree,
                comments.ToDictionary(x => x.Id, x => (double)x.SortConfidence));
            var nodes = _commentNodeHierarchyBuilder.Build(tree, treeContext, null);

            //assert
            nodes.Count.Should().Be(2);
            CommentExtensions.As<CommentNode>(nodes[0]).Comment.Comment.SortConfidence.Should().Be(10);
            CommentExtensions.As<CommentNode>(nodes[0]).Children.Count.Should().Be(2);
            CommentExtensions.As<CommentNode>(nodes[0].As<CommentNode>().Children[0]).Comment.Comment.SortConfidence.Should().Be(5);
            CommentExtensions.As<CommentNode>(nodes[0].As<CommentNode>().Children[1]).Comment.Comment.SortConfidence.Should().Be(4);
            CommentExtensions.As<CommentNode>(nodes[0]).Children[1].Children.Count.Should().Be(2);
            CommentExtensions.As<CommentNode>(nodes[0].As<CommentNode>().Children[1].Children[0]).Comment.Comment.SortConfidence.Should().Be(20);
            CommentExtensions.As<CommentNode>(nodes[0].As<CommentNode>().Children[1].Children[1]).Comment.Comment.SortConfidence.Should().Be(2);
            CommentExtensions.As<CommentNode>(nodes[1]).Comment.Comment.SortConfidence.Should().Be(9);
            CommentExtensions.As<CommentNode>(nodes[1]).Children.Count.Should().Be(2);
            CommentExtensions.As<CommentNode>(nodes[1].As<CommentNode>().Children[0]).Comment.Comment.SortConfidence.Should().Be(12);
            CommentExtensions.As<CommentNode>(nodes[1].As<CommentNode>().Children[1]).Comment.Comment.SortConfidence.Should().Be(3);
            CommentExtensions.As<CommentNode>(nodes[1]).Children[1].Children.Count.Should().Be(1);
            CommentExtensions.As<CommentNode>(nodes[1].As<CommentNode>().Children[1].Children[0]).Comment.Comment.SortConfidence.Should().Be(1);
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
            _commentService.Setup(x => x.GetAllCommentsForPost(It.IsAny<Guid>(), It.IsAny<CommentSortBy?>())).Returns(comments);
            _commentDao.Setup(x => x.GetCommentsByIds(It.IsAny<List<Guid>>()))
                .Returns(new Func<List<Guid>, List<Comment>>(
                    list =>
                    {
                        return comments.Where(x => list.Contains(x.Id)).ToList();
                    }));
            _commentDao.Setup(x => x.GetCommentById(It.IsAny<Guid>()))
                .Returns(new Func<Guid, Comment>(commentId => comments.Single(x => x.Id == commentId)));

        }

        private List<Comment> ConvertTestNodesToComments(IEnumerable<TestNodeTreeSorted> nodes)
        {
            var comments = new List<Comment>();
            Action<TestNodeTreeSorted, Guid?> add = null;
            add = (node, parentCommentId) =>
            {
                var comment = new Comment();
                comment.Id = Guid.Parse(node.Sort.ToString("00000000") + "-0000-0000-0000-000000000000");
                comment.ParentId = parentCommentId;
                comment.SortConfidence = node.Sort;
                comments.Add(comment);
                foreach (var child in node.Children)
                {
                    add(child, comment.Id);
                }
            };

            foreach (var node in nodes)
            {
                add(node, null);
            }

            return comments;
        }

        public class TestNodeTreeSorted
        {
            public TestNodeTreeSorted(decimal sort)
            {
                Sort = sort;
                Children = new List<TestNodeTreeSorted>();
            }

            public decimal Sort { get; set; }

            public List<TestNodeTreeSorted> Children { get; set; }

            public static TestNodeTreeSorted Create(decimal sort, params TestNodeTreeSorted[] children)
            {
                var result = new TestNodeTreeSorted(sort);
                if (children != null && children.Length > 0)
                    result.Children.AddRange(children);
                return result;
            }
        }
    }
}
