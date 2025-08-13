using CBS.TransactionManagement.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data
{
    public class CurrencyNotes:BaseEntity
    {
        public string Id { get; set; }
        public string DinominationType { get; set; }
        public string Denomination { get; set; }
        public int Value { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public string ReferenceId { get; set; }
        public string? ServiceType { get; set; }
    }
}
