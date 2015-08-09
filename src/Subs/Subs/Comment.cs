using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using ServiceStack.DataAnnotations;

namespace Subs
{
    [Alias("Comments")]
    public class Comment : IAggregateRoot
    {
        /// <summary>
        /// The id of this message
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The date this message was created
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The date this message was edited, if at all
        /// </summary>
        public DateTime? DateEdited { get; set; }

        /// <summary>
        /// The sub this comment is located in
        /// </summary>
        public Guid SubId { get; set; }

        /// <summary>
        /// If this message is a response to another message, this will point to that message
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// Empty for top level comments, but for children, contains all the parent comments.
        /// This will make tree querying easier.
        /// </summary>
        public Array Parents { get; set; }

        /// <summary>
        /// The user name of who authored this comment.
        /// </summary>
        public Guid AuthorUserId { get; set; }

        /// <summary>
        /// The ip address of the author when posting the comment.
        /// </summary>
        public string AuthorIpAddress { get; set; }

        /// <summary>
        /// The slug of the post that this message was a comment to.
        /// </summary>
        public Guid PostId { get; set; }

        /// <summary>
        /// Gets the body of the comment. This could be markdown content.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets the body of the comment, rendered via markdown for display.
        /// </summary>
        public string BodyFormatted { get; set; }

        /// <summary>
        /// Do we want to notify the author of any replies?
        /// </summary>
        public bool SendReplies { get; set; }

        /// <summary>
        /// The number of up votes this message has.
        /// This should only have valid values if it is a comment to a post.
        /// </summary>
        public int VoteUpCount { get; set; }

        /// <summary>
        /// The number of down votes this message has.
        /// This should only have valid values if it is a comment to a post.
        /// </summary>
        public int VoteDownCount { get; set; }

        /// <summary>
        /// Is this comment deleted? If so, AuthorUserName, Body and BodyFormatted should be marked as "deleted".
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Value used to sort the best comments
        /// </summary>
        public decimal SortConfidence { get; set; }

        /// <summary>
        /// Values used to sort the top comments for a QA-style thread.
        /// </summary>
        public decimal SortQa { get; set; }
    }
}
