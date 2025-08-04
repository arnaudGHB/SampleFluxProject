using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Resources
{
    public abstract class ResourceParameter
    {
        public ResourceParameter(string order)
        {
            OrderBy = order;
        }
        const int MaxPageSize = 100;
        public int Skip { get; set; } = 0;

        private int _PageSize = 200;
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
    public abstract class GroupResourceParameter
    {

        const int MaxPageSize = 100;
        public int Skip { get; set; } = 0;

        private int _PageSize = 200;
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
