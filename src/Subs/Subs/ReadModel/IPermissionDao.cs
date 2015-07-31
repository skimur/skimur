namespace Subs.ReadModel
{
    public interface IPermissionDao
    {
        bool CanUserDeleteComment(string userName, Comment comment);

        bool CanUserMarkCommentAsSpam(string userName, Comment comment);

        bool CanUserMarkPostAsSpam(string userName, Post post);

        bool CanUserModerateSub(string userName, string subName);
    }
}
