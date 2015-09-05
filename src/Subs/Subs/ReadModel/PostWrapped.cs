using Membership;

namespace Subs.ReadModel
{
    public class PostWrapped
    {
        public PostWrapped(Post post)
        {
            Post = post;
        }

        public Post Post { get; private set; }

        public Sub Sub { get; set; }

        public User Author { get; set; }
        
        public VoteType? CurrentUserVote;
        
        public Verdict? Verdict { get; set; }

        public bool CanManagePost { get; set; }
    }
}
