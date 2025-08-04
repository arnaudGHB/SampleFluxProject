using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.RefundMongoP;
using CBS.NLoan.Repository.RefundP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the command to process bulk loan SO repayments.
    /// </summary>
    public class AddLoanSOBulkRepaymentCommandHandler : IRequestHandler<AddLoanSOBulkRepaymentCommand, ServiceResponse<bool>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing refund data.
        private readonly ILoanRepository _LoanRepository; // Repository for accessing loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanSOBulkRepaymentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow; // Unit of Work for managing transactions.
        private readonly IRefundDetailRepository _RefundDetailRepository; // Repository for refund details.
        private readonly IMediator _mediator; // Mediator for handling requests between services.
        private readonly ITaxRepository _TaxRepository; // Repository for managing tax-related data.
        private readonly ILoanAmortizationRepository _loanAmortizationRepository; // Repository for loan amortization details.
        private readonly ILoanProductRepaymentOrderRepository _loanProductRepaymentOrderRepository; // Repository for repayment orders.
        private readonly ILoanProductRepository _loanProductRepository; // Repository for loan product information.
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for loan applications.
        private readonly ILoanRepaymentMongoRepository _loanRepaymentMongoRepository; // MongoDB repository for loan repayments.

        /// <summary>
        /// Initializes a new instance of the <see cref="AddLoanSOBulkRepaymentCommandHandler"/> class.
        /// </summary>
        /// <param name="RefundRepository">Repository for managing refunds.</param>
        /// <param name="mapper">AutoMapper for object transformations.</param>
        /// <param name="logger">Logger for error tracking and debugging.</param>
        /// <param name="uow">Unit of Work instance for handling database transactions.</param>
        /// <param name="loanRepository">Repository for managing loan-related data.</param>
        /// <param name="mediator">Mediator instance for sending commands and queries.</param>
        /// <param name="loanAmortizationRepository">Repository for loan amortization schedules.</param>
        /// <param name="refundDetailRepository">Repository for refund details.</param>
        /// <param name="taxRepository">Repository for tax-related operations (optional).</param>
        /// <param name="loanProductRepaymentOrderRepository">Repository for loan product repayment order (optional).</param>
        /// <param name="loanProductRepository">Repository for loan products (optional).</param>
        /// <param name="loanApplicationRepository">Repository for loan applications (optional).</param>
        /// <param name="loanRepaymentMongoRepository">Repository for managing loan repayments in MongoDB (optional).</param>
        public AddLoanSOBulkRepaymentCommandHandler(
            IRefundRepository RefundRepository,
            IMapper mapper,
            ILogger<AddLoanSOBulkRepaymentCommandHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanRepository loanRepository,
            IMediator mediator,
            ILoanAmortizationRepository loanAmortizationRepository,
            IRefundDetailRepository refundDetailRepository,
            ITaxRepository taxRepository = null,
            ILoanProductRepaymentOrderRepository loanProductRepaymentOrderRepository = null,
            ILoanProductRepository loanProductRepository = null,
            ILoanApplicationRepository loanApplicationRepository = null,
            ILoanRepaymentMongoRepository loanRepaymentMongoRepository = null)
        {
            _RefundRepository = RefundRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _LoanRepository = loanRepository;
            _mediator = mediator;
            _loanAmortizationRepository = loanAmortizationRepository;
            _RefundDetailRepository = refundDetailRepository;
            _TaxRepository = taxRepository;
            _loanProductRepaymentOrderRepository = loanProductRepaymentOrderRepository;
            _loanProductRepository = loanProductRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _loanRepaymentMongoRepository = loanRepaymentMongoRepository;
        }

        /// <summary>
        /// Handles the <see cref="AddLoanSOBulkRepaymentCommand"/> to process bulk loan repayments.
        /// </summary>
        /// <param name="request">The command containing details for processing loan SO repayments.</param>
        /// <param name="cancellationToken">Cancellation token for async operations.</param>
        /// <returns>A <see cref="ServiceResponse{T}"/> indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddLoanSOBulkRepaymentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch pending loan repayments by Salary Code
                var loanRepayments = await _loanRepaymentMongoRepository.GetBySalaryCodeAndStatusAsync(request.SalaryCode, LoanSORepayment.Pending.ToString());

                if (loanRepayments != null && loanRepayments.Any())
                {
                    foreach (var loanRepayment in loanRepayments)
                    {
                        // Create a new AddLoanSORepaymentCommand for processing repayment
                        var addLoanSORepayment = new AddLoanSORepaymentCommand
                        {
                            TransactionReference = loanRepayment.TransactionReference,
                            PrincipalAmount = loanRepayment.PrincipalAmount,
                            Interest = loanRepayment.Interest,
                            LoanId = loanRepayment.LoanId,
                            PaymentChannel = loanRepayment.PaymentChannel,
                            PaymentMethod = loanRepayment.PaymentMethod,
                            TotalRepaymentAmount = loanRepayment.TotalRepaymentAmount,
                            ChargeAmount = loanRepayment.ChargeAmount,
                            TaxAmount = loanRepayment.TaxAmount,
                        };

                        // Send the repayment command for processing
                        var serviceResponse = await _mediator.Send(addLoanSORepayment, cancellationToken);

                        // Handle response status
                        if (serviceResponse.StatusCode != 200)
                        {
                            // Mark the loan repayment as failed
                            loanRepayment.Status = LoanSORepayment.Failed.ToString();
                            loanRepayment.RefundDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                            loanRepayment.Error = serviceResponse.Message;

                            // Update failed status in MongoDB
                            await _loanRepaymentMongoRepository.UpdateAsync(loanRepayment.Id, loanRepayment);
                            continue; // Skip further processing for this loan repayment
                        }

                        // Mark the loan repayment as successful
                        loanRepayment.Status = LoanSORepayment.Successful.ToString();
                        loanRepayment.RefundDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);

                        // Update successful status in MongoDB
                        await _loanRepaymentMongoRepository.UpdateAsync(loanRepayment.Id, loanRepayment);
                    }
                }

                // Return success response
                return ServiceResponse<bool>.ReturnResultWith200(true, "Bulk loan SO repayment was processed successfully.");
            }
            catch (Exception e)
            {
                // Log the exception for debugging
                _logger.LogError($"Error occurred while processing loan repayments: {e.Message}");

                // Return error response
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}

