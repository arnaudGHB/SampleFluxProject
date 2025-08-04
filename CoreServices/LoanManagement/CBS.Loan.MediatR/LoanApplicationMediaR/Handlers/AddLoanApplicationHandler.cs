using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerAccountDto;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.CustomerAccount.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanCalculatorHelper;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.LoanPurposeP;
using CBS.NLoan.Repository.TaxP;
using MediatR;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddLoanApplicationHandler : IRequestHandler<AddLoanApplicationCommand, ServiceResponse<LoanApplicationDto>>
    {
        private readonly ILoanApplicationRepository _loanapplicationRepository; // Repository for accessing Loan data.
        private readonly ITaxRepository _TaxRepository;
        private readonly ILoanPurposeRepository _loanPurposeRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository;
        private readonly IFeeRepository _feeRepository;
        private readonly IMediator _mediator;
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing Loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddLoanApplicationHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddLoanCommandHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddLoanApplicationHandler(
            ILoanApplicationRepository LoanRepository,
            ILoanProductRepository LoanProductRepository,
            IMapper mapper,
            ILogger<AddLoanApplicationHandler> logger,
            IUnitOfWork<LoanContext> uow,
            ITaxRepository taxRepository,
            IMediator mediator = null,
            ILoanPurposeRepository loanPurposeRepository = null,
            UserInfoToken userInfoToken = null,
            ILoanRepository loanRepository = null,
            ILoanApplicationFeeRepository loanApplicationFeeRepository = null,
            IFeeRepository feeRepository = null)
        {
            _LoanProductRepository = LoanProductRepository;
            _loanapplicationRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _TaxRepository = taxRepository;
            _mediator = mediator;
            _loanPurposeRepository = loanPurposeRepository;
            _userInfoToken = userInfoToken;
            _loanRepository = loanRepository;
            _loanApplicationFeeRepository=loanApplicationFeeRepository;
            _feeRepository=feeRepository;
        }

        /// <summary>
        /// <summary>
        /// Handles the AddLoanApplicationCommand to process and add a new loan application.
        /// It validates loan products, customer accounts, and branch information,
        /// ensuring compliance with loan policies and saving requirements.
        /// </summary>
        /// <param name="request">The AddLoanApplicationCommand containing the loan application data.</param>
        /// <param name="cancellationToken">A token to cancel the request.</param>
        /// <returns>A ServiceResponse object containing LoanApplicationDto if successful, or error messages.</returns>
        public async Task<ServiceResponse<LoanApplicationDto>> Handle(AddLoanApplicationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Initialize necessary variables for logging and error handling
                string message = null;

                // Step 2: Check if the requested LoanProduct exists and fetch related details
                var loanProduct = await _LoanProductRepository
                    .FindBy(x => x.Id == request.LoanProductId)
                    .Include(x => x.LoanProductRepaymentCycles)
                    .Include(x => x.LoanTerm)
                    .FirstOrDefaultAsync();
                if (loanProduct == null)
                {
                    message = $"The loan product with ID {request.LoanProductId} could not be found in the system. Please verify the provided Loan Product ID and try again.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return404(message);
                }

                // Step 2: Check if the application is for refinancing and validate accordingly
                if (request.LoanApplicationType==LoanApplicationTypes.Refinancing.ToString())
                {
                    // Validate the existence of the original loan for refinancing
                    var originalLoan = await _loanRepository.FindBy(x => x.Id == request.LoanId).FirstOrDefaultAsync();
                    if (originalLoan == null)
                    {
                        message = $"No original loan found for refinancing with Loan ID {request.LoanId}. Please provide a valid loan reference.";
                        _logger.LogError(message);
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                        return ServiceResponse<LoanApplicationDto>.Return404(message);
                    }

                    // Calculate the refunded percentage and validate
                    decimal refundedAmount = originalLoan.Paid;
                    decimal totalOriginalLoanAmount = originalLoan.LoanAmount;
                    decimal refundPercentage = (refundedAmount / totalOriginalLoanAmount) * 100;

                    if (refundPercentage < loanProduct.MinimumPercentageRefundBeforeRefinancing)
                    {
                        message = $"The minimum percentage refund before refinancing is {loanProduct.MinimumPercentageRefundBeforeRefinancing}%. Your current refund percentage is {refundPercentage:F2}%. Please meet the requirement before proceeding.";
                        _logger.LogWarning(message);
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                        return ServiceResponse<LoanApplicationDto>.Return403(message);
                    }
                }

         

                // Remaining steps and validations continue as in the original method

                var loanApplicationEntity = _mapper.Map<LoanApplication>(request);
                // Step 2: Check if there are any pending loan applications for the customer
                var pendingApplications = await _loanapplicationRepository.FindBy(x => x.ApprovalStatus == LoanApplicationStatus.Pending.ToString() && x.CustomerId == request.CustomerId).ToListAsync();

                if (pendingApplications.Any())
                {
                    message = $"You currently have a pending loan application (Application ID: {pendingApplications.First().Id}). Please resolve this before submitting a new application.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }

                if (request.IsPreferenceShareAccountCoverageAmount && request.PreferenceShareAccountCoverageAmount <= 0)
                {
                    const string errorMessage = "You have indicated that a Preference Share Account Coverage Amount is required, but no valid amount has been provided. Please provide a valid coverage amount.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return400(errorMessage);
                }

                if (request.IsDepositAccountCoverageAmount && request.DepositAccountCoverageAmount <= 0)
                {
                    const string errorMessage = "You have indicated that a Deposit Account Coverage Amount is required, but no valid amount has been provided. Please provide a valid coverage amount.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return400(errorMessage);
                }

                if (request.IsTermDeposiAccountCoverageAmount && request.TermDeposiAccountCoverageAmount <= 0)
                {
                    const string errorMessage = "You have indicated that a Term Deposit Account Coverage Amount is required, but no valid amount has been provided. Please provide a valid coverage amount.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return400(errorMessage);
                }



                if (loanProduct == null)
                {
                    message = $"The loan product with ID {request.LoanProductId} could not be found in the system. Please verify the provided Loan Product ID and try again.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return404(message);
                }

                var loan = await _loanRepository.FindAsync(request.LoanId);

                request.DownPaymentCoverageAmountProvided = _loanapplicationRepository.DownPaymentProvision(loanApplicationEntity, request.Amount);
                // Step 3: Perform loan validation based on product rules
                var validationErrors = LoanValidation.AddLoanValidation(request, loanProduct);
                if (!string.IsNullOrEmpty(validationErrors))
                {
                    message = $"The loan application validation failed due to the following issues: {validationErrors}. Please correct these issues and resubmit your request.";
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);

                    _logger.LogWarning("Loan validation failed: {ValidationErrors}", validationErrors);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }

                // Step 4: Retrieve customer information through a service call
                var customerPICallCommandResult = await _mediator.Send(new GetCustomerCallCommand { CustomerId = request.CustomerId }, cancellationToken);
                if (customerPICallCommandResult.StatusCode != 200)
                {
                    message = $"Unable to retrieve customer information for Customer ID {request.CustomerId}. The service returned a status code of {customerPICallCommandResult.StatusCode}. Please contact support for assistance.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }
                var customer = customerPICallCommandResult.Data;

                // Step 5: Retrieve branch information to confirm the request's validity
                var branchPICallCommandResult = await _mediator.Send(new BranchPICallCommand { BranchId = request.BranchId }, cancellationToken);
                if (branchPICallCommandResult.StatusCode != 200)
                {
                    message = $"Unable to retrieve branch information for Branch ID {request.BranchId}. The service returned a status code of {branchPICallCommandResult.StatusCode}. Please ensure the Branch ID is correct or contact support.";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }
                var branch = branchPICallCommandResult.Data;

                // Step 6: Retrieve customer accounts, required for validation of savings and shares
                var getCustomerAccountsCommandResult = await _mediator.Send(new GetCustomerAccountsCommand { CustomerId = request.CustomerId }, cancellationToken);
                if (getCustomerAccountsCommandResult.StatusCode != 200)
                {
                    message = $"Unable to retrieve account details for Customer ID {request.CustomerId}. Please verify the Customer ID or try again later. If the issue persists, contact support.";
                    _logger.LogError(getCustomerAccountsCommandResult.Message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }


                var accounts = getCustomerAccountsCommandResult.Data;
                var savingAccount = accounts.FirstOrDefault(x => x.Product.AccountType.Equals("saving", StringComparison.OrdinalIgnoreCase));

              

                if (request.LoanApplicationType==LoanApplicationTypes.Normal.ToString())
                {  // Step 7: Validate the saving account if the loan product requires it
                    if (loanProduct.IsRequiredSavingAccount && savingAccount != null)
                    {
                        decimal savingBalance = savingAccount.Balance - savingAccount.BlockedAmount;
                        if (!IsSavingBalanceEnough(savingBalance, request.Amount, request.SavingAccountCoverageRate))
                        {
                            if (savingAccount.BlockedAmount > 0)
                            {
                                var requiredSavingsAmount = CalculateBlockedAmount(request.Amount, request.SavingAccountCoverageRate);
                                var availableBalance = savingAccount.Balance - savingAccount.BlockedAmount;
                                var amountToTopUp = requiredSavingsAmount - availableBalance;

                                message = $"Your loan request of {BaseUtilities.FormatCurrency(request.Amount)} requires {BaseUtilities.FormatCurrency(requiredSavingsAmount)} in savings to meet the coverage rate of {request.SavingAccountCoverageRate}%. " +
                                          $"You currently have {BaseUtilities.FormatCurrency(availableBalance)} available, as {BaseUtilities.FormatCurrency(savingAccount.BlockedAmount)} is blocked. " +
                                          $"Please deposit an additional {BaseUtilities.FormatCurrency(amountToTopUp)} to proceed.";

                            }
                            else
                            {
                                var requiredSavingsAmount = CalculateBlockedAmount(request.Amount, request.SavingAccountCoverageRate);
                                var amountToTopUp = requiredSavingsAmount - savingBalance;

                                message = $"Your loan request of {BaseUtilities.FormatCurrency(request.Amount)} requires {BaseUtilities.FormatCurrency(requiredSavingsAmount)} in savings to meet the coverage rate of {request.SavingAccountCoverageRate}%. " +
                                          $"You currently have {BaseUtilities.FormatCurrency(savingBalance)} available. " +
                                          $"Please deposit an additional {BaseUtilities.FormatCurrency(amountToTopUp)} to proceed.";
                            }


                            await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                            _logger.LogError(message);
                            return ServiceResponse<LoanApplicationDto>.Return403(message);
                        }


                    }
                    var shareAccount = accounts.FirstOrDefault(x => x.Product.AccountType.Equals("membershare", StringComparison.OrdinalIgnoreCase));
                    // Step 9: Validate the share account if required by the loan product
                    if (loanProduct.IsRequiredShareAccount)
                    {
                        if (shareAccount == null)
                        {
                            message = $"Share account missing: No share account was found for {customerPICallCommandResult.Data.firstName}. " +
                                      $"To proceed with your loan application, please contact Member Services to create an Ordinary Share Account.";
                            _logger.LogError(message);
                            await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                            return ServiceResponse<LoanApplicationDto>.Return403(message);
                        }

                        if (shareAccount.Balance < request.ShareAccountCoverageAmount)
                        {
                            message = $"Insufficient share account balance: Your share account balance of {BaseUtilities.FormatCurrency(shareAccount.Balance)} " +
                                      $"does not meet the loan policy requirement of {BaseUtilities.FormatCurrency(request.ShareAccountCoverageAmount)}. " +
                                      $"Please deposit additional funds into your share account to fulfill the loan requirements.";
                            _logger.LogError(message);
                            await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                            return ServiceResponse<LoanApplicationDto>.Return403(message);
                        }
                    }
                    // Step 8: Validate requirements for Special Saving Facility loans
                    if (request.LoanCategory == LoanCategories.Special_Saving_Facility_Loan.ToString())
                    {
                        decimal totalCoverage = 0m;
                        StringBuilder coverageDetails = new();

                        // Add Share Account Balance if available
                        if (shareAccount != null)
                        {
                            totalCoverage += request.ShareAccountCoverageAmount;
                            coverageDetails.AppendLine($"- Share Account Balance: {BaseUtilities.FormatCurrency(shareAccount.Balance)}");
                        }
                        else
                        {
                            coverageDetails.AppendLine("- Share Account: Not provided.");
                        }
                        decimal requiredSavings = 0;
                        // Add Savings Account Balance if available and validate against provided percentage
                        if (savingAccount != null)
                        {
                            decimal availableSavings = savingAccount.Balance - savingAccount.BlockedAmount;
                            requiredSavings = CalculateBlockedAmount(request.Amount, request.SavingAccountCoverageRate); ; // Calculate the required savings coverage

                            if (availableSavings < requiredSavings)
                            {
                                message = $"Your loan application for {BaseUtilities.FormatCurrency(request.Amount)} requires a savings coverage rate of {request.SavingAccountCoverageRate}% " +
                                          $"(equivalent to {BaseUtilities.FormatCurrency(requiredSavings)}). " +
                                          $"Unfortunately, your available savings balance of {BaseUtilities.FormatCurrency(availableSavings)} is insufficient to meet this requirement. " +
                                          $"Your total savings balance is {BaseUtilities.FormatCurrency(savingAccount.Balance)}, but {BaseUtilities.FormatCurrency(savingAccount.BlockedAmount)} is currently blocked. " +
                                          $"To proceed with the loan application, please deposit additional funds to satisfy the required savings coverage.";

                                _logger.LogError(message);
                                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                                return ServiceResponse<LoanApplicationDto>.Return403(message);
                            }


                            totalCoverage += requiredSavings;
                            coverageDetails.AppendLine($"- Savings Account Balance: {BaseUtilities.FormatCurrency(availableSavings)} (Total: {BaseUtilities.FormatCurrency(savingAccount.Balance)}, Blocked: {BaseUtilities.FormatCurrency(savingAccount.BlockedAmount)})");
                        }
                        else
                        {
                            coverageDetails.AppendLine("- Savings Account: Not provided.");
                        }

                        // Add Deposit Account Balance if applicable
                        if (request.IsDepositAccountCoverageAmount && request.DepositAccountCoverageAmount > 0)
                        {
                            totalCoverage += request.DepositAccountCoverageAmount;
                            coverageDetails.AppendLine($"- Deposit Account Balance: {BaseUtilities.FormatCurrency(request.DepositAccountCoverageAmount)}");
                        }

                        // Add Preference Share Account Balance if applicable
                        if (request.IsPreferenceShareAccountCoverageAmount && request.PreferenceShareAccountCoverageAmount > 0)
                        {
                            totalCoverage += request.PreferenceShareAccountCoverageAmount;
                            coverageDetails.AppendLine($"- Preference Share Account Balance: {BaseUtilities.FormatCurrency(request.PreferenceShareAccountCoverageAmount)}");
                        }

                        // Validate total coverage
                        decimal shortfall = request.Amount - totalCoverage;


                        if (totalCoverage < request.Amount)
                        {
                            message = $"Your loan application for {BaseUtilities.FormatCurrency(request.Amount)} cannot be processed as the total coverage provided is insufficient. " +
                                      $"The total coverage currently provided is {BaseUtilities.FormatCurrency(totalCoverage)}, leaving a shortfall of {BaseUtilities.FormatCurrency(shortfall)} to fully cover the requested loan amount. " +
                                      $"For a Special Saving Facility Loan, the combined values provided from Share, Savings, Deposit, and Preference Share accounts must completely cover the loan amount.\n\n" +
                                      $"Coverage Details:\n\n" +
                                      $"Share Account Coverage: {BaseUtilities.FormatCurrency(request.ShareAccountCoverageAmount)},\n" +
                                      $"Savings Account Coverage: {BaseUtilities.FormatCurrency(requiredSavings)},\n" +
                                      $"Deposit Account Coverage: {BaseUtilities.FormatCurrency(request.DepositAccountCoverageAmount)},\n" +
                                      $"Preference Share Coverage: {BaseUtilities.FormatCurrency(request.PreferenceShareAccountCoverageAmount)}\n\n" +
                                      $"To proceed, please provide additional funds amounting to {BaseUtilities.FormatCurrency(shortfall)} to meet the required coverage and re-submit your application form.";

                            _logger.LogError(message);
                            await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                            return ServiceResponse<LoanApplicationDto>.Return403(message);
                        }



                    }

                }




                // Step 10: Calculate VAT and other loan charges based on loan amount and type
                const decimal defaultSavingControlAmount = 2000000.00m; // Default control amount for savings
                const decimal defaultTaxRate = 19.25m; // Default tax rate
                var vat = await _TaxRepository.FindBy(x => x.IsVat && x.IsDeleted == false).FirstOrDefaultAsync();

                // Step 11: Create and populate the LoanApplication entity
                loanApplicationEntity.DownPaymentCoverageAmountProvided = request.DownPaymentCoverageAmountProvided;
                await _loanapplicationRepository.ValidateMembersAccounts(loanApplicationEntity, customerPICallCommandResult.Data, accounts, loanProduct);
                loanApplicationEntity.ApplicationDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                loanApplicationEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Fetch fees paid before processing
                if (request.FeeIds == null || !request.FeeIds.Any())
                {
                    message = $"Your loan application for {BaseUtilities.FormatCurrency(request.Amount)} cannot be processed because the required processing fees have not been selected. " +
                              $"Certain fees must be applied before processing your loan application. Without these mandatory fees, we cannot proceed with your request.\n\n" +
                              $"To continue with your application, please ensure that all necessary fees are included and resubmit your loan application.";

                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }

                // Step 1: Convert FeeIds to a List
                var feeGuids = request.FeeIds.ToList();

                // Step 2: Retrieve all fees from the database
                var allFees = await _feeRepository.All.ToListAsync();

                // Step 3: Manually filter using nested foreach loops (No Contains)
                var feesBefore = new List<Fee>();

                foreach (var fee in allFees)
                {
                    foreach (var feeId in feeGuids)
                    {
                        if (fee.Id == feeId && fee.IsBoforeProcesing)
                        {
                            feesBefore.Add(fee);
                            break; // Exit inner loop early to optimize performance
                        }
                    }
                }







                // Check if there are any fees before processing
                if (feesBefore.Any() || request.IsPaidFeeAfterProcessing)
                {
                    loanApplicationEntity.ApprovalStatus = LoanApplicationStatusX.Await_Initialization_Fee_Payment.ToString();
                }
                else
                {
                    loanApplicationEntity.ApprovalStatus = LoanApplicationStatusX.Awaits_Loan_Commitee_Validation.ToString();
                }
            
                loanApplicationEntity.ApprovalComment=loanApplicationEntity.ApprovalStatus;
                loanApplicationEntity.VatRate = vat != null && request.Amount > vat.SavingControlAmount ? vat.TaxRate : 0;
                loanApplicationEntity.LoanProduct = null;
                loanApplicationEntity.LoanPurpose = null;
                loanApplicationEntity.BankId = customerPICallCommandResult.Data.bankId;
                loanApplicationEntity.LoanId = request.LoanApplicationType == LoanApplicationTypes.Normal.ToString() ? "N/A" : request.LoanId;
                loanApplicationEntity.OrganizationId = "1";
                loanApplicationEntity.LoanApplicationType = request.LoanApplicationType;
                loanApplicationEntity.IsDisbursed = false;
                loanApplicationEntity.DateInterestCalaculationWasStoped = DateTime.MinValue;
                loanApplicationEntity.ApprovalDate = DateTime.MinValue;
                loanApplicationEntity.DisbursementDate = DateTime.MinValue;
                loanApplicationEntity.Status = LoanApplicationStatus.Pending.ToString();
                loanApplicationEntity.DisburstmentType = LoanApplicationStatus.Pending.ToString();
                loanApplicationEntity.AmortizationType = AmortizationType.Constant_Amortization.ToString();
                var repaymentCycle = loanProduct.LoanProductRepaymentCycles.FirstOrDefault(x => x.Id == request.RepaymentCircle)?.RepaymentCycle;
                loanApplicationEntity.NumberOfRepayment = LoanCalculator.CalculateTotalPayments(request.LoanDuration, repaymentCycle);
                loanApplicationEntity.FirstInstallmentDate = BaseUtilities.UtcNowToDoualaTime().AddMonths(request.GracePeriodBeforeFirstPayment);
                loanApplicationEntity.LoanPurpose = null;
                loanApplicationEntity.LoanManager = _userInfoToken.FullName;
                loanApplicationEntity.IsIninitalProcessingFeePaid = false;
                loanApplicationEntity.IsUpload = false;
                loanApplicationEntity.OldLoanAmount = request.OldLoanPayment.Amount;
                loanApplicationEntity.OldLoanInterest = request.OldLoanPayment.Interest;
                loanApplicationEntity.OldLoanCapital = request.OldLoanPayment.Capital;
                loanApplicationEntity.OldLoanVat = request.OldLoanPayment.VAT;
                loanApplicationEntity.OldLoanPenalty = request.OldLoanPayment.Penalty;
                loanApplicationEntity.CustomerName = customer.firstName+" "+customer.lastName;
                loanApplicationEntity.BranchCode = _userInfoToken.BranchCode;
                loanApplicationEntity.BranchName = _userInfoToken.BranchName;

                if (request.IsInterestPaidUpFront)
                {
                    loanApplicationEntity.IsInterestPaidUpFront=false;
                    loanApplicationEntity.InterestAmountUpfront=0;

                    //loanApplicationEntity.InterestAmountUpfront=LoanCalculator.CalculateInterestUpfront(request.Amount, request.InterestRate, request.LoanDuration);
                }

                if (request.LoanApplicationType==LoanApplicationTypes.Refinancing.ToString())
                {
                    loanApplicationEntity.RequestedAmount=request.Amount;
                }
                // Step 12: Add the loan application to the repository
                _loanapplicationRepository.Add(loanApplicationEntity);
                _logger.LogInformation("Added loan application with ID {LoanApplicationId} for Customer ID {CustomerId}.", loanApplicationEntity.Id, request.CustomerId);
                // Step 13: Process loan application fees
                var addLoanApplicationFeeCommand = new AddLoanApplicationFeeCommand
                {
                    IsWithin = true,
                    FeeId = request.FeeIds,
                    LoanApplicationId = loanApplicationEntity.Id,
                    Amount = request.Amount,
                    IsCashDeskPayment = request.IsPaidAllFeeUpFront ? false : true,
                    CustomerId = request.CustomerId
                };
                var addLoanApplicationFeeCommandResults = await _mediator.Send(addLoanApplicationFeeCommand, cancellationToken);
                if (addLoanApplicationFeeCommandResults.StatusCode != 200)
                {
                    message = $"Failed to process loan application fees for Loan Application ID {loanApplicationEntity.Id}. Message: {addLoanApplicationFeeCommandResults.Message}";
                    _logger.LogError(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                    return ServiceResponse<LoanApplicationDto>.Return403(message);
                }

                // Step 14: Block relevant customer accounts if needed
                var accountToBlockeds = GetBlockedAccounts(accounts, loanProduct, request);
                if (accountToBlockeds.Any())
                {
                    // Serialize the accounts to be blocked for logging
                    var serializedAccounts = JsonConvert.SerializeObject(accountToBlockeds);

                    var blockListOfAccountBalance = new BlockListOfAccountBalanceCommand
                    {
                        LoanApplicationId = loanApplicationEntity.Id,
                        AccountToBlockeds = accountToBlockeds
                    };

                    var blockListOfAccountBalanceResult = await _mediator.Send(blockListOfAccountBalance);
                    if (blockListOfAccountBalanceResult.StatusCode != 200)
                    {
                        string errorMessage = $"Failed to block accounts for Loan Application ID {loanApplicationEntity.Id}. Message: {blockListOfAccountBalanceResult.Message}";
                        _logger.LogError(errorMessage);
                        await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                        return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                    }

                    // Log the blocked accounts as a serialized string
                    _logger.LogInformation("Blocked accounts for Loan Application ID {LoanApplicationId}: {BlockedAccounts}", loanApplicationEntity.Id, serializedAccounts);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Forbidden, LogAction.LoanApplicationSubmitted, LogLevelInfo.Warning);
                }


                // Step 15: Save changes to the database
                await _uow.SaveAsync();

                // Step 16: Send SMS notification regarding the loan application
                var smsPICallCommand = LoanApplicationSubmissionSMS(customerPICallCommandResult.Data, branchPICallCommandResult.Data, request, addLoanApplicationFeeCommandResults.Data);
                var result = await _mediator.Send(smsPICallCommand);
                _logger.LogInformation("SMS notification sent for Loan Application ID {LoanApplicationId} to Customer {name}.", loanApplicationEntity.Id, customer.firstName);

                // Step 17: Return successful response
                var loanApplicationDto = _mapper.Map<LoanApplicationDto>(loanApplicationEntity);

                // Construct an explicit success message with details about the loan application
                string successMessage = $"Loan application processed successfully for {customer.firstName} {customer.lastName}. " +
                                        $"Loan Amount: {BaseUtilities.FormatCurrency(request.Amount)}, " +
                                        $"Loan Product: {loanProduct.ProductName}, Duration: {request.LoanDuration} months. " +
                                        $"The application status is currently '{loanApplicationEntity.ApprovalStatus}'. You will be notified of any updates.";

                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.Created, LogAction.LoanApplicationSubmitted, LogLevelInfo.Information);

                // Return the response with the detailed success message
                return ServiceResponse<LoanApplicationDto>.ReturnResultWith200(loanApplicationDto, successMessage);
            }
            catch (Exception ex)
            {
                string message = $"An error occurred while processing loan application for Customer ID {request.CustomerId}: Error: {ex.Message}";
                // Step 18: Log unexpected errors and return a failure response
                _logger.LogError(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.Created, LogAction.LoanApplicationSubmitted, LogLevelInfo.Information);

                return ServiceResponse<LoanApplicationDto>.Return500("An unexpected error occurred while processing the loan application. Please try again later.");
            }
        }
        public decimal CalculateBlockedAmount(decimal amount, decimal coverageRate)
        {
            // Calculate the blocked amount based on the provided coverage rate
            return amount * (coverageRate / 100);
        }


        /// <summary>
        /// This function calculates and returns a list of accounts to be blocked based on the loan application request.
        /// It checks if the account types require blocking, calculates the coverage amounts, and adds them to the blocked accounts list.
        /// </summary>
        /// <param name="accounts">List of accounts to check for blocking.</param>
        /// <param name="loanProduct">The loan product to check for required accounts.</param>
        /// <param name="request">The loan application request containing the amounts and coverage details.</param>
        /// <returns>A list of accounts to be blocked with the respective blocked amounts and reasons.</returns>
        // Assuming the CalculateBlockedAmount method is already defined
        public List<AccountToBlocked> GetBlockedAccounts(List<AccountDto> accounts, LoanProduct loanProduct, AddLoanApplicationCommand request)
        {
            var blockedAccounts = new List<AccountToBlocked>(); // Initialize the list to store blocked account information

            // Create a mapping of account types to coverage information. Each entry maps an account type to a function
            // that calculates the required coverage amount and the reason for blocking that account.
            var accountTypeMappings = new List<(string AccountType, Func<AddLoanApplicationCommand, decimal> GetCoverageAmount, string Reason)>
            {
                ("membershare", (cmd) => cmd.ShareAccountCoverageAmount, "Required Share Account"), // Map for share account
                ("saving", (cmd) => CalculateBlockedAmount(cmd.Amount, cmd.SavingAccountCoverageRate), "Required Saving Account"), // Updated for saving account
                ("salary", (cmd) => cmd.SalaryAccountCoverageAmount, "Required Salary Account"), // Map for salary account
                ("preferenceshare", (cmd) => cmd.PreferenceShareAccountCoverageAmount, "Preference Share Account Coverage"), // Map for preference share account
                ("deposit", (cmd) => cmd.DepositAccountCoverageAmount, "Deposit Account Coverage"), // Map for deposit account
                ("termdepost", (cmd) => cmd.TermDeposiAccountCoverageAmount, "Term Deposit Account Coverage") // Map for term deposit account
            };

            // Iterate over each account in the provided accounts list
            foreach (var account in accounts)
            {
                // Check if there is a matching account type in the accountTypeMappings list
                var accountTypeMapping = accountTypeMappings.FirstOrDefault(mapping => account.AccountType.Equals(mapping.AccountType, StringComparison.OrdinalIgnoreCase));

                if (accountTypeMapping != default) // Proceed only if a matching account type was found
                {
                    bool isRequiredAccount = false; // Flag to check if the account is required for the loan application
                    decimal coverageAmount = 0; // Variable to store the calculated coverage amount

                    // Check if the account is required for the loan application and calculate the coverage amount accordingly
                    if (account.AccountType.Equals("membershare", StringComparison.OrdinalIgnoreCase) && loanProduct.IsRequiredShareAccount)
                    {
                        isRequiredAccount = true;
                        coverageAmount = accountTypeMapping.GetCoverageAmount(request); // Get the coverage amount from the mapping
                    }
                    else if (account.AccountType.Equals("saving", StringComparison.OrdinalIgnoreCase) && loanProduct.IsRequiredSavingAccount)
                    {
                        isRequiredAccount = true;
                        coverageAmount = accountTypeMapping.GetCoverageAmount(request); // Get the coverage amount for saving account using CalculateBlockedAmount
                    }
                    else if (account.AccountType.Equals("salary", StringComparison.OrdinalIgnoreCase) && loanProduct.IsRequiredSalaryccount)
                    {
                        isRequiredAccount = true;
                        coverageAmount = accountTypeMapping.GetCoverageAmount(request); // Get the coverage amount for salary account
                    }
                    else if (account.AccountType.Equals("preferenceshare", StringComparison.OrdinalIgnoreCase) && request.IsPreferenceShareAccountCoverageAmount)
                    {
                        isRequiredAccount = true;
                        coverageAmount = accountTypeMapping.GetCoverageAmount(request); // Get the coverage amount for preference share account
                    }
                    else if (account.AccountType.Equals("deposit", StringComparison.OrdinalIgnoreCase) && request.IsDepositAccountCoverageAmount)
                    {
                        isRequiredAccount = true;
                        coverageAmount = accountTypeMapping.GetCoverageAmount(request); // Get the coverage amount for deposit account
                    }
                    else if (account.AccountType.Equals("termdepost", StringComparison.OrdinalIgnoreCase) && request.IsTermDeposiAccountCoverageAmount)
                    {
                        isRequiredAccount = true;
                        coverageAmount = accountTypeMapping.GetCoverageAmount(request); // Get the coverage amount for term deposit account
                    }

                    // If the account is required and has a valid coverage amount, add it to the blocked accounts list
                    if (isRequiredAccount && coverageAmount > 0)
                    {
                        blockedAccounts.Add(new AccountToBlocked
                        {
                            AccountNumber = account.AccountNumber, // Set the account number
                            Amount = coverageAmount, // Set the calculated coverage amount to block
                            Reason = accountTypeMapping.Reason // Set the reason for blocking the account
                        });
                    }
                }
            }

            return blockedAccounts; // Return the list of accounts that need to be blocked
        }


        private bool IsSavingBalanceEnough(decimal savingBalance, decimal loanAmount, decimal percentage)
        {
            // Calculate 70% of the loan amount
            decimal seventyPercentOfLoan = loanAmount * percentage / 100;

            // Check if saving balance is greater than or equal to 70% of the loan amount
            return savingBalance >= seventyPercentOfLoan;
        }

        private SendSMSPICallCommand LoanApplicationSubmissionSMS(CustomerDto customer, BranchDto branch, AddLoanApplicationCommand request, decimal Charges)
        {
            string bankName = branch.bank.name;
            string msg = GenerateLoanApplicationSubmissionMessage(customer, branch, request, Charges);
            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
            };
        }
        private string GenerateLoanApplicationSubmissionMessage(CustomerDto customer, BranchDto branch, AddLoanApplicationCommand request, decimal Charges)
        {
            string msg;
            string loanType = request.LoanApplicationType.ToLower() == "normal" ? "New Loan" : request.LoanApplicationType;
            string chargeAmount = BaseUtilities.FormatCurrency(Charges); // Format the charge amount as currency

            bool isUpfrontPaymentRequired = request.LoanApplicationType.ToLower() == "reschedule" || request.LoanApplicationType.ToLower() == "restructure";

            if (customer.language.ToLower() == "english")
            {
                msg = $"Dear {customer.firstName} {customer.lastName},\n" +
                      $"We have received your {loanType.ToLower()} application.\n" +
                      (isUpfrontPaymentRequired
                        ? $"Please pay the required fees of {chargeAmount} at the cash desk to proceed with your application."
                        : request.IsPaidAllFeeUpFront
                            ? $"The necessary fees of {chargeAmount} will be deducted from your loan amount once it's approved."
                            : $"Please pay the required fees of {chargeAmount} at the cash desk so we can process your application.") + "\n" +
                      $"For any questions or assistance, please contact us at {branch.bank.customerServiceContact}.\n" +
                      $"Thank you for choosing {branch.bank.name}.";
            }
            else // French
            {
                msg = $"Cher {customer.firstName} {customer.lastName},\n" +
                      $"Nous avons bien reçu votre demande de prêt pour {loanType.ToLower()}.\n" +
                      (isUpfrontPaymentRequired
                        ? $"Veuillez payer les frais requis de {chargeAmount} au guichet afin de poursuivre votre demande."
                        : request.IsPaidAllFeeUpFront
                            ? $"Les frais nécessaires de {chargeAmount} seront déduits du montant de votre prêt une fois approuvé."
                            : $"Veuillez payer les frais requis de {chargeAmount} au guichet afin que nous puissions traiter votre demande.") + "\n" +
                      $"Pour toute question ou assistance, veuillez nous contacter au {branch.bank.customerServiceContact}.\n" +
                      $"Merci d'avoir choisi {branch.bank.name}.";
            }

            return msg;
        }


        //private string GenerateLoanApplicationSubmissionMessage(CustomerDto customer, LoanApplication loanApplication, BranchDto branch)
        //{
        //    string bankName = branch.bank.name;
        //    string msg;

        //    if (customer.language.ToLower() == "english")
        //    {
        //        msg = $"Hello {customer.firstName} {customer.lastName}, Thank you for submitting your loan application to {bankName}.\n\nLoan Application Details:\n- Amount: {BaseUtilities.FormatCurrency(loanApplication.Amount)}\n- Loan duration: {loanApplication.LoanDuration} Months\n- VAT: {loanApplication.VatRate}%\n- Interest rate: {loanApplication.InterestRate}%\n- Fee: {loanApplication.ProcessingFee}% & {loanApplication.InspectionFee}% of {BaseUtilities.FormatCurrency(loanApplication.Amount)}\n- Purpose: {loanApplication.LoanPurpose.PurposeName}\n\nWe have received your application and will process it shortly.\nThank you for choosing {branch.name} for your financial needs.\nFor more information, contact customer service {branch.customerServiceContact}.";
        //    }
        //    else // Assuming if not English, it's French
        //    {
        //        msg = $"Bonjour {customer.firstName} {customer.lastName}, Merci d'avoir soumis votre demande de prêt à {bankName}.\n\nDétails de la demande de prêt:\n- Montant: {BaseUtilities.FormatCurrency(loanApplication.Amount)}\n- Durée du prêt: {loanApplication.LoanDuration} mois\n- TVA: {loanApplication.VatRate}%\n- Taux d'intérêt: {loanApplication.InterestRate}%\n- Frais: {loanApplication.ProcessingFee}% & {loanApplication.InspectionFee}% de {BaseUtilities.FormatCurrency(loanApplication.Amount)}\n- Objectif: {loanApplication.LoanPurpose.PurposeName}\n\nNous avons bien reçu votre demande et la traiterons sous peu.\nMerci d'avoir choisi {branch.name} pour vos besoins financiers.\nPour plus d'informations, contactez le service clientèle {branch.customerServiceContact}.";
        //    }

        //    return msg;
        //}
    }
}
