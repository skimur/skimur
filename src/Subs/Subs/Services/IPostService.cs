namespace Subs.Services
{
    public interface IPostService
    {
        void InsertPost(Post post);

        void UpdatePost(Post post);

        Post GetPostBySlug(string slug);
    }
}
