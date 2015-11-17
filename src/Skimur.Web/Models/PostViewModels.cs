using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class CreatePostModel
    {
        public string Title { get; set; }
        
        public string Url { get; set; }
        
        [AllowHtml]
        public string Content { get; set; }

        public PostType PostType { get; set; }

        [DisplayName("Sub name")]
        public string PostToSub { get; set; }

        [DisplayName("Notify replies")]
        public bool NotifyReplies { get; set; }

        public SubWrapped Sub { get; set; }
    }

    public class EditPostModel
    {
        public Guid PostId { get; set; }

        [AllowHtml]
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
