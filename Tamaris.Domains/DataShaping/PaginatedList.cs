using System;
using System.Collections.Generic;

namespace Tamaris.Domains.DataShaping
{
    public class PaginatedList<T>
    {
        public int CurrentPage { get; private set; }
        public int From { get; private set; }
        public List<T> Items { get; private set; }
        public int PageSize { get; private set; }
        public int To { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            PageSize = pageSize;
            From = ((currentPage - 1) * pageSize) + 1;
            To = (From + pageSize) - 1;

            Items = items;
        }

        public bool HasPreviousPage => (CurrentPage > 1);

        public bool HasNextPage => (CurrentPage < TotalPages);
    }
}
