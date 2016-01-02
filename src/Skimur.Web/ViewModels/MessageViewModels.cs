using System;
using System.Collections.Generic;
using Skimur.App;
using Skimur.App.ReadModel;

namespace Skimur.Web.ViewModels
{
    public class MessageThreadViewModel
    {
        public bool IsModerator { get; set; }

        public MessageThreadViewModel()
        {
            Messages = new List<MessageWrapped>();
        }

        public MessageWrapped FirstMessage { get; set; }

        public MessageWrapped ContextMessage { get; set; }

        public List<MessageWrapped> Messages { get; set; }
    }

    public class ReplyMessageViewModel
    {
        public Guid ReplyToMessage { get; set; }

        public string Body { get; set; }
    }

    public class ComposeMessageViewModel
    {
        public bool IsModerator { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }
    }

    public class InboxViewModel
    {
        public InboxType InboxType { get; set; }

        public PagedList<MessageWrapped> Messages { get; set; }

        public List<Guid> ModeratorMailForSubs { get; set; }

        public Sub Sub { get; set; }

        public bool IsModerator { get; set; }
    }

    public enum InboxType
    {
        All,
        Unread,
        Messages,
        CommentReplies,
        PostReplies,
        Mentions,
        Sent,
        ModeratorMail,
        ModeratorMailUnread,
        ModeratorMailSent
    }
}
