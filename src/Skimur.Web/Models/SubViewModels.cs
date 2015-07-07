using System;

namespace Skimur.Web.Models
{
    public class CreateEditSubModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string SidebarText { get; set; }

        public bool IsEditing { get; set; }
    }

    public class SubModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSubscribed { get; set; }
    }

    public class SubPosts
    {
        public SubModel Sub { get; set; }
    }
}
