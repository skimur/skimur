using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Skimur.App;
using Skimur.App.ReadModel;

namespace Skimur.Web.ViewModels
{
    public class CreatePostModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        // TODO: [AllowHtml]
        public string Content { get; set; }

        public PostType PostType { get; set; }

        [Display(Name = "Sub name")]
        public string PostToSub { get; set; }

        [Display(Name = "Notify replies")]
        public bool NotifyReplies { get; set; }

        public SubWrapped Sub { get; set; }
    }

    public class EditPostModel
    {
        public Guid PostId { get; set; }

        // TODO:[AllowHtml]
        public string Content { get; set; }
    }

    public class PostDetailsModel
    {
        public PostWrapped Post { get; set; }

        public SubWrapped Sub { get; set; }

        public CommentListModel Comments { get; set; }

        public bool ViewingSpecificComment { get; set; }
    }
}
