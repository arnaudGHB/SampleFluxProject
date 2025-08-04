using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddFeeCommand : IRequest<ServiceResponse<FeeDto>>
    {
        public string Name { get; set; }
        public string FeeBase { get; set; }//Percentage Or Range
        public string? AccountingEventCode { get; set; }
        public bool IsBoforeProcesing { get; set; }

    }

}
