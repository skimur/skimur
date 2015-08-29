using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;
using Subs.ReadModel;
using Subs.ReadModel.Impl;
using Subs.Services;
using Subs.Services.Impl;

namespace Subs
{
    public class Registrar : IRegistrar
    {
        public void Register(SimpleInjector.Container container)
        {
            container.RegisterSingleton<ISubUserBanService, SubUserBanService>();
            container.RegisterSingleton<ISubUserBanDao, SubUserBanDao>();
            container.RegisterSingleton<ISubService, SubService>();
            container.RegisterSingleton<ISubDao, SubDao>();
            container.RegisterSingleton<IPostService, PostService>();
            container.RegisterSingleton<IPostDao, PostDao>();
            container.RegisterSingleton<IVoteService, VoteService>();
            container.RegisterSingleton<IVoteDao, VoteDao>();
            container.RegisterSingleton<ICommentService, CommentService>();
            container.RegisterSingleton<ICommentDao, CommentDao>();
            container.RegisterSingleton<IPermissionService, PermissionService>();
            container.RegisterSingleton<IPermissionDao, PermissionDao>();
            container.RegisterSingleton<ICommentTreeBuilder, CommentTreeBuilder>();
            container.RegisterSingleton<ICommentTreeContextBuilder, CommentTreeContextBuilder>();
            container.RegisterSingleton<ICommentNodeHierarchyBuilder, CommentNodeHierarchyBuilder>();
            container.RegisterSingleton<ICommentWrapper, CommentWrapper>();
            container.RegisterSingleton<IPostWrapper, PostWrapper>();
            container.RegisterSingleton<ISubWrapper, SubWrapper>();
            container.RegisterSingleton<ISubUserBanWrapper, SubUserBanWrapper>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
