using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA
{
    public class GetCardSignatureSpecimen
    {
        public string? Id { get; set; }
        public string? CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? BranchId { get; set; }
        public string? BranchMangerId { get; set; }
        public string? IHereByTestifyforAllTheSignatures { get; set; }


    }
}
