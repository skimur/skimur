using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs.ReadModel
{
    public interface IPostDao
    {
        List<Post> GetPosts(List<string> subs = null, PostsSortBy sortBy = PostsSortBy.Hot);
    }

    public enum PostsSortBy
    {
        Hot,
        New,
        Rising,
        Controversial,
        Top
    }
}
