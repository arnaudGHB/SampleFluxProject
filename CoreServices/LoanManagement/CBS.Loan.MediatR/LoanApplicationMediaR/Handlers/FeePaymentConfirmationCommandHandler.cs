using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanCalculatorHelper;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.LoanPurposeP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class FeePaymentConfirmationCommandHandler : IRequestHandler<FeePaymentConfirmationCommand, ServiceResponse<List<FeePaymentConfirmationDto>>>
    {
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository; // Repository for accessing Loan data.
        private readonly IMediator _mediator;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<FeePaymentConfirmationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for accessing Loan data.
        private readonly IFeeRepository _feeRepository; // Repository for accessing Loan data.
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing Loan data.

        /// <summary>
        /// Constructor for initializing the AddLoanCommandHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public FeePaymentConfirmationCommandHandler(
            ILoanApplicationFeeRepository LoanRepository,
            IMapper mapper,
            ILogger<FeePaymentConfirmationCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
            IMediator mediator = null,
            ILoanPurposeRepository loanPurposeRepository = null,
            UserInfoToken userInfoToken = null,
            ILoanApplicationRepository loanApplicationRepository = null,
            IFeeRepository feeRepository = null,
            IFeeRangeRepository feeRangeRepository = null)
        {
            _loanApplicationFeeRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _loanApplicationRepository = loanApplicationRepository;
            _feeRepository = feeRepository;
            _feeRangeRepository = feeRangeRepository;
        }

        /// <summary>
        /// Handles the AddLoanCommand to add a new Loan.
        /// </summary>
        /// <param name="request">The AddLoanCommand containing Loan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FeePaymentConfirmationDto>>> Handle(FeePaymentConfirmationCommand request, CancellationToken cancellationToken)
        {
            try
            {




                // Check if the period is "Before"
                bool isBefore = request.Period == "Before";

                // Find the loan application by its ID
                var application = await _loanApplicationRepository.FindAsync(request.LoanApplicationId);
                if (application.BranchId != _userInfoToken.BranchID)
                {
                    string errorMessage =
                        "Loan fee payments can only be processed in the branch where the loan application was initiated. " +
                        "Please ensure that fee payments are handled by the originating branch.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<List<FeePaymentConfirmationDto>>.Return403(errorMessage);
                }

                var feePaymentConfirmationDtos = new List<FeePaymentConfirmationDto>();
                // Update loan payment fees based on the period
                foreach (var payment in request.PaymentRequests)
                {
                    var loanPaymentFee = await _loanApplicationFeeRepository.FindAsync(payment.LoanApplicationFeeId);
                    var feeRange = await _feeRangeRepository.FindAsync(loanPaymentFee.FeeRangeId);
                    var loanFee = await _feeRepository.FindAsync(feeRange.FeeId);
                    loanPaymentFee.AmountPaid = payment.Amount;
                    loanPaymentFee.IsPaid = true;
                    loanPaymentFee.Status = "Completed"; // Set status based on period
                    loanPaymentFee.TransactionReference = request.TransactionReference;
                    loanPaymentFee.DateOfPayment = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                    loanPaymentFee.PaidBy = _userInfoToken.FullName;
                    _loanApplicationFeeRepository.Update(loanPaymentFee);
                    feePaymentConfirmationDtos.Add(new FeePaymentConfirmationDto { AmountPaid= payment.Amount, FeeName= loanFee.Name, EventCode= loanFee.AccountingEventCode});
                }

                // Update loan application approval status and comment if period is "Before"
                if (isBefore)
                {
                    application.ApprovalComment = LoanApplicationStatusX.Awaits_Loan_Commitee_Validation.ToString();
                    application.ApprovalStatus = LoanApplicationStatusX.Awaits_Loan_Commitee_Validation.ToString();
                    application.IsIninitalProcessingFeePaid = true;
                    _loanApplicationRepository.Update(application);
                }

                // Call commands to retrieve customer and branch information
                var customerPICallCommand = new GetCustomerCallCommand { CustomerId = application.CustomerId };
                var customerPICallCommandResult = await _mediator.Send(customerPICallCommand, cancellationToken);
                if (customerPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"CustomerPICallCommand failed with status code {customerPICallCommandResult.StatusCode} and message {customerPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<List<FeePaymentConfirmationDto>>.Return403(errorMessage);
                }

                var branchPICallCommand = new BranchPICallCommand { BranchId = customerPICallCommandResult.Data.branchId };
                var branchPICallCommandResult = await _mediator.Send(branchPICallCommand, cancellationToken);
                if (branchPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"BranchPICallCommand failed with status code {branchPICallCommandResult.StatusCode} and message {branchPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<List<FeePaymentConfirmationDto>>.Return403(errorMessage);
                }

                // Save changes to the database
                await _uow.SaveAsync();

                // Calculate total payment amount
                decimal Total = request.PaymentRequests.Sum(x => x.Amount);

                // Call command to send SMS notification
                var sMSPICallCommand = LoanApplicationSubmissionSMS(customerPICallCommandResult.Data, branchPICallCommandResult.Data, Total, isBefore);
                var result = await _mediator.Send(sMSPICallCommand);

                // Return success response
                return ServiceResponse<List<FeePaymentConfirmationDto>>.ReturnResultWith200(feePaymentConfirmationDtos);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Loan: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<FeePaymentConfirmationDto>>.Return500(e);
            }
        }
        private SendSMSPICallCommand LoanApplicationSubmissionSMS(CustomerDto customer, BranchDto branch, decimal Amount,bool isBefore)
        {
            string bankName = branch.bank.name;
            string msg = GenerateLoanApplicationSubmissionMessage(customer, branch, Amount, isBefore);
            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
            };
        }
        private string GenerateLoanApplicationSubmissionMessage(CustomerDto customer, BranchDto branch, decimal loanFee, bool isBefore)
        {
            string bankName = branch.bank.name;
            string language = customer.language.ToLower();
            string msg;

            // Determine the language and construct the message accordingly
            if (language == "english")
            {
                msg = isBefore ?
                    $"Hello {customer.firstName} {customer.lastName}, Thank you for paying your loan initialization fee of {BaseUtilities.FormatCurrency(loanFee)}\nYour application process is initialized.\nFor more information, contact customer service {branch.customerServiceContact}." :
                    $"Hello {customer.firstName} {customer.lastName}, Thank you for completing your loan fee payment of {BaseUtilities.FormatCurrency(loanFee)}\nYour loan shall be disbursed in your account shortly within the next 24H.\nFor more information, contact customer service {branch.customerServiceContact}.";
            }
            else // Assuming if not English, it's French
            {
                msg = isBefore ?
                    $"Bonjour {customer.firstName} {customer.lastName}, Merci d'avoir payé vos frais d'initialisation de prêt de {BaseUtilities.FormatCurrency(loanFee)}\nVotre processus de demande est initialisé.\nPour plus d'informations, contactez le service clientèle au {branch.customerServiceContact}." :
                    $"Bonjour {customer.firstName} {customer.lastName}, Merci d'avoir effectué le paiement de vos frais de prêt d'un montant de {BaseUtilities.FormatCurrency(loanFee)}\nVotre prêt sera bientôt versé sur votre compte dans les prochaines 24 heures.\nPour plus d'informations, veuillez contacter le service clientèle au {branch.customerServiceContact}.";
            }

            return msg;
        }

    }
}
