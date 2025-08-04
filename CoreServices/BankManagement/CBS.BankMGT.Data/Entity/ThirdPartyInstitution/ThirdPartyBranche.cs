using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class ThirdPartyBranche : BaseEntity
    {
        public string Id { get; set; }
        public string RegionId { get; set; }
        public string DivisionId { get; set; }
        public string SubdivisionId { get; set; }
        public string? TownId { get; set; }
        public string ThirdPartyInstitutionId { get; set; }
        public string? BranchName { get; set; }

        public string FocalPointContact { get; set; }
        public string FocalPointName { get; set; }
        public string  Name { get; set; }

    }
    public class CorrespondingBankBranch : BaseEntity
    {
        public string Id { get; set; }
        public string RegionId { get; set; }
        public string DivisionId { get; set; }
        public string SubdivisionId { get; set; }
        public string TownId { get; set; }
        public string ThirdPartyInstitutionId { get; set; }
        public string TownName { get; set; }
        public string FocalPointContact { get; set; }
        public string FocalPointName { get; set; }
        public string BranchName { get; set; }
    }
}
