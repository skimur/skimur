using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skimur;
using Subs.ReadModel;

namespace Subs.Services
{
    public interface ICommentService
    {
        Comment GetCommentById(Guid id);

        void InsertComment(Comment comment);

        void UpdateCommentBody(Guid commentId, string body, string bodyFormatted, DateTime dateEdited);

        List<Comment> GetAllCommentsForPost(Guid postId, CommentSortBy? sortBy = null);

        List<Comment> GetChildrenForComment(Guid commentId, Guid? authorName = null);

        void UpdateCommentVotes(Guid commentId, int? upVotes, int? downVotes);

        void UpdateCommentSorting(Guid commentId, double? confidence, double? qa);

        void DeleteComment(Guid commentId, string body);

        void UpdateNumberOfReportsForComment(Guid commentId, int numberOfReports);

        void SetIgnoreReportsForComment(Guid commentId, bool ignoreReports);

        SeekedList<Guid> GetReportedComments(List<Guid> subs = null, int? skip = null, int? take = null);

        int GetNumberOfCommentsForPost(Guid postId);

        SeekedList<Guid> GetCommentsForUser(Guid userId, CommentSortBy? sortBy = null, int? skip = null, int? take = null);
    }
}
