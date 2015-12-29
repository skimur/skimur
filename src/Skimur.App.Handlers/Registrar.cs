using Skimur;
using Skimur.Messaging.Handling;
using Subs.Commands;
using Subs.Events;
using Subs.Worker.Commands;
using Subs.Worker.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Subs.Worker
{
    public class Registrar : IRegistrar
    {
        public void Register(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICommandHandlerResponse<CreateSub, CreateSubResponse>, SubHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<EditSub, EditSubResponse>, SubHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<SubcribeToSub, SubcribeToSubResponse>, SubHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<UnSubcribeToSub, UnSubcribeToSubResponse>, SubHandler>();
            
            serviceCollection.AddTransient<ICommandHandler<CastVoteForPost>, VoteCommandHandler>();
            serviceCollection.AddTransient<ICommandHandler<CastVoteForComment>, VoteCommandHandler>();
            
            serviceCollection.AddTransient<ICommandHandlerResponse<CreateComment, CreateCommentResponse>, CommentCommandHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<EditComment, EditCommentResponse>, CommentCommandHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<DeleteComment, DeleteCommentResponse>, CommentCommandHandler>();

            serviceCollection.AddTransient<ICommandHandlerResponse<BanUserFromSub, BanUserFromSubResponse>, SubBanning>();
            serviceCollection.AddTransient<ICommandHandlerResponse<UnBanUserFromSub, UnBanUserFromSubResponse>, SubBanning>();
            serviceCollection.AddTransient<ICommandHandlerResponse<UpdateUserSubBan, UpdateUserSubBanResponse>, SubBanning>();

            serviceCollection.AddTransient<ICommandHandlerResponse<ApprovePost, ApprovePostResponse>, PostModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<RemovePost, RemovePostResponse>, PostModerationHandler>();

            serviceCollection.AddTransient<ICommandHandlerResponse<SendMessage, SendMessageResponse>, MessagesHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<ReplyMessage, ReplyMessageResponse>, MessagesHandler>();
            serviceCollection.AddTransient<ICommandHandler<MarkMessagesAsRead>, MessagesHandler>();
            serviceCollection.AddTransient<ICommandHandler<MarkMessagesAsUnread>, MessagesHandler>();

            serviceCollection.AddTransient<ICommandHandler<ReportComment>, ReportHandler>();
            serviceCollection.AddTransient<ICommandHandler<ReportPost>, ReportHandler>();
            serviceCollection.AddTransient<ICommandHandler<ConfigureReportIgnoring>, ReportHandler>();
            serviceCollection.AddTransient<ICommandHandler<ClearReports>, ReportHandler>();

            serviceCollection.AddTransient<ICommandHandlerResponse<RemoveModFromSub, RemoveModFromSubResponse>, ModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<ChangeModPermissionsForSub, ChangeModPermissionsForSubResponse>, ModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<AddUserModToSub, AddUserModToSubResponse>, ModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<InviteModToSub, InviteModToSubResponse>, ModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<AcceptModInvitation, AcceptModInvitationResponse>, ModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<RemoveModInviteFromSub, RemoveModInviteFromSubResponse>, ModerationHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<ChangeModInvitePermissions, ChangeModInvitePermissionsResponse>, ModerationHandler>();

            serviceCollection.AddTransient<ICommandHandlerResponse<CreatePost, CreatePostResponse>, PostHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<EditPostContent, EditPostContentResponse>, PostHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<DeletePost, DeletePostResponse>, PostHandler>();
            serviceCollection.AddTransient<ICommandHandler<TogglePostNsfw>, PostHandler>();
            serviceCollection.AddTransient<ICommandHandlerResponse<ToggleSticky, ToggleStickyResponse>, PostHandler>();

            serviceCollection.AddTransient<ICommandHandlerResponse<EditSubStylesCommand, EditSubStylesCommandResponse>, StylesHandler>();

            serviceCollection.AddTransient<ICommandHandler<GenerateThumbnailForPost>, ThumbnailHandler>();

            serviceCollection.AddTransient<ICommandHandler<GenerateEmbeddedMediaObject>, EmbededHandler>();

            serviceCollection.AddTransient<KudosUpdateEventHandler>();
            serviceCollection.AddTransient<ScoringAndSortingEventHandler>();
            serviceCollection.AddTransient<ReplyNotificationEventHandler>();
            serviceCollection.AddTransient<UserMentionNotificationHandler>();
            serviceCollection.AddTransient<SubEventHandler>();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}
