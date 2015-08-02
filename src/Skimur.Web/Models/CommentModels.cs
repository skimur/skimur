using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Subs;
using Subs.ReadModel;
using Subs.Services;

namespace Skimur.Web.Models
{
    public class CreateCommentModel
    {
        public CreateCommentModel()
        {
            SendReplies = true;
        }

        public string PostSlug { get; set; }

        public Guid? ParentId { get; set; }

        public string Body { get; set; }

        public bool SendReplies { get; set; }
    }

    public class EditCommentModel
    {
        public Guid CommentId { get; set; }

        public string Body { get; set; }
    }

    public class CommentModel : Comment
    {
        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateEdited { get; set; }

        public Guid? ParentId { get; set; }

        public string AuthorUserName { get; set; }

        public string Body { get; set; }

        public string BodyFormatted { get; set; }

        public int VoteUpCount { get; set; }

        public int VoteDownCount { get; set; }

        public List<CommentModel> Children { get; set; } 

        public VoteType? CurrentVote { get; set; }

        public bool CanDelete { get; set; }

        public bool CanEdit { get; set; }

        public bool CanMarkSpam { get; set; }
    }

    public class CommentListModel
    {
        public string PostSlug { get; set; }

        public CommentSortBy SortBy { get; set; } 

        public List<CommentNode> Comments { get; set; }
    }
}
