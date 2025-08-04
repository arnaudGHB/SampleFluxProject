using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
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
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.RefundP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Refund.
    /// </summary>
    public class AddRefundCommandHandler : IRequestHandler<AddRefundCommand, ServiceResponse<RefundDto>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refund data.
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Refund data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddRefundCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IRefundDetailRepository _RefundDetailRepository;
        private readonly IMediator _mediator;
        private readonly ITaxRepository _TaxRepository;
        private readonly ILoanAmortizationRepository _loanAmortizationRepository;
        /// <summary>
        /// Constructor for initializing the AddRefundCommandHandler.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refund data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddRefundCommandHandler(
            IRefundRepository RefundRepository,
            IMapper mapper,
            ILogger<AddRefundCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ILoanRepository loanRepository,
            IMediator mediator,
            ILoanAmortizationRepository loanAmortizationRepository,
            IRefundDetailRepository refundDetailRepository,
            ITaxRepository taxRepository = null)
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
        }

        /// <summary>
        /// Handles the AddRefundCommand to add a new Refund.
        /// </summary>
        /// <param name="request">The AddRefundCommand containing Refund data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RefundDto>> Handle(AddRefundCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the loan exists
                var loan = await _LoanRepository.AllIncluding(x => x.LoanAmortizations, x => x.LoanApplication, x => x.Refunds).Include(x => x.LoanApplication.LoanProduct)
                    .FirstOrDefaultAsync(x => x.Id == request.LoanId);

                // If the loan does not exist, return a not found response
                if (loan == null)
                {
                    return ServiceResponse<RefundDto>.Return404($"Loan with ID {request.LoanId} does not exist.");
                }
                var dueAmount = request.Amount - (request.Interest + request.Tax + request.Penalty + request.Principal);
                if (dueAmount != 0)
                {
                    return ServiceResponse<RefundDto>.Return403($"Amount to refund should be equal to sum of interest, tax, penalty and principal.");

                }
                if (loan.AccrualInterest < request.Interest)
                {
                    return ServiceResponse<RefundDto>.Return403($"Invalid interest is entered. Loan interest is {BaseUtilities.FormatCurrency(loan.AccrualInterest)} and entered interest is {request.Interest}");

                }
                if (loan.Tax < request.Tax)
                {
                    return ServiceResponse<RefundDto>.Return403($"Invalid tax amount is entered. Loan current tax amount is {BaseUtilities.FormatCurrency(loan.Tax)} and entered tax amount is {request.Tax}");

                }
                if (loan.Penalty < request.Penalty)
                {
                    return ServiceResponse<RefundDto>.Return403($"Invalid penalty amount is entered. Loan current penalty amount is {BaseUtilities.FormatCurrency(loan.Penalty)} and entered penalty amount is {request.Penalty}");

                }
                if (loan.Balance < request.Amount)
                {
                    return ServiceResponse<RefundDto>.Return403($"Invalid balance amount is entered. Loan current balance is {BaseUtilities.FormatCurrency(loan.Balance)} and entered balance is {request.Amount}");

                }
                // Check if the loan is fully refunded
                if (loan.Balance <= 0)
                {
                    return ServiceResponse<RefundDto>.Return409($"Loan with ID {request.LoanId} is already fully refunded.");
                }

                var customerPICallCommand = new GetCustomerCallCommand { CustomerId = loan.CustomerId };
                var customerPICallCommandResult = await _mediator.Send(customerPICallCommand, cancellationToken);
                if (customerPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"CustomerPICallCommand failed with status code {customerPICallCommandResult.StatusCode} and message {customerPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }
                var branchPICallCommand = new BranchPICallCommand { BranchId = loan.BranchId };
                var branchPICallCommandResult = await _mediator.Send(branchPICallCommand, cancellationToken);
                if (branchPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"BranchPICallCommand failed with status code {branchPICallCommandResult.StatusCode} and message {branchPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }


                // Create a new instance of Refund entity
                var refundEntity = _mapper.Map<Refund>(request);
                refundEntity.Id = request.TransactionCode;
                refundEntity.BankId = loan.BankId;
                refundEntity.BranchId = loan.BranchId;
                refundEntity.CustomerId = loan.CustomerId;
                // Calculate the remaining balance to be paid
                decimal remainingBalanceToBePaid = loan.Balance;
                decimal amountPaid = request.Amount;
                decimal principalBalance = request.Principal;
                decimal interestBalance = request.Interest;
                decimal TaxBalance = request.Tax;
                decimal PenaltyBalance = request.Penalty;
                var vat = await _TaxRepository.All.FirstOrDefaultAsync();
                // Create a list to store refund details
                List<RefundDetail> refundDetails = new List<RefundDetail>();
                List<LoanAmortization> loanAmortizations = new List<LoanAmortization>();
                // Iterate through each amortization
                foreach (var amortization in loan.LoanAmortizations.Where(x => x.Balance > 0).OrderBy(x => x.Sno))
                {
                    // Calculate the amount to refund for this amortization
                    decimal amountToRefund = Math.Min(remainingBalanceToBePaid, amortization.Due);
                    // Update amortization details

                    if (principalBalance > 0)
                    {
                        if (principalBalance >= amortization.Principal)
                        {
                            amortization.PrincipalPaid += amortization.Principal;
                            amountPaid -= amortization.Principal;
                            principalBalance -= amortization.Principal;
                            amortization.Principal = 0;

                        }
                        else
                        {
                            amortization.PrincipalPaid += principalBalance;
                            amortization.Principal -= principalBalance;
                            amountPaid -= principalBalance;
                            principalBalance = 0;
                        }
                    }
                    if (interestBalance > 0)
                    {
                        // Check if the interest refund amount is greater than or equal to the interest due
                        if (interestBalance >= amortization.Interest)
                        {
                            amortization.InterestPaid += amortization.Interest;
                            amortization.Tax = vat.TaxRate / 100 * amortization.Interest;
                            loan.Tax+= amortization.Tax;
                            amountPaid -= amortization.Interest;
                            interestBalance -= amortization.Interest;
                            amortization.Interest = 0;
                        }
                        else
                        {
                            amortization.InterestPaid += interestBalance;
                            amortization.Interest -= interestBalance;
                            amortization.Tax = vat.TaxRate / 100 * interestBalance;
                            loan.Tax += amortization.Tax;
                            amountPaid -= interestBalance;
                            interestBalance = 0;

                        }
                    }

                    if (TaxBalance > 0)
                    {
                        // Check if the tax refund amount is greater than or equal to the tax due
                        if (TaxBalance >= amortization.Tax)
                        {
                            amortization.TaxPaid += amortization.Tax;
                            amountPaid -= amortization.Tax;
                            TaxBalance -= amortization.Tax;
                            amortization.Tax = 0;
                        }
                        else
                        {
                            amortization.TaxPaid += amortization.Tax;
                            amortization.Tax -= TaxBalance;
                            amountPaid -= TaxBalance;
                            TaxBalance = 0;

                        }
                    }

                    if (PenaltyBalance > 0)
                    {
                        // Check if the penalty refund amount is greater than or equal to the penalty due
                        if (PenaltyBalance >= amortization.Penalty)
                        {
                            amortization.PenaltyPaid += amortization.Penalty;
                            amountPaid -= amortization.Penalty;
                            PenaltyBalance -= amortization.Penalty;
                            amortization.Penalty = 0;

                        }
                        else
                        {
                            amortization.PenaltyPaid += PenaltyBalance;
                            amortization.Penalty -= PenaltyBalance;
                            PenaltyBalance = 0;
                            amountPaid -= PenaltyBalance;
                        }
                    }

                    amortization.Paid = amortization.PrincipalPaid + amortization.InterestPaid + amortization.PenaltyPaid + amortization.TaxPaid;
                    amortization.Due -= amortization.Paid;
                    //amortization.Balance -= amortization.Paid;
                    amortization.Status = amortization.Due <= 0 ? "Completed" : "Partial";

                    // Create a refund detail object
                    var refundDetail = new RefundDetail
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        RefundId = refundEntity.Id,
                        LoanAmortizationId = amortization.Id,
                        CollectedAmount = amortization.Paid,
                        PrincipalAmount = amortization.PrincipalPaid,
                        TaxAmount = amortization.TaxPaid,
                        Interest = amortization.InterestPaid,
                        PenaltyAmount = amortization.PenaltyPaid,
                        Balance = amortization.Due,
                        BranchId = loan.BranchId,
                        BankId = loan.BankId,
                        InterestBalance = amortization.Interest,
                        PrincipalBalance = amortization.Principal,
                        PenaltyAmountBalance = amortization.Penalty,
                        TaxAmountBalance = amortization.Tax,
                    };

                    // Add refund detail to the list
                    refundDetails.Add(refundDetail);
                    loanAmortizations.Add(amortization);
                    if (amountPaid == 0)
                    {
                        break;
                    }

                }

                // Check if the loan balance is fully refunded
                loan.Paid += request.Amount;
                loan.Balance -= request.Amount;

                loan.AccrualInterestPaid += request.Interest;
                loan.TaxPaid += loanAmortizations.Sum(x=>x.TaxPaid);
                loan.PenaltyPaid += request.Penalty;
                loan.TotalPrincipalPaid += request.Principal;

                loan.InterestForcasted -= request.Interest;
                loan.Tax -= request.Tax;
                loan.Penalty -= request.Penalty;
                loan.Principal -= request.Principal;
                loan.LastPayment = request.Amount;
           
                refundEntity.Balance = loan.Balance;
                if (loan.Balance == 0)
                {
                    loan.LoanStatus = LoanStatus.Closed.ToString();

                }
            
                loan.LastEventData = BaseUtilities.UtcToLocal(DateTime.Now);
                loan.LastRefundDate = BaseUtilities.UtcToLocal(DateTime.Now);
                // Add refund details to the database
                _LoanRepository.Update(loan);
                _RefundRepository.Add(refundEntity);
                _loanAmortizationRepository.UpdateRange(loanAmortizations);
                _RefundDetailRepository.AddRange(refundDetails);

                // Save changes to the database
                await _uow.SaveAsync();
                var sMSPICallCommand = LoanRefundConfirmationSMS(loan, branchPICallCommandResult.Data, customerPICallCommandResult.Data, request.Amount);
                var result = await _mediator.Send(sMSPICallCommand);
                // Map the Refund entity to RefundDto and return it with a success response
                var refundDto = _mapper.Map<RefundDto>(refundEntity);
                refundDto.LoanProduct=loan.LoanApplication.LoanProduct;
                return ServiceResponse<RefundDto>.ReturnResultWith200(refundDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Refund: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<RefundDto>.Return500(e);
            }
        }


        private SendSMSPICallCommand LoanRefundConfirmationSMS(Loan loan, BranchDto branch, CustomerDto customer, decimal amountPaid)
        {



            string msg = GenerateLoanRefundConfirmationMessage(loan, branch, customer, amountPaid);
            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
            };
        }
        private string GenerateLoanRefundConfirmationMessage(Loan loan, BranchDto branch, CustomerDto customer, decimal amountPaid)
        {
            string bankName = branch.bank.name;
            string msg;

            if (customer.language.ToLower() == "english")
            {
                msg = $"Hello {customer.firstName} {customer.lastName}, ";

                if (loan.Balance <= 0)
                {
                    msg += $"Your loan with {bankName} has been fully refunded. ";
                }
                else
                {
                    msg += $"A partial refund of {BaseUtilities.FormatCurrency(amountPaid)} has been made on your loan with {bankName}. ";
                    msg += $"Your new balance is {BaseUtilities.FormatCurrency(loan.Balance)}. ";
                }

                msg += $"Thank you for choosing {branch.name}.";

                if (loan.Balance <= 0)
                {
                    msg += $" For more information, contact customer service {branch.customerServiceContact}.";
                }
            }
            else // Assuming if not English, it's French
            {
                msg = $"Bonjour {customer.firstName} {customer.lastName}, ";

                if (loan.Balance <= 0)
                {
                    msg += $"Votre prêt avec {bankName} a été entièrement remboursé. ";
                }
                else
                {
                    msg += $"Un remboursement partiel de {BaseUtilities.FormatCurrency(amountPaid)} a été effectué sur votre prêt avec {bankName}. ";
                    msg += $"Votre nouveau solde est de {BaseUtilities.FormatCurrency(loan.Balance)}. ";
                }

                msg += $"Merci d'avoir choisi {branch.name}.";

                if (loan.Balance <= 0)
                {
                    msg += $" Pour plus d'informations, contactez le service clientèle {branch.customerServiceContact}.";
                }
            }

            return msg;
        }



    }



}
