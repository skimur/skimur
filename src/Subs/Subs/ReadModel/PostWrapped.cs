using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;

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
    }
}
