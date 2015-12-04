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
using Microsoft.Extensions.DependencyInjection;

namespace Subs
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IModeratorInviteWrapper, ModeratorInviteWrapper>();
            serviceCollection.AddSingleton<IModerationInviteService, ModerationInviteService>();
            serviceCollection.AddSingleton<IModerationInviteDao, ModerationInviteDao>();
            serviceCollection.AddSingleton<IKarmaService, KarmaService>();
            serviceCollection.AddSingleton<IKarmaDao, KarmaDao>();
            serviceCollection.AddSingleton<IModerationService, ModerationService>();
            serviceCollection.AddSingleton<IModerationDao, ModerationDao>();
            serviceCollection.AddSingleton<IReportService, ReportService>();
            serviceCollection.AddSingleton<IReportDao, ReportDao>();
            serviceCollection.AddSingleton<ISubActivityService, SubActivityService>();
            serviceCollection.AddSingleton<ISubActivityDao, SubActivityDao>();
            serviceCollection.AddSingleton<ISubUserBanService, SubUserBanService>();
            serviceCollection.AddSingleton<ISubUserBanDao, SubUserBanDao>();
            serviceCollection.AddSingleton<ISubService, SubService>();
            serviceCollection.AddSingleton<ISubDao, SubDao>();
            serviceCollection.AddSingleton<IPostService, PostService>();
            serviceCollection.AddSingleton<IPostDao, PostDao>();
            serviceCollection.AddSingleton<IVoteService, VoteService>();
            serviceCollection.AddSingleton<IVoteDao, VoteDao>();
            serviceCollection.AddSingleton<ICommentService, CommentService>();
            serviceCollection.AddSingleton<ICommentDao, CommentDao>();
            serviceCollection.AddSingleton<IPermissionService, PermissionService>();
            serviceCollection.AddSingleton<IPermissionDao, PermissionDao>();
            serviceCollection.AddSingleton<IMessageService, MessageService>();
            serviceCollection.AddSingleton<IMessageDao, MessageDao>();
            serviceCollection.AddSingleton<ICommentTreeBuilder, CommentTreeBuilder>();
            serviceCollection.AddSingleton<ICommentTreeContextBuilder, CommentTreeContextBuilder>();
            serviceCollection.AddSingleton<ICommentNodeHierarchyBuilder, CommentNodeHierarchyBuilder>();
            serviceCollection.AddSingleton<ICommentWrapper, CommentWrapper>();
            serviceCollection.AddSingleton<IPostWrapper, PostWrapper>();
            serviceCollection.AddSingleton<ISubWrapper, SubWrapper>();
            serviceCollection.AddSingleton<ISubUserBanWrapper, SubUserBanWrapper>();
            serviceCollection.AddSingleton<IMessageWrapper, MessageWrapper>();
            serviceCollection.AddSingleton<IModeratorWrapper, ModeratorWrapper>();
            serviceCollection.AddSingleton<ISubCssService, SubCssService>();
            serviceCollection.AddSingleton<ISubCssDao, SubCssDao>();
            serviceCollection.AddSingleton<IPostThumbnailService, PostThumbnailService>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
