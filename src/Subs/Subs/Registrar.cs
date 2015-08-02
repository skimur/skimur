using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;
using Subs.ReadModel;
using Subs.Services;

namespace Subs
{
    public class Registrar : IRegistrar
    {
        public void Register(SimpleInjector.Container container)
        {
            container.RegisterSingle<ISubService, SubService>();
            container.RegisterSingle<ISubDao, SubDao>();
            container.RegisterSingle<IPostService, PostService>();
            container.RegisterSingle<IPostDao, PostDao>();
            container.RegisterSingle<IVoteService, VoteService>();
            container.RegisterSingle<IVoteDao, VoteDao>();
            container.RegisterSingle<ICommentService, CommentService>();
            container.RegisterSingle<ICommentDao, CommentDao>();
            container.RegisterSingle<IPermissionService, PermissionService>();
            container.RegisterSingle<IPermissionDao, PermissionDao>();
            container.RegisterSingle<ICommentTreeBuilder, CommentTreeBuilder>();
            container.RegisterSingle<ICommentTreeContextBuilder, CommentTreeContextBuilder>();
            container.RegisterSingle<ICommentNodeHierarchyBuilder, CommentNodeHierarchyBuilder>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
