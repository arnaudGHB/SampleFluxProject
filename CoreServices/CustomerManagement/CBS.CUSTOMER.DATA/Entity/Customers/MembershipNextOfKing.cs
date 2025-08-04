using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.DATA.Entity
{
    public class MembershipNextOfKing:BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }
        public string Relation { get; set; }
        public string Ratio { get; set; }
        public string? SignatureUrl { get; set; }
        public string? PhotoUrl { get; set; }
        public string BranchId { get; set; }
    }
}
