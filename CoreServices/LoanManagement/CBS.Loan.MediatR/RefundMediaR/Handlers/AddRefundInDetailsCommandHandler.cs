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
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.RefundP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Refund.
    /// </summary>
    public class AddRefundInDetailsCommand : IRequestHandler<AddRefundCommand, ServiceResponse<RefundDto>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refund data.
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Refund data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddRefundInDetailsCommand> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly IRefundDetailRepository _RefundDetailRepository;
        private readonly IMediator _mediator;
        private readonly ILoanProductRepository _loanProductRepository; // Repository for accessing Refund data.
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for accessing Refund data.
        private readonly ITaxRepository _TaxRepository;
        private readonly ILoanAmortizationRepository _loanAmortizationRepository;

        /// <summary>
        /// Constructor for initializing the AddRefundInDetailsCommand.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refund data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddRefundInDetailsCommand(
            IRefundRepository RefundRepository,
            IMapper mapper,
            ILogger<AddRefundInDetailsCommand> logger,
            IUnitOfWork<LoanContext> uow,
            ILoanRepository loanRepository,
            IMediator mediator,
            ILoanAmortizationRepository loanAmortizationRepository,
            IRefundDetailRepository refundDetailRepository,
            ITaxRepository taxRepository = null,
            ILoanProductRepository loanProductRepository = null,
            ILoanApplicationRepository loanApplicationRepository = null)
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
            _loanProductRepository = loanProductRepository;
            _loanApplicationRepository = loanApplicationRepository;
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
                var loan = await _LoanRepository.FindAsync(request.LoanId);

                if (loan == null)
                {
                    return ServiceResponse<RefundDto>.Return404("The loan does not exist.");
                }
                if (loan.LoanStatus == LoanStatus.Closed.ToString())
                {
                    string errorMessage =
                        "The refund operation cannot proceed because the loan status is 'Closed'. " +
                        "Refunds are not allowed on loans that are already closed. Please verify the loan status and try again.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }
                // Check if the loan has been disbursed
                if (!loan.IsLoanDisbursted)
                {
                    string errorMessage =
                        "The refund operation cannot proceed because the loan has not been disbursed. " +
                        "Please ensure that the loan is fully disbursed before attempting any refund.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }

                var loanApplication = await _loanApplicationRepository.FindAsync(loan.LoanApplicationId);

                if (loanApplication == null)
                {
                    return ServiceResponse<RefundDto>.Return404("The loan application does not exist.");
                }

                var loanProduct = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);

                if (loanProduct == null)
                {
                    return ServiceResponse<RefundDto>.Return404("The loan product does not exist.");
                }

                // Validate if the refund amount matches the total of interest, tax, penalty, and principal
                var dueAmountDifference = request.Amount - (request.Interest + request.Tax + request.Penalty + request.Principal);
                if (dueAmountDifference != 0)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        "The refund amount must equal the sum of interest, tax, penalty, and principal.");
                }

                // Validate that the provided refund components (interest, tax, penalty, principal) do not exceed the loan's remaining balances
                if (loan.AccrualInterest < request.Interest)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        $"Invalid interest amount. The remaining loan interest is {BaseUtilities.FormatCurrency(loan.AccrualInterest)}, " +
                        $"but the entered interest is {request.Interest}.");
                }

                if (loan.Penalty < request.Penalty)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        $"Invalid penalty amount. The remaining loan penalty balance is {BaseUtilities.FormatCurrency(loan.Penalty)}, " +
                        $"but the entered penalty is {request.Penalty}.");
                }

                if (loan.DueAmount < request.Amount)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        $"Invalid refund amount. The loan balance is {BaseUtilities.FormatCurrency(loan.DueAmount)}, " +
                        $"but the entered refund amount is {request.Amount}.");
                }

                // Check if the loan has already been fully refunded
                if (loan.DueAmount <= 0)
                {
                    return ServiceResponse<RefundDto>.Return409($"The loan with ID {request.LoanId} has already been fully refunded.");
                }

                // Retrieve customer details to proceed with the refund process
                var customerPICallCommand = new GetCustomerCallCommand { CustomerId = loan.CustomerId };
                var customerPICallCommandResult = await _mediator.Send(customerPICallCommand, cancellationToken);

                if (customerPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage =
                        $"Failed to retrieve customer details. The CustomerPICallCommand returned a status code " +
                        $"{customerPICallCommandResult.StatusCode} with the following message: {customerPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }

                // Retrieve branch details for refund processing
                var branchPICallCommand = new BranchPICallCommand { BranchId = loan.BranchId };
                var branchPICallCommandResult = await _mediator.Send(branchPICallCommand, cancellationToken);

                if (branchPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage =
                        $"Failed to retrieve branch details. The BranchPICallCommand returned a status code " +
                        $"{branchPICallCommandResult.StatusCode} with the following message: {branchPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }
                if (loan.Tax>0)
                {
                    request.Tax = loan.VatRate / 100 * request.Interest;

                    //if (request.Tax>loan.Tax)
                    //{
                    //    request.Interest -= request.Tax;
                    //}
                    //else
                    //{
                    //    request.Interest -= request.Tax;
                    //}
                }
                else
                {
                    request.Tax = 0;
                    //request.Interest -= request.Tax;

                }
                // Create a new Refund entity
                var refundEntity = _mapper.Map<Refund>(request);
                refundEntity.Id = request.TransactionCode;
                refundEntity.BankId = loan.BankId;
                refundEntity.BranchId = loan.BranchId;
                refundEntity.CustomerId = loan.CustomerId;
                refundEntity.PaymentChannel = request.PaymentChannel;
                refundEntity.PaymentMethod = request.PaymentMethod;
                refundEntity.Comment = request.Comment;
                refundEntity.DateOfPayment = BaseUtilities.UtcNowToDoualaTime();
                // Calculate refund balances
                decimal amountPaid = request.Amount;
                decimal principalBalance = request.Principal;
                decimal interestBalance = request.Interest;
                decimal taxBalance = request.Tax;
                decimal penaltyBalance = request.Penalty;

                loan.Balance -= request.Principal;
                loan.AccrualInterest -= request.Interest;
                loan.Tax -= request.Tax;
                loan.DueAmount -= request.Amount;
                loan.Principal -= request.Principal;
                loan.LastPayment = request.Amount;
                loan.LastRefundDate = BaseUtilities.UtcNowToDoualaTime();
                loan.LastPayment = amountPaid;
                loan.Paid += amountPaid;
                loan.AccrualInterestPaid += request.Interest;
                loan.TotalPrincipalPaid += request.Principal;
                loan.TaxPaid += request.Tax;
                loan.PenaltyPaid += request.Penalty;
                refundEntity.Balance = loan.DueAmount;
                loan.LastRefundDate = BaseUtilities.UtcNowToDoualaTime();
                if (loan.DueAmount==0)
                {
                    loan.IsCompleted=true;
                    loan.IsCurrentLoan = false;
                    refundEntity.IsCompleted=true;
                    if (loan.LoanStatus==LoanStatus.Open.ToString())
                    {
                        loan.LoanStatus = LoanStatus.Closed.ToString();
                  
                    }
                    if (loan.LoanStatus==LoanStatus.Refinancing.ToString())
                    {
                        loan.IsCurrentLoan = false;
                    }
                }
                var (amortizations, refundDetails, paymentStatus) = UpdateAmortizationAndRefundDetails(loan.Id, request.Interest, request.Principal, request.Tax, request.Penalty, request.Amount, refundEntity);
                _loanAmortizationRepository.UpdateRange(amortizations);
                _RefundDetailRepository.AddRange(refundDetails);
                _RefundRepository.Add(refundEntity);
                loan.AdvancedPaymentAmount = paymentStatus.AdvancedPaymentAmount;
                loan.AdvancedPaymentDays = paymentStatus.AdvancedPaymentDays;
                loan.DeliquentDays = paymentStatus.DelinquentDays;
                loan.DeliquentAmount = paymentStatus.DelinquentAmount;
                loan.NextInstallmentDate = paymentStatus.NextPaymentDate;
                _LoanRepository.Update(loan);
                await _uow.SaveAsync();
                refundEntity.RefundDetails = refundDetails;
                var refundDto = _mapper.Map<RefundDto>(refundEntity);
                refundDto.LoanProduct = loanProduct;
                refundDto.Loan=loan;
                refundDto.IsComplete = loan.LoanStatus == LoanStatus.Closed.ToString() || loan.LoanStatus == LoanApplicationTypes.Refinancing.ToString() ? true : false;
                string message = $"Repayment of {request.Amount} was successful.";
                _logger.LogInformation(message);
                return ServiceResponse<RefundDto>.ReturnResultWith200(refundDto, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while processing refund for Loan ID: {request.LoanId}. Error: {ex.Message}");
                return ServiceResponse<RefundDto>.Return500($"An error occurred while processing refund. {ex.Message}");
            }
        }
        public async Task<ServiceResponse<RefundDto>> Handlex(AddRefundCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the loan exists
                var loan = await _LoanRepository.FindAsync(request.LoanId);

                if (loan == null)
                {
                    return ServiceResponse<RefundDto>.Return404("The loan does not exist.");
                }

                // Check if the loan has been disbursed
                if (!loan.IsLoanDisbursted)
                {
                    string errorMessage =
                        "The refund operation cannot proceed because the loan has not been disbursed. " +
                        "Please ensure that the loan is fully disbursed before attempting any refund.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }

                var loanApplication = await _loanApplicationRepository.FindAsync(loan.LoanApplicationId);

                if (loanApplication == null)
                {
                    return ServiceResponse<RefundDto>.Return404("The loan application does not exist.");
                }

                var loanProduct = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);

                if (loanProduct == null)
                {
                    return ServiceResponse<RefundDto>.Return404("The loan product does not exist.");
                }

                // Validate if the refund amount matches the total of interest, tax, penalty, and principal
                var dueAmountDifference = request.Amount - (request.Interest + request.Tax + request.Penalty + request.Principal);
                if (dueAmountDifference != 0)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        "The refund amount must equal the sum of interest, tax, penalty, and principal.");
                }

                // Validate that the provided refund components (interest, tax, penalty, principal) do not exceed the loan's remaining balances
                if (loan.AccrualInterest < request.Interest)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        $"Invalid interest amount. The remaining loan interest is {BaseUtilities.FormatCurrency(loan.AccrualInterest)}, " +
                        $"but the entered interest is {request.Interest}.");
                }

                if (loan.Penalty < request.Penalty)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        $"Invalid penalty amount. The remaining loan penalty balance is {BaseUtilities.FormatCurrency(loan.Penalty)}, " +
                        $"but the entered penalty is {request.Penalty}.");
                }

                if (loan.DueAmount < request.Amount)
                {
                    return ServiceResponse<RefundDto>.Return403(
                        $"Invalid refund amount. The loan balance is {BaseUtilities.FormatCurrency(loan.Balance)}, " +
                        $"but the entered refund amount is {request.Amount}.");
                }

                // Check if the loan has already been fully refunded
                if (loan.DueAmount <= 0)
                {
                    return ServiceResponse<RefundDto>.Return409($"The loan with ID {request.LoanId} has already been fully refunded.");
                }

                // Retrieve customer details to proceed with the refund process
                var customerPICallCommand = new GetCustomerCallCommand { CustomerId = loan.CustomerId };
                var customerPICallCommandResult = await _mediator.Send(customerPICallCommand, cancellationToken);

                if (customerPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage =
                        $"Failed to retrieve customer details. The CustomerPICallCommand returned a status code " +
                        $"{customerPICallCommandResult.StatusCode} with the following message: {customerPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }

                // Retrieve branch details for refund processing
                var branchPICallCommand = new BranchPICallCommand { BranchId = loan.BranchId };
                var branchPICallCommandResult = await _mediator.Send(branchPICallCommand, cancellationToken);

                if (branchPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage =
                        $"Failed to retrieve branch details. The BranchPICallCommand returned a status code " +
                        $"{branchPICallCommandResult.StatusCode} with the following message: {branchPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RefundDto>.Return403(errorMessage);
                }

                // Tax calculation logic
                if (loan.Tax > 0)
                {
                    request.Tax = loan.VatRate / 100 * request.Interest;
                }
                else
                {
                    request.Tax = 0;
                    request.Interest -= request.Tax;
                }

                // Create a new Refund entity
                var refundEntity = _mapper.Map<Refund>(request);
                refundEntity.Id = request.TransactionCode;
                refundEntity.BankId = loan.BankId;
                refundEntity.BranchId = loan.BranchId;
                refundEntity.CustomerId = loan.CustomerId;
                refundEntity.PaymentChannel = request.PaymentChannel;
                refundEntity.PaymentMethod = request.PaymentMethod;
                refundEntity.Comment = request.Comment;
                refundEntity.DateOfPayment = BaseUtilities.UtcNowToDoualaTime();

                // Update loan balances
                loan.Balance -= request.Principal;
                loan.AccrualInterest -= request.Interest;
                loan.Tax -= request.Tax;
                loan.DueAmount -= request.Amount;
                loan.Principal -= request.Principal;
                loan.LastPayment = request.Amount;
                loan.LastRefundDate = BaseUtilities.UtcNowToDoualaTime();
                loan.Paid += request.Amount;
                loan.AccrualInterestPaid += request.Interest;
                loan.TotalPrincipalPaid += request.Principal;
                loan.TaxPaid += request.Tax;
                loan.PenaltyPaid += request.Penalty;
                refundEntity.Balance = loan.DueAmount;

                if (loan.DueAmount == 0)
                {
                    loan.LoanStatus = LoanStatus.Closed.ToString();
                    loan.IsCurrentLoan = false;
                }

                var (amortizations, refundDetails, paymentStatus) = UpdateAmortizationAndRefundDetails(
                    loan.Id, request.Interest, request.Principal, request.Tax, request.Penalty, request.Amount, refundEntity);

                _loanAmortizationRepository.UpdateRange(amortizations);
                _RefundDetailRepository.AddRange(refundDetails);
                _RefundRepository.Add(refundEntity);

                loan.AdvancedPaymentAmount = paymentStatus.AdvancedPaymentAmount;
                loan.AdvancedPaymentDays = paymentStatus.AdvancedPaymentDays;
                loan.DeliquentDays = paymentStatus.DelinquentDays;
                loan.DeliquentAmount = paymentStatus.DelinquentAmount;
                loan.NextInstallmentDate = paymentStatus.NextPaymentDate;

                _LoanRepository.Update(loan);
                await _uow.SaveAsync();

                refundEntity.RefundDetails = refundDetails;
                var refundDto = _mapper.Map<RefundDto>(refundEntity);
                refundDto.LoanProduct = loanProduct;

                string message = $"Repayment of {request.Amount} was successful.";
                _logger.LogInformation(message);
                return ServiceResponse<RefundDto>.ReturnResultWith200(refundDto, message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while processing refund for Loan ID: {request.LoanId}. Error: {ex.Message}");
                return ServiceResponse<RefundDto>.Return500($"An error occurred while processing refund. {ex.Message}");
            }
        }

        private (List<LoanAmortization> amortizations, List<RefundDetail> refundDetails) UpdateAmortizationAndRefundDetailsX(string loanid, decimal interestPaid, decimal principalPaid, decimal taxPaid, decimal chargesPaid, decimal amountPaid, Refund refund)
        {
            var amortizations = new List<LoanAmortization>();
            var refundDetails = new List<RefundDetail>();

            var loanAmortizations = _loanAmortizationRepository.FindBy(x => x.LoanId == loanid).OrderBy(a => a.NextPaymentDate).ToList();

            foreach (var amortization in loanAmortizations)
            {
                var totalPaid = 0m;
                var totalDue = amortization.TotalDue;

                if (interestPaid > 0)
                {
                    var interestToPay = Math.Min(interestPaid, amortization.Interest - amortization.InterestPaid);
                    amortization.InterestPaid += interestToPay;
                    amortization.Balance -= interestToPay;
                    interestPaid -= interestToPay;
                    totalPaid += interestToPay;
                    totalDue -= totalPaid;
                }

                if (principalPaid > 0)
                {
                    var principalToPay = Math.Min(principalPaid, amortization.Principal - amortization.PrincipalPaid);
                    amortization.PrincipalPaid += principalToPay;
                    amortization.Balance -= principalToPay;
                    principalPaid -= principalToPay;
                    totalPaid += principalToPay;
                    totalDue -= totalPaid;
                }

                if (taxPaid > 0)
                {
                    amortization.TaxPaid += taxPaid;
                    taxPaid -= taxPaid;
                    totalPaid += taxPaid;
                    totalDue -= totalPaid;
                }

                amortization.Due = totalDue;
                amortization.Paid += totalPaid;
                amortization.TotalDue -= totalPaid;

                if (amortization.TotalDue <= 0)
                {
                    amortization.Status = "Completed";
                    amortization.IsCompleted = true;
                }

                refundDetails.Add(new RefundDetail
                {
                    RefundId = refund.Id,
                    PrincipalAmount = principalPaid,
                    Interest = interestPaid,
                    TaxAmount = taxPaid,
                    PenaltyAmount = chargesPaid,
                    Balance = amortization.Balance,
                    BankId = amortization.BankId,
                    BranchId = amortization.BranchId,
                    CollectedAmount = amountPaid,
                    InterestBalance = amortization.Interest - amortization.InterestPaid,
                    LoanAmortizationId = amortization.Id,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    PenaltyAmountBalance = amortization.Penalty - amortization.PenaltyPaid,
                    PrincipalBalance = amortization.Principal - amortization.PrincipalPaid,
                    TaxAmountBalance = amortization.Tax - amortization.TaxPaid,
                });

                amortizations.Add(amortization);

                if (principalPaid <= 0 && interestPaid <= 0)
                    break;
            }

            return (amortizations, refundDetails);
        }

        private (List<LoanAmortization> amortizations, List<RefundDetail> refundDetails,
        PaymentStatus paymentStatus)
         UpdateAmortizationAndRefundDetails(string loanid, decimal interestPaid, decimal principalPaid, decimal taxPaid,
                                            decimal chargesPaid, decimal amountPaid, Refund refund)
        {
            var amortizations = new List<LoanAmortization>();
            var refundDetails = new List<RefundDetail>();
            PaymentStatus paymentStatus = new PaymentStatus();
            var loanAmortizations = _loanAmortizationRepository.FindBy(x => x.LoanId == loanid && x.TotalDue>0).OrderBy(a => a.NextPaymentDate).ToList();

            int TadvancedPaymentDays = 0;
            int TdeliquentDays = 0;
            decimal TadvancedPaymentAmount = 0m;
            decimal TdeliquentAmount = 0m;

            foreach (var amortization in loanAmortizations)
            {
                var totalPaid = 0m;
                var totalDue = amortization.TotalDue;
                int advancedPaymentDays = 0;
                int deliquentDays = 0;
                decimal advancedPaymentAmount = 0m;
                decimal deliquentAmount = 0m;

                // Interest payment logic
                if (interestPaid > 0)
                {
                    var interestToPay = Math.Min(interestPaid, amortization.Interest - amortization.InterestPaid);
                    if (amortization.Interest>0)
                    {
                        amortization.InterestPaid += interestToPay;
                        amortization.Balance -= interestToPay;
                        amortization.Interest -= interestToPay;
                        interestPaid -= interestToPay;
                        totalPaid += interestToPay;
                        totalDue -= totalPaid;
                    }
             
                }

                // Principal payment logic
                if (principalPaid > 0)
                {
                    var principalToPay = Math.Min(principalPaid, amortization.Principal - amortization.PrincipalPaid);
                    if (amortization.Principal>0)
                    {
                        amortization.PrincipalPaid += principalToPay;
                        amortization.Balance -= principalToPay;
                        amortization.Principal -= principalToPay;
                        principalPaid -= principalToPay;
                        totalPaid += principalToPay;
                        totalDue -= totalPaid;
                    }

                }

                // Tax payment logic
                if (taxPaid > 0)
                {
                    var taxToPay = Math.Min(taxPaid, amortization.Tax - amortization.TaxPaid);
                    if (amortization.Tax>0)
                    {
                        amortization.TaxPaid += taxToPay;
                        amortization.Balance -= taxToPay;
                        amortization.Tax -= taxToPay;
                        totalPaid += taxToPay;
                        totalDue -= totalPaid;
                    }
 
                }
                // Tax payment logic
                if (chargesPaid > 0)
                {
                    var penaltyToPay = Math.Min(chargesPaid, amortization.Penalty - amortization.PenaltyPaid);
                    if (amortization.Penalty > 0)
                    {
                        amortization.PenaltyPaid += penaltyToPay;
                        amortization.Balance -= penaltyToPay;
                        amortization.Penalty -= penaltyToPay;
                        totalPaid += penaltyToPay;
                        totalDue -= totalPaid;
                    }
 
                }

                amortization.Due = totalDue;
                amortization.Paid += totalPaid;
                amortization.TotalDue -= totalPaid;

                // Check if installment is completed
                if (amortization.TotalDue <= 0)
                {
                    amortization.Status = "Completed";
                    amortization.IsCompleted = true;
                }

                // Calculate advanced or delinquent payments
                var currentDate = DateTime.Now;
                var daysDifference = (amortization.NextPaymentDate - currentDate).Days;

                if (daysDifference > 0)
                {
                    // Advanced payment
                    advancedPaymentDays = daysDifference;
                    advancedPaymentAmount = totalPaid - totalDue;
                    paymentStatus.AdvancedPaymentDays += advancedPaymentDays;
                    paymentStatus.AdvancedPaymentAmount += advancedPaymentAmount;
                }
                else if (daysDifference < 0)
                {
                    // Delinquent payment
                    deliquentDays = Math.Abs(daysDifference);
                    deliquentAmount = amortization.TotalDue - totalPaid;
                    paymentStatus.DelinquentDays += deliquentDays;
                    paymentStatus.DelinquentAmount += deliquentAmount;
                }
                refundDetails.Add(new RefundDetail
                {
                    RefundId = refund.Id,
                    PrincipalAmount = principalPaid,
                    Interest = interestPaid,
                    TaxAmount = taxPaid,
                    PenaltyAmount = chargesPaid,
                    Balance = amortization.Balance,
                    BankId = amortization.BankId,
                    BranchId = amortization.BranchId,
                    CollectedAmount = amountPaid,
                    InterestBalance = amortization.Interest - amortization.InterestPaid,
                    LoanAmortizationId = amortization.Id,
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    PenaltyAmountBalance = amortization.Penalty - amortization.PenaltyPaid,
                    PrincipalBalance = amortization.Principal - amortization.PrincipalPaid,
                    TaxAmountBalance = amortization.Tax - amortization.TaxPaid,
                });
                paymentStatus.NextPaymentDate = amortization.NextPaymentDate;
                amortizations.Add(amortization);

                if (principalPaid <= 0 && interestPaid <= 0)
                    break;
            }

            return (amortizations, refundDetails, paymentStatus);
        }
        //public void UpdateAmortizationAndRefundDetails(LoanAmortization amortization, DateTime paymentDate, decimal totalPaid)
        //{
        //    // Assume amortization has properties for TotalDue, DueDate, and InstallmentAmount
        //    decimal installmentAmount = amortization.InstallmentAmount; // Monthly installment amount (principal + interest)
        //    decimal totalDue = amortization.TotalDue; // Total amount due at the current payment
        //    DateTime dueDate = amortization.NextPaymentDate; // Due date for the current installment

        //    // Step 1: Calculate how many full installments were covered by the payment
        //    int numberOfInstallmentsPaid = (int)(totalPaid / installmentAmount);

        //    // Step 2: Calculate the remaining balance after covering full installments
        //    decimal remainingBalance = totalPaid - (numberOfInstallmentsPaid * installmentAmount);

        //    // Step 3: Calculate the number of days paid in advance (30 days per installment)
        //    int advancedPaymentDays = numberOfInstallmentsPaid * 30;

        //    // Step 4: Handle any remaining balance (could be partial payment towards the next installment)
        //    decimal advancedPaymentAmount = remainingBalance; // This is any leftover amount after covering full installments

        //    // If there's a delinquent payment (in case totalPaid < totalDue)
        //    int delinquentDays = 0;
        //    decimal delinquentAmount = 0;
        //    if (totalPaid < totalDue)
        //    {
        //        delinquentDays = (paymentDate - dueDate).Days;
        //        delinquentAmount = totalDue - totalPaid;
        //    }

        //    // Update amortization record with the results
        //    amortization.AdvancedPaymentDays = advancedPaymentDays;
        //    amortization.AdvancedPaymentAmount = advancedPaymentAmount;
        //    amortization.DelinquentDays = delinquentDays;
        //    amortization.DelinquentAmount = delinquentAmount;

        //    // Additional logic for updating amortization details in your system...
        //}

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
