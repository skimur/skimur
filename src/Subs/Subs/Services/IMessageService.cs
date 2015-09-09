using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;

namespace Subs.Services
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

        SeekedList<Guid> GetSentMessagesForUser(Guid userId, int? skip, int? take);

        List<Guid> GetMessagesForThread(Guid messageId);

        SeekedList<Guid> GetModeratorMailForSubs(List<Guid> subs, int? skip, int? take);

        SeekedList<Guid> GetUnreadModeratorMailForSubs(List<Guid> subs, int? skip, int? take);
    }
}
