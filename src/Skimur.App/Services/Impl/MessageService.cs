using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack.OrmLite;
using Skimur.Data;
using Skimur.Utils;

namespace Skimur.App.Services.Impl
{
    public class MessageService : IMessageService
    {
        private readonly IDbConnectionProvider _conn;

        public MessageService(IDbConnectionProvider conn)
        {
            _conn = conn;
        }

        public void InsertMessage(Message message)
        {
            _conn.Perform(conn =>
            {
                conn.Insert(message);
            });
        }

        public int GetNumberOfUnreadMessagesForUser(Guid userId)
        {
            return _conn.Perform(conn =>
            {
                return (int)conn.Count<Message>(x => x.ToUser == userId && x.IsNew);
            });
        }

        public List<Message> GetMessagesByIds(List<Guid> ids)
        {
            return _conn.Perform(conn => conn.SelectByIds<Message>(ids));
        }

        public Message GetMessageById(Guid id)
        {
            return _conn.Perform(conn => conn.SingleById<Message>(id));
        }

        public SeekedList<Guid> GetAllMessagesForUser(Guid userId, int? skip = null, int? take = null)
        {
            return QueryMessagesForUser(userId, query => { }, skip, take);
        }

        public SeekedList<Guid> GetUnreadMessagesForUser(Guid userId, int? skip = null, int? take = null)
        {
            return QueryMessagesForUser(userId, query => query.Where(x => x.IsNew), skip, take);
        }

        public SeekedList<Guid> GetPrivateMessagesForUser(Guid userId, int? skip = null, int? take = null)
        {
            return QueryMessagesForUser(userId, query => query.Where(x => x.Type == (int)MessageType.Private), skip, take);
        }

        public SeekedList<Guid> GetCommentRepliesForUser(Guid userId, int? skip = null, int? take = null)
        {
            return QueryMessagesForUser(userId, query => query.Where(x => x.Type == (int)MessageType.CommentReply), skip, take);
        }

        public SeekedList<Guid> GetPostRepliesForUser(Guid userId, int? skip = null, int? take = null)
        {
            return QueryMessagesForUser(userId, query => query.Where(x => x.Type == (int)MessageType.PostReply), skip, take);
        }

        public SeekedList<Guid> GetMentionsForUser(Guid userId, int? skip = null, int? take = null)
        {
            return QueryMessagesForUser(userId, query => query.Where(x => x.Type == (int)MessageType.Mention), skip, take);
        }

        public SeekedList<Guid> GetSentMessagesForUser(Guid userId, int? skip = null, int? take = null)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Message>().Where(x => x.AuthorId == userId
                    // if the user sent this message as a moderator, hide it.
                    // it will be visible in the moderator mail
                    && x.FromSub == null);
                var totalCount = conn.Count(query);
                query.Skip(skip).Take(take);
                query.SelectExpression = "SELECT \"id\"";
                query.OrderByDescending(x => x.DateCreated);
                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public List<Guid> GetMessagesForThread(Guid messageId)
        {
            return _conn.Perform(conn =>
            {
                var query = conn.From<Message>();
                query.Where(x => x.FirstMessage == messageId || x.Id == messageId);
                query.SelectExpression = "SELECT \"id\"";
                return conn.Select(query).Select(x => x.Id).ToList();
            });
        }

