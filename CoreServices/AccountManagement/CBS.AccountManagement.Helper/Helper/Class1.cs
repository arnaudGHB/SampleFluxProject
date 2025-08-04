using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Helper
{
    public class QueryParameters
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Filters { get; set; }

        public QueryParameters(int pageNumber, int pageSize, string filters)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            Filters = filters;
        }

        public override string ToString()
        {
            return $"PageNumber={PageNumber}&PageSize={PageSize}&Filters={Filters}";
        }
    }
}
