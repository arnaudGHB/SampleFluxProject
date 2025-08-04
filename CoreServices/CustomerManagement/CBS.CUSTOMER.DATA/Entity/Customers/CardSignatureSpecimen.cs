using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class CardSignatureSpecimen:BaseEntity
    {
        [Key]
        public string? Id { get; set; }
        public string? CustomerId { get; set; }
        public string? AccountNumber { get; set; }
        public string? BranchId { get; set; }
        public string? BranchMangerId { get; set; }
        public string? IHereByTestifyforAllTheSignatures { get; set; }
        public virtual List<CardSignatureSpecimenDetail>? CardSignatureSpecimenDetails { get; set; }

    }
}
