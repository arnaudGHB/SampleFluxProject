using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddSavingProductFeeCommand : IRequest<ServiceResponse<SavingProductFeeDto>>
    {
        public string FeeId { get; set; }
        public string SavingProductId { get; set; }
        public string FeeType { get; set; }//Withdrawal, Tranfer Or Cash-in
        public string FeePolicyType { get; set; }

    }

}
