using System;
using System.Collections.Generic;

namespace TempPlugin
{
    /// <summary>
    /// TempPaginatedList class that uses Skip and Take statements to filter data on the server instead of always retrieving all rows of the temperature table.
    /// Code from https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-3.0.
    /// </summary>
    public class TempPaginatedList
    {
        /// <summary>
        /// The number of total pages in the list.
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// The page index where the list is currently at.
        /// </summary>
        public int PageIndex { get; set; }
        
        /// <summary>
        /// The number of elements the list is maximal holding.
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Temperature entities in the list containing each row in the temperature table.
        /// </summary>
        public IEnumerable<TempModel> Items { get; set; }
        
        /// <summary>
        /// The number of elements that are skipped.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// Set the parameters and calculating the total pages and and the number of elements to skip.
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        public TempPaginatedList(int pageIndex, int pageSize, int count)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            Skip = (pageIndex - 1) * pageSize;
        }
    }
}