namespace Skimur.App.ReadModel
{
    public class MessageWrapped
    {
        public MessageWrapped(Message message)
        {
            Message = message;
        }

        public Message Message { get; private set; }
        
        public User Author { get; set; }

        public Sub FromSub { get; set; }

        public User ToUser { get; set; }

        public Sub ToSub { get; set; }
        
        public bool CanReply { get; set; }

        public bool UserIsRecipiant { get; set; }

        public bool UserIsSender { get; set; }
        
        public bool CanMarkRead { get; set; }

        public bool? IsUnread { get; set; }

        public CommentWrapped Comment { get; set; }

        public PostWrapped Post { get; set; }
    }
}
