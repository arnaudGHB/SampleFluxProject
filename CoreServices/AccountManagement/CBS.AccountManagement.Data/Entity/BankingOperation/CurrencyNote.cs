using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data.Entity
{
    public class CurrencyNote : BaseEntity
    {
 
        public string Id { get; set; }

        public int Value { get; set; }

 
        [Required]
        public string Denomination { get; set; }

        [Required]
        public string DinominationType { get; set; }

        [Required]
        public string ReferenceId { get; set; }


        public decimal SubTotal { get; set; }


        public decimal Total { get; set; }

        public string ServiceType { get; set; }
    }
}
