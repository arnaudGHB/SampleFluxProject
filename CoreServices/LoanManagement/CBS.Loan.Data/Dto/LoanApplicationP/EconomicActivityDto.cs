using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.Data.Dto.LoanApplicationP
{
    public class EconomicActivityDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }  // Nullable since deletedDate can be null
        public string DeletedBy { get; set; }
        public int ObjectState { get; set; }
        public bool IsDeleted { get; set; }
    }
}
