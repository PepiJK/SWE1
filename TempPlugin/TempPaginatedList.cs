using System;
using System.Collections.Generic;

namespace TempPlugin
{
    // Based on https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-3.0
    public class TempPaginatedList
    {
        public int TotalPages { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<TempModel> Items { get; set; }
        public int Skip { get; set; }

        public TempPaginatedList(int pageIndex, int pageSize, int count)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            Skip = (pageIndex - 1) * pageSize;
        }
    }
}