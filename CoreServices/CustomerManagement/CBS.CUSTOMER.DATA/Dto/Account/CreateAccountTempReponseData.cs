using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Dto
{
    public class CreateAccountTempReponseData
    {
        public string? Id { get; set; }
        public string? AccountNumber { get; set; }
        public object? Balance { get; set; }
        public string? Status { get; set; }
        public string? ProductId { get; set; }
        public object? Product { get; set; }
        public string? UserId { get; set; }
        public object? Deposits { get; set; }
    }
}
