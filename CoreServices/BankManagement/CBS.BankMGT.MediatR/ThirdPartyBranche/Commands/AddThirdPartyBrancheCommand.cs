using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data.Entity;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new ThirdPartyBranche.
    /// </summary>
    public class AddThirdPartyBrancheCommand : IRequest<ServiceResponse<CorrespondingBankBranchDto>>
    {
      
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
