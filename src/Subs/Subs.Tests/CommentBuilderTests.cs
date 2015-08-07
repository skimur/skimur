using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;
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
        private Mock<ICommentDao> _commentDao;
        private Mock<IMembershipService> _membershipService;
        private Mock<ISubDao> _subDao;
        private Mock<IPermissionDao> _permissionDao;
        private Mock<IVoteDao> _voteDao;
        private Mock<IPostDao> _postDao;
        private ICommentTreeBuilder _commentTreeBuilder;
        private ICommentTreeContextBuilder _commentTreeContextBuilder;
        private ICommentNodeHierarchyBuilder _commentNodeHierarchyBuilder;

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
            var result = _commentTreeContextBuilder.Build(tree, null, new List<Guid> { comment1, comment2 });

            // assert
            Assert.That(result.TopLevelComments, Has.Count.EqualTo(2));
            CollectionAssert.Contains(result.TopLevelComments, comment1);
            CollectionAssert.Contains(result.TopLevelComments, comment2);
        }

        [Test]
        public void Can_get_children_for_a_specific_comment()
        {
            // arrange
            CreateTreeComments();
            var tree = _commentTreeBuilder.GetCommentTree(null);

            // act
            var result = _commentTreeContextBuilder.Build(tree, null, comment: tree.Tree[Guid.Empty][0], maxDepth: 1);

            // assert
            Assert.That(result.TopLevelComments, Has.Count.EqualTo(1));
            foreach (var comment in result.Comments)
            {
                if (comment != tree.Tree[Guid.Empty][0])
                {
                    Assert.That(tree.Parents[comment], Is.EqualTo(tree.Tree[Guid.Empty][0]));
                }
            }
        }

        [Test]
        public void Can_build_node_sorted()
        {
            // arrange
            var sub = new Sub();
            sub.Name = "test";
            var author = new User();
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
                comment.SubName = sub.Name;
                comment.AuthorUserName = author.UserName;
            }
            SetupComments(comments);
            _membershipService.Setup(x => x.GetUsersByUserNames(It.IsAny<List<string>>())).Returns(new List<User> {author});
            _subDao.Setup(x => x.GetSubByNames(It.IsAny<List<string>>())).Returns(new List<Sub> {sub});

            // act
            var tree = _commentTreeBuilder.GetCommentTree(null);
            var treeContext = _commentTreeContextBuilder.Build(tree,
                comments.ToDictionary(x => x.Id, x => (double) x.SortConfidence));
            var nodes = _commentNodeHierarchyBuilder.Build(tree, treeContext, null);

            //assert
            Assert.That(nodes, Has.Count.EqualTo(2));
            Assert.That(nodes[0].Comment.SortConfidence, Is.EqualTo(10));
            Assert.That(nodes[0].Children, Has.Count.EqualTo(2));
            Assert.That(nodes[0].Children[0].Comment.SortConfidence, Is.EqualTo(5));
            Assert.That(nodes[0].Children[1].Comment.SortConfidence, Is.EqualTo(4));
            Assert.That(nodes[0].Children[1].Children, Has.Count.EqualTo(2));
            Assert.That(nodes[0].Children[1].Children[0].Comment.SortConfidence, Is.EqualTo(20));
            Assert.That(nodes[0].Children[1].Children[1].Comment.SortConfidence, Is.EqualTo(2));
            Assert.That(nodes[1].Comment.SortConfidence, Is.EqualTo(9));
            Assert.That(nodes[1].Children, Has.Count.EqualTo(2));
            Assert.That(nodes[1].Children[0].Comment.SortConfidence, Is.EqualTo(12));
            Assert.That(nodes[1].Children[1].Comment.SortConfidence, Is.EqualTo(3));
            Assert.That(nodes[1].Children[1].Children, Has.Count.EqualTo(1));
            Assert.That(nodes[1].Children[1].Children[0].Comment.SortConfidence, Is.EqualTo(1));
        }

        [SetUp]
        public void Setup()
        {
            _commentService = new Mock<ICommentService>();
            _commentDao = new Mock<ICommentDao>();
            _membershipService = new Mock<IMembershipService>();
            _subDao = new Mock<ISubDao>();
            _permissionDao = new Mock<IPermissionDao>();
            _voteDao = new Mock<IVoteDao>();
            _commentTreeBuilder = new CommentTreeBuilder(_commentService.Object);
            _commentTreeContextBuilder = new CommentTreeContextBuilder();
            _commentNodeHierarchyBuilder = new CommentNodeHierarchyBuilder(_commentDao.Object, _membershipService.Object, _subDao.Object, _permissionDao.Object, _voteDao.Object, _postDao.Object);
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
