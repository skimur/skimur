using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs
{
    public class Message
    {
        public Guid Id { get; set; }
        
        public DateTime DateCreated { get; set; }

        public Guid? ParentId { get; set; }

        public Guid? FirstMessage { get; set; }

        public bool Deleted { get; set; }

        public Guid AuthorId { get; set; }

        public string AuthorIp { get; set; }

        public bool IsNew { get; set; }

        public Guid? ToUser { get; set; }

        public Guid? ToSub { get; set; }

        public Guid? FromSub { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }
    }
}
