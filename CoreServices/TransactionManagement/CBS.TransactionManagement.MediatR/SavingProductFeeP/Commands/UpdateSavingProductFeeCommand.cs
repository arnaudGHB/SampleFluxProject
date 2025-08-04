using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands
{
    /// <summary>
    /// Represents a command to update a Loan.
    /// </summary>
    public class UpdateSavingProductFeeCommand : IRequest<ServiceResponse<SavingProductFeeDto>>
    {
        public string Id { get; set; }
        public string FeeId { get; set; }
        public string SavingProductId { get; set; }
        public string FeeType { get; set; }//Withdrawal, Tranfer Or Cash-in
        public string FeePolicyType { get; set; }

    }

}
