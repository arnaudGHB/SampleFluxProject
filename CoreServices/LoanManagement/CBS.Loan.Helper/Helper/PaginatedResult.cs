using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Helper.Helper
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } // List of items for the current page
        public int PageNumber { get; set; } // Current page number
        public int PageSize { get; set; } // Number of items per page
        public int TotalItems { get; set; } // Total number of items across all pages
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize); // Calculate total pages

        public PaginatedResult(List<T> items, int pageNumber, int pageSize, int totalItems)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalItems = totalItems;
        }

        public static PaginatedResult<T> Create(List<T> items, int pageNumber, int pageSize, int totalItems)
        {
            return new PaginatedResult<T>(items, pageNumber, pageSize, totalItems);
        }
    }

}