        public SeekedList<Guid> GetModeratorMailForSubs(List<Guid> subs, int? skip = null, int? take = null)
        {
            if (subs == null) throw new ArgumentNullException("subs");

            return _conn.Perform(conn =>
            {
                var query = conn.From<Message>();
                query.Where(x => subs.Contains((Guid)x.ToSub));
                var totalCount = conn.Count(query);
                query.SelectExpression = "SELECT \"id\"";
                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public SeekedList<Guid> GetSentModeratorMailForSubs(List<Guid> subs, int? skip = null, int? take = null)
        {
            if (subs == null) throw new ArgumentNullException("subs");

            return _conn.Perform(conn =>
            {
                var query = conn.From<Message>();
                query.Where(x => subs.Contains((Guid)x.FromSub));
                var totalCount = conn.Count(query);
                query.SelectExpression = "SELECT \"id\"";
                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public SeekedList<Guid> GetUnreadModeratorMailForSubs(List<Guid> subs, int? skip = null, int? take = null)
        {
            if (subs == null) throw new ArgumentNullException("subs");

            return _conn.Perform(conn =>
            {
                var query = conn.From<Message>();
                query.Where(x => subs.Contains((Guid)x.ToSub) && x.IsNew);
                var totalCount = conn.Count(query);
                query.SelectExpression = "SELECT \"id\"";
                return new SeekedList<Guid>(conn.Select(query).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        private SeekedList<Guid> QueryMessagesForUser(Guid userId, Action<SqlExpression<Message>> query, int? skip, int? take)
        {
            return _conn.Perform(conn =>
            {
                var q = conn.From<Message>();
                query(q);
                q.Where(x => x.ToUser == userId
                    // if the user was sent this message as a mod of a sub,
                    // then don't show it. it is visible in moderator mail
                    && x.ToSub == null);
                var totalCount = conn.Count(q);
                q.Skip(skip).Take(take);
                q.SelectExpression = "SELECT \"id\"";
                q.OrderByDescending(x => x.DateCreated);
                return new SeekedList<Guid>(conn.Select(q).Select(x => x.Id), skip ?? 0, take, totalCount);
            });
        }

        public void MarkMessagesAsRead(List<Guid> messages)
        {
            if (messages == null || messages.Count == 0)
                return;

            _conn.Perform(conn =>
            {
                conn.Update<Message>(new { IsNew = false }, x => messages.Contains(x.Id));
            });
        }

        public void MarkMessagesAsUnread(List<Guid> messages)
        {
            if (messages == null || messages.Count == 0)
                return;

            _conn.Perform(conn =>
            {
                conn.Update<Message>(new { IsNew = true }, x => messages.Contains(x.Id));
            });
        }

        public void DeleteNotificationsForComment(Guid commentId)
        {
            _conn.Perform(conn =>
            {
                conn.Delete<Message>(x => x.CommentId == commentId);
            });
        }

        public void InsertMention(Guid userId, Guid authorUserId, Guid? postId, Guid? commentId)
        {
            if (postId.HasValue && commentId.HasValue) throw new Exception("You mast pass either post, or comment, but not both.");
            if (!postId.HasValue && !commentId.HasValue) throw new Exception("You must provide a comment or post id.");

            _conn.Perform(conn =>
            {
                if (postId.HasValue)
                {
                    if (conn.Count<Message>(x => x.ToUser == userId && x.Type == (int)MessageType.Mention && x.PostId == postId) == 0)
                    {
                        conn.Insert(new Message
                        {
                            Id = GuidUtil.NewSequentialId(),
                            DateCreated = Common.CurrentTime(),
                            ToUser = userId,
                            AuthorId = authorUserId,
                            MessageType = MessageType.Mention,
                            PostId = postId,
                            IsNew = true
                        });
                    }
                }

                if (commentId.HasValue)
                {
                    if (conn.Count<Message>(x => x.ToUser == userId && x.Type == (int)MessageType.Mention && x.CommentId == commentId) == 0)
                    {
                        conn.Insert(new Message
                        {
                            Id = GuidUtil.NewSequentialId(),
                            DateCreated = Common.CurrentTime(),
                            ToUser = userId,
                            AuthorId = authorUserId,
                            MessageType = MessageType.Mention,
                            CommentId = commentId,
                            IsNew = true
                        });
                    }
                }
            });
        }

        public void DeleteMention(Guid userId, Guid? postId, Guid? commentId)
        {
            if (postId.HasValue && commentId.HasValue) throw new Exception("You mast pass either post, or comment, but not both.");
            if (!postId.HasValue && !commentId.HasValue) throw new Exception("You must provide a comment or post id.");

            _conn.Perform(conn =>
            {
                if (postId.HasValue)
                {
                    conn.Delete<Message>(x => x.ToUser == userId && x.Type == (int)MessageType.Mention && x.PostId == postId);
                }

                if (commentId.HasValue)
                {
                    conn.Delete<Message>(x => x.ToUser == userId && x.Type == (int)MessageType.Mention && x.CommentId == commentId);
                }
            });
        }
    }
}
