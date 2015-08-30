﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Membership;
using Subs;
using Subs.ReadModel;

namespace Skimur.Web.Models
{
    public class PagedList<T> : List<T>
    {
        public PagedList(SeekedList<T> list, int pageNumber, int pageSize)
            :base(list)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            HasPreviousPage = list.HasPrevious;
            HasNextPage = list.HasMore;
        }

        public PagedList(IEnumerable<T> list, int pageNumber, int pageSize, bool hasMore)
            : base(list)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            HasPreviousPage = pageNumber > 1;
            HasNextPage = hasMore;
        }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public bool HasNextPage { get; set; }

        public bool HasPreviousPage { get; set; }

        public static PagedList<T> Build<TSource>(SeekedList<TSource> source, Func<TSource, T> select, int pageNumber, int pageSize)
        {
            return new PagedList<T>(source.Select(select), pageNumber, pageSize, source.HasMore);
        }
    }

    public class SidebarViewModel
    {
        public SubWrapped CurrentSub { get; set; }
        
        public bool IsModerator { get; set; }

        public List<User> Moderators { get; set; }

        public bool ShowSubmit { get; set; }

        public bool ShowCreateSub { get; set; }
    }
}
