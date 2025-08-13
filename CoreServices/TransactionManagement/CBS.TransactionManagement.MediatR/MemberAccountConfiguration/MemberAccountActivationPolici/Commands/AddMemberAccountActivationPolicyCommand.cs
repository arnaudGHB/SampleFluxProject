using CBS.TransactionManagement.Data.Dto.MemberAccountConfiguration;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.MemberAccountConfiguration.MemberAccountActivationPolici.Commands
{
    /// <summary>
    /// Represents a command to add a new TransferLimits.
    /// </summary>
    public class AddMemberAccountActivationPolicyCommand : IRequest<ServiceResponse<MemberAccountActivationPolicyDto>>
    {
        public string PolicyName { get; set; }
        public decimal MinimumEntranceFee { get; set; }
        public decimal MaximumEntrancenFee { get; set; }
        public bool IsActive { get; set; }
        public decimal MinimumByeLawsFee { get; set; }
        public decimal MaximumByeLawsFee { get; set; }
        public decimal MinimumLoanPolicyFee { get; set; }
        public decimal MaximumLoanPolicyFee { get; set; }
        public decimal MinimumBuildingContributionFee { get; set; }
        public decimal MaximumBuildingContribution { get; set; }
        public decimal YearBuildingContributionFee { get; set; }
        public string LegalForm { get; set; }//Physical_Person Or Moral_Person
        public string AccountTypeForYearyDeductionOfBuildingContribution { get; set; }//Saving, Share, Deposit

        public string? EventCodeEntranceFee { get; set; }
        public string? EventCodeByeLawsFee { get; set; }
        public string? EventCodeLoanPolicyFee { get; set; }
        public string? EventCodeBuildingContributionFee { get; set; }
        public string BankId { get; set; }
    }

}
