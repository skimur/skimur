using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Handling;
using SimpleInjector;
using Skimur;
using Subs.Commands;
using Subs.Events;
using Subs.Worker.Commands;
using Subs.Worker.Events;

namespace Subs.Worker
{
    public class Registrar : IRegistrar
    {
        public void Register(Container container)
        {
            container.Register<ICommandHandlerResponse<CreateSub, CreateSubResponse>, SubHandler>();
            container.Register<ICommandHandlerResponse<EditSub, EditSubResponse>, SubHandler>();
            container.Register<ICommandHandlerResponse<SubcribeToSub, SubcribeToSubResponse>, SubHandler>();
            container.Register<ICommandHandlerResponse<UnSubcribeToSub, UnSubcribeToSubResponse>, SubHandler>();
            
            container.Register<ICommandHandler<CastVoteForPost>, VoteCommandHandler>();
            container.Register<ICommandHandler<CastVoteForComment>, VoteCommandHandler>();
            
            container.Register<ICommandHandlerResponse<CreateComment, CreateCommentResponse>, CommentCommandHandler>();
            container.Register<ICommandHandlerResponse<EditComment, EditCommentResponse>, CommentCommandHandler>();
            container.Register<ICommandHandlerResponse<DeleteComment, DeleteCommentResponse>, CommentCommandHandler>();

            container.Register<ICommandHandlerResponse<BanUserFromSub, BanUserFromSubResponse>, SubBanning>();
            container.Register<ICommandHandlerResponse<UnBanUserFromSub, UnBanUserFromSubResponse>, SubBanning>();
            container.Register<ICommandHandlerResponse<UpdateUserSubBan, UpdateUserSubBanResponse>, SubBanning>();

            container.Register<ICommandHandlerResponse<ApprovePost, ApprovePostResponse>, PostModerationHandler>();
            container.Register<ICommandHandlerResponse<RemovePost, RemovePostResponse>, PostModerationHandler>();

            container.Register<ICommandHandlerResponse<SendMessage, SendMessageResponse>, MessagesHandler>();
            container.Register<ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse>, MessagesHandler>();
            container.Register<ICommandHandler<MarkMessagesAsRead>, MessagesHandler>();
            container.Register<ICommandHandler<MarkMessagesAsUnread>, MessagesHandler>();

            container.Register<ICommandHandler<ReportComment>, ReportHandler>();
            container.Register<ICommandHandler<ReportPost>, ReportHandler>();
            container.Register<ICommandHandler<ConfigureReportIgnoring>, ReportHandler>();
            container.Register<ICommandHandler<ClearReports>, ReportHandler>();

            container.Register<ICommandHandlerResponse<RemoveModFromSub, RemoveModFromSubResponse>, ModerationHandler>();
            container.Register<ICommandHandlerResponse<ChangeModPermissionsForSub, ChangeModPermissionsForSubResponse>, ModerationHandler>();
            container.Register<ICommandHandlerResponse<AddUserModToSub, AddUserModToSubResponse>, ModerationHandler>();
            container.Register<ICommandHandlerResponse<InviteModToSub, InviteModToSubResponse>, ModerationHandler>();
            container.Register<ICommandHandlerResponse<AcceptModInvitation, AcceptModInvitationResponse>, ModerationHandler>();
            container.Register<ICommandHandlerResponse<RemoveModInviteFromSub, RemoveModInviteFromSubResponse>, ModerationHandler>();
            container.Register<ICommandHandlerResponse<ChangeModInvitePermissions, ChangeModInvitePermissionsResponse>, ModerationHandler>();

            container.Register<ICommandHandlerResponse<CreatePost, CreatePostResponse>, PostHandler>();
            container.Register<ICommandHandlerResponse<EditPostContent, EditPostContentResponse>, PostHandler>();
            container.Register<ICommandHandlerResponse<DeletePost, DeletePostResponse>, PostHandler>();
            container.Register<ICommandHandler<TogglePostNsfw>, PostHandler>();
            
            container.Register<ICommandHandlerResponse<EditSubStylesCommand, EditSubStylesCommandResponse>, StylesHandler>();

            container.Register<KudosUpdateEventHandler>();
            container.Register<ScoringAndSortingEventHandler>();
            container.Register<ReplyNotificationEventHandler>();
            container.Register<UserMentionNotificationHandler>();
            container.Register<SubEventHandler>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
