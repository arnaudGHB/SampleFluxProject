using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Helper.Helper.Pagging
{
    /// <summary>
    /// Paginated list utility for handling paginated data.
    /// </summary>
    /// <typeparam name="T">Type of the items in the paginated list.</typeparam>
    public class PaginatedList<T>
    {
        /// <summary>
        /// The items in the current page.
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// The total count of all items available.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// The current page number.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of pages available.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        /// <summary>
        /// Indicates if there is a previous page.
        /// </summary>
        public bool HasPreviousPage => PageNumber > 1;

        /// <summary>
        /// Indicates if there is a next page.
        /// </summary>
        public bool HasNextPage => PageNumber < TotalPages;

        /// <summary>
        /// Constructor for creating a paginated list.
        /// </summary>
        /// <param name="items">The items in the current page.</param>
        /// <param name="totalCount">The total count of all items available.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        public PaginatedList(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Creates a paginated list asynchronously from a data source.
        /// </summary>
        /// <param name="source">The IQueryable data source (e.g., database query).</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="cancellationToken">The cancellation token to signal task cancellation.</param>
        /// <returns>A task representing the asynchronous operation, with the paginated list as a result.</returns>
        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await source.CountAsync(cancellationToken);
            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PaginatedList<T>(items, totalCount, pageNumber, pageSize);
        }

    }
}
