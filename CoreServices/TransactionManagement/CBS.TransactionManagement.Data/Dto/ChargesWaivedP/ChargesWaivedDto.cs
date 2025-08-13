using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.ChargesWaivedP
{
    public class ChargesWaivedDto
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public decimal NormalCharge { get; set; }
        public decimal CustomCharge { get; set; }
        public DateTime DateOfWaiverRequest { get; set; }
        public DateTime DateOfWaived { get; set; }
        public bool IsWaiverDone { get; set; }
        public string TellerId { get; set; }
        public string TellerCaise { get; set; }
        public string TellerName { get; set; }
        public string WaiverInitiator { get; set; }
        public string? TransactionReference { get; set; }
        public string Comment { get; set; }
    }
}
