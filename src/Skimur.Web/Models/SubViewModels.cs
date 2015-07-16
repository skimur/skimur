﻿using System;
using System.Collections.Generic;
using Subs.ReadModel;

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

        public DateTime CreatedDate { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsSubscribed { get; set; }

        public ulong NumberOfSubscribers { get; set; }
    }

    public class SubPosts
    {
        public SubPosts()
        {
            Posts = new List<PostModel>();
        }

        public SubModel Sub { get; set; }

        public List<PostModel> Posts { get; set; }

        public PostsSortBy SortBy { get; set; }

        public TimeFilter? TimeFilter { get; set; }
    }
}
