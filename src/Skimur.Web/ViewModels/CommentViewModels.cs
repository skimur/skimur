using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App.ReadModel;

namespace Skimur.Web.ViewModels
{
    public class CreateCommentModel
    {
        public CreateCommentModel()
        {
            SendReplies = true;
        }

        public Guid PostId { get; set; }

        public Guid? ParentId { get; set; }

        // TODO: [AllowHtml]
        public string Body { get; set; }

        public bool SendReplies { get; set; }
    }

    public class EditCommentModel
    {
        public Guid CommentId { get; set; }

        // TODO: [AllowHtml]
        public string Body { get; set; }
    }

    public class CommentListModel
    {
        public CommentSortBy SortBy { get; set; }

        public List<ICommentNode> CommentNodes { get; set; }
    }
}
