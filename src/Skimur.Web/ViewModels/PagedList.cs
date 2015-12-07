using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skimur.Web.ViewModels
{
    public class PagedList<T> : List<T>
    {
        public PagedList(SeekedList<T> list, int pageNumber, int pageSize)
            : base(list)
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
}
