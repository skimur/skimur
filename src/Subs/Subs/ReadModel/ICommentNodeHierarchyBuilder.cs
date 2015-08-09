using System;
using System.Collections.Generic;
using Infrastructure.Membership;
using Subs.Services;

namespace Subs.ReadModel
{
    public interface ICommentNodeHierarchyBuilder
    {
        List<CommentWrapped> Build(CommentTree tree, CommentTreeContext treeContext, User currentUser);
    }
}
