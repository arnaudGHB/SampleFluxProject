using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class ValidateLoanHandler : IRequestHandler<ValidateLoanCommand, ServiceResponse<LoanApplicationDto>>
    {
        private readonly AddLoanApprovalCommandCommandHandler _LoanApprovalCommandCommandHandler;
        private readonly ILogger<ValidateLoanHandler> _logger;

        public ValidateLoanHandler(
            ILogger<ValidateLoanHandler> logger,
            AddLoanApprovalCommandCommandHandler LoanApprovalCommandCommandHandler)
        {
            _logger = logger;
            _LoanApprovalCommandCommandHandler = LoanApprovalCommandCommandHandler;
        }

        /// <summary>
        /// Handles the UpdateLoanCommand to update a Loan.
        /// </summary>
        /// <param name="request">The UpdateLoanCommand containing updated Loan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationDto>> Handle(ValidateLoanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //create update boject
                AddLoanApprovalCommand updateLoan = new AddLoanApprovalCommand();
                updateLoan.Id = request.Id;
                updateLoan.ApprovalStatus = TransactionStatus.Validaded.ToString();
                var result = _LoanApprovalCommandCommandHandler.Handle(updateLoan, cancellationToken);
                return await result;

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Loan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationDto>.Return500(e);
            }
        }
    }

}
