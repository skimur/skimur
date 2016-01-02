using System;
using System.Collections.Generic;

namespace Skimur.App.Services
{
    public interface IMessageService
    {
        void InsertMessage(Message message);

        int GetNumberOfUnreadMessagesForUser(Guid userId);

        List<Message> GetMessagesByIds(List<Guid> ids);

        Message GetMessageById(Guid id);

        SeekedList<Guid> GetAllMessagesForUser(Guid userId, int? skip = null, int? take = null);

        SeekedList<Guid> GetUnreadMessagesForUser(Guid userId, int? skip = null, int? take = null);

        SeekedList<Guid> GetPrivateMessagesForUser(Guid userId, int? skip = null, int? take = null);
            
        SeekedList<Guid> GetCommentRepliesForUser(Guid userId, int? skip = null, int? take = null);

        SeekedList<Guid> GetPostRepliesForUser(Guid userId, int? skip = null, int? take = null);

        SeekedList<Guid> GetMentionsForUser(Guid userId, int? skip = null, int? take = null);

        SeekedList<Guid> GetSentMessagesForUser(Guid userId, int? skip = null, int? take = null);

        List<Guid> GetMessagesForThread(Guid messageId);

        SeekedList<Guid> GetModeratorMailForSubs(List<Guid> subs, int? skip = null, int? take = null);

        SeekedList<Guid> GetSentModeratorMailForSubs(List<Guid> subs, int? skip = null, int? take = null);

        SeekedList<Guid> GetUnreadModeratorMailForSubs(List<Guid> subs, int? skip = null, int? take = null);

        void MarkMessagesAsRead(List<Guid> messages);

        void MarkMessagesAsUnread(List<Guid> messages);

        void DeleteNotificationsForComment(Guid commentId);

        void InsertMention(Guid userId, Guid authorUserId, Guid? postId, Guid? commentId);

        void DeleteMention(Guid userId, Guid? postId, Guid? commentId);
    }
}
