using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.Resources
{
    public abstract class ResourceParameter
    {
        public ResourceParameter(string orderBy)
        {
            this.OrderBy = orderBy;
        }
        const int MaxPageSize = 100;
        public int Skip { get; set; } = 0;

        private int _PageSize = 10;
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {

                _PageSize = (value > MaxPageSize) ? MaxPageSize : value;
            }
        }
        public string? SearchQuery { get; set; }
        public string OrderBy { get; set; }

    }

}
