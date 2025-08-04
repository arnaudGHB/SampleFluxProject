using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Pagging
{
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }
    }
    public class DataTableOptions
    {
        public int start { get; set; }
        public string draw { get; set; }
        public int length { get; set; }
        public string sortColumnName { get; set; } // Change type to int?
        public string sortColumnDirection { get; set; }
        public string searchValue { get; set; }
        public int pageSize { get; set; }
        public int skip { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public string search { get; set; }
        public string sortDirection { get; set; }
    }
    public class CustomDataTable
    {
        public int draw { get; set; }
        public long recordsTotal { get; set; }
        public long recordsFiltered { get; set; }
        public object data { get; set; }
        public DataTableOptions DataTableOptions { get; set; }

        public CustomDataTable(int draw, long recordsTotal, long recordsFiltered, object data, DataTableOptions dataTableOptions)
        {
            this.draw = draw;
            this.recordsTotal = recordsTotal;
            this.recordsFiltered = recordsFiltered;
            this.data = data;
            DataTableOptions = dataTableOptions;
        }
    }
}
