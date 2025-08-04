using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Data;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerAccountDto;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Data.Helper;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.Command;
using CBS.NLoan.MediatR.CustomerAccount.Command;
using CBS.NLoan.MediatR.CustomerP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanMediaR.Commands;
using CBS.NLoan.MediatR.Notifications.Commands;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.CollateralP;
using CBS.NLoan.Repository.DocumentP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{

    public class AddLoanApprovalCommandCommandHandler : IRequestHandler<AddLoanApprovalCommand, ServiceResponse<LoanApplicationDto>>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository;
        private readonly ILoanProductRepository _loanProductRepository;
        private readonly ILoanGuarantorRepository _loanGuarantorRepository; // Repository for accessing LoanGuarantor data.
        private readonly ILoanCollateralRepository _loanCollateralRepository; // Repository for accessing LoanGuarantor data.
        private readonly IDocumentAttachedToLoanRepository _documentAttachedToLoanRepository; // Repository for accessing LoanGuarantor data.

        private readonly ILogger<AddLoanApprovalCommandCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<LoanContext> _unitOfWork;
        private readonly IMediator _mediator;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the UpdateLoanCommandHandler.
        /// </summary>
        /// <param name="loanApplicationRepository">Repository for Loan data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="unitOfWork">Unit of Work for database transactions.</param>
        /// <param name="mediator">Mediator for handling additional commands.</param>
        public AddLoanApprovalCommandCommandHandler(
            ILoanApplicationRepository loanApplicationRepository,
            ILogger<AddLoanApprovalCommandCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> unitOfWork = null,
            IMediator mediator = null,
            UserInfoToken userInfoToken = null,
            ILoanApplicationFeeRepository loanApplicationFeeRepository = null,
            ILoanProductRepository loanProductRepository = null,
            ILoanGuarantorRepository loanGuarantorRepository = null,
            ILoanCollateralRepository loanCollateralRepository = null,
            IDocumentAttachedToLoanRepository documentAttachedToLoanRepository = null)
        {
            _loanApplicationRepository = loanApplicationRepository ?? throw new ArgumentNullException(nameof(loanApplicationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _loanProductRepository = loanProductRepository;
            _loanGuarantorRepository = loanGuarantorRepository;
            _loanCollateralRepository = loanCollateralRepository;
            _documentAttachedToLoanRepository = documentAttachedToLoanRepository;
        }

        /// <summary>
        /// Handles the UpdateLoanCommand to update a Loan.
        /// </summary>
        /// <param name="request">The UpdateLoanCommand containing updated Loan data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<LoanApplicationDto>> Handle(AddLoanApprovalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Initialize necessary variables for logging and error handling
                string message = null;

                // Step 2: Retrieve the loan application based on the request ID
                var loanApplication = await _loanApplicationRepository.FindAsync(request.Id);

                // Step 3: Check if the loan application exists
                if (loanApplication == null)
                {
                    string errorMessage = $"Loan application with ID {request.Id} was not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanApplicationDto>.Return404(errorMessage);
                }

                // Step 4: Handle the case where the approval status is "Pending"
                if (request.ApprovalStatus == LoanApplicationStatus.Rejected.ToString())
                {
                    // Check if the loan application is already approved
                    if (loanApplication.ApprovalStatus == LoanApplicationStatus.Approved.ToString())
                    {
                        string errorMessage = "Operation denied: The loan application is already approved and cannot be rejected.";
                        _logger.LogError(errorMessage);

                        // Log and audit the attempted operation
                        await BaseUtilities.LogAndAuditAsync(
                            errorMessage,
                            request,
                            HttpStatusCodeEnum.Forbidden,
                            LogAction.LoanApplicationRejectionAttempt,
                            LogLevelInfo.Warning
                        );

                        // Return a forbidden response with the error message
                        return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                    }

                    // Update the loan application details for a rejected status
                    loanApplication.ApplicationDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow); // Update the application date
                    loanApplication.ApprovalStatus = LoanApplicationStatus.Rejected.ToString(); // Set the status to "Rejected"
                    loanApplication.IsDisbursed = false; // Ensure the loan is marked as not disbursed
                    loanApplication.IsApproved = false; // Mark the loan as not approved
                    loanApplication.Status = LoanApplicationStatus.Rejected.ToString(); // Set the main status to "Rejected"
                    loanApplication.ApprovalDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow); // Clear the approval date as the loan is rejected
                    loanApplication.DisbursementDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow); // Clear the disbursement date

                    // Update related loan application fees to reflect the rejection status
                    var loanApplicationFees = _loanApplicationFeeRepository
                        .FindBy(x => x.LoanApplicationId == request.Id)
                        .ToList();

                    foreach (var applicationFee in loanApplicationFees)
                    {
                        applicationFee.Status = LoanApplicationStatus.Rejected.ToString(); // Update fee status
                        _loanApplicationFeeRepository.Update(applicationFee); // Save changes for each fee
                    }

                    // Save the changes to the loan application and its fees
                    _loanApplicationRepository.Update(loanApplication);
                    await _unitOfWork.SaveAsync();

                    // Log the successful rejection update
                    message = $"Loan application with ID {loanApplication.Id} has been updated to 'Rejected' successfully.";
                    _logger.LogInformation(message);

                    // Create an audit log entry
                    await BaseUtilities.LogAndAuditAsync(
                        message,
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.LoanApplicationRejected,
                        LogLevelInfo.Information
                    );

                    // Map the updated loan application to a DTO and return the response
                    var updatedLoanApplicationDto = _mapper.Map<LoanApplicationDto>(loanApplication);
                    return ServiceResponse<LoanApplicationDto>.ReturnResultWith200(updatedLoanApplicationDto, message);
                }


                var product = await _loanProductRepository.FindAsync(loanApplication.LoanProductId);

                // Step 3: Check if charges are not inclusive, and ensure all required fees are paid
                if (loanApplication.IsPaidFeeBeforeProcessing)
                {
                    // Retrieve all fees associated with the loan application
                    var loanApplicationFees = _loanApplicationFeeRepository
                        .FindBy(x => x.LoanApplicationId == request.Id)
                        .ToList();

                    if (loanApplicationFees.Any())
                    {
                        // Step 3a: Check for unpaid fees categorized as "Before Processing"
                        if (LoanHelper.HasUnpaidFees(loanApplicationFees, "Before"))
                        {
                            string errorMessage =
                                "Approval halted: There are outstanding fees that need to be settled before the loan application can proceed. " +
                                "These fees fall under the category 'Before Processing Fees'. Please ensure all necessary payments are completed to continue.";

                            _logger.LogError(errorMessage);

                            // Add to utility logger for audit
                            await BaseUtilities.LogAndAuditAsync(
                                errorMessage,
                                loanApplicationFees,
                                HttpStatusCodeEnum.Forbidden,
                                LogAction.LoanApplicationApproval,
                                LogLevelInfo.Warning
                            );

                            // Return a 404 response with the error message
                            return ServiceResponse<LoanApplicationDto>.Return404(errorMessage);
                        }

                        // Step 3b: Check for unpaid fees categorized as "After Processing"
                        if (loanApplication.IsPaidFeeAfterProcessing)
                        {
                            if (LoanHelper.HasUnpaidFees(loanApplicationFees, "After"))
                            {
                                string errorMessage =
                                    "Approval halted: There are outstanding fees required after processing and validation. " +
                                    "Please settle all 'After Processing Fees' before proceeding with the loan approval.";

                                _logger.LogError(errorMessage);

                                // Add to utility logger for audit
                                await BaseUtilities.LogAndAuditAsync(
                                    errorMessage,
                                    loanApplicationFees,
                                    HttpStatusCodeEnum.Forbidden,
                                    LogAction.LoanApplicationApproval,
                                    LogLevelInfo.Warning
                                );

                                // Return a 404 response with the error message
                                return ServiceResponse<LoanApplicationDto>.Return404(errorMessage);
                            }
                        }
                    }
                }
                // Check if the loan application was initiated in the same branch
                if (loanApplication.BranchId != _userInfoToken.BranchID)
                {
                    string errorMessage =
                        $"Branch mismatch: Loan application ID {loanApplication.Id} was initiated in Branch ID {loanApplication.BranchId}, " +
                        $"but the current user belongs to Branch ID {_userInfoToken.BranchID}. Loan approvals are restricted to the initiating branch. " +
                        $"Please contact Branch ID {loanApplication.BranchId} for further assistance.";

                    _logger.LogError(errorMessage);

                    // Log the error with Base Utility Logger
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.LoanApplicationApproval,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<LoanApplicationDto>.Return404(errorMessage);
                }

                // Check if the loan application is already approved
                if (loanApplication.Status == LoanApplicationStatus.Approved.ToString())
                {
                    string errorMessage = $"Duplicate approval: Loan application with ID {loanApplication.Id} is already approved.";
                    _logger.LogWarning(errorMessage);

                    // Log the warning with Base Utility Logger
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.LoanApplicationApproval,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                }

                // Retrieve customer information
                var customerPICallCommand = new GetCustomerCallCommand { CustomerId = loanApplication.CustomerId };
                var customerPICallCommandResult = await _mediator.Send(customerPICallCommand, cancellationToken);

                if (customerPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"Failed to retrieve customer information for Customer ID {loanApplication.CustomerId}. Service returned: {customerPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);

                    // Log the error with Base Utility Logger
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.LoanApplicationApproval,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                }

                var customer = customerPICallCommandResult.Data;

                // Retrieve branch information
                var branchPICallCommand = new BranchPICallCommand { BranchId = _userInfoToken.BranchID };
                var branchPICallCommandResult = await _mediator.Send(branchPICallCommand, cancellationToken);

                if (branchPICallCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"Failed to retrieve branch information for Branch ID {_userInfoToken.BranchID}. Service returned: {branchPICallCommandResult.Message}";
                    _logger.LogError(errorMessage);

                    // Log the error with Base Utility Logger
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.LoanApplicationApproval,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<LoanApplicationDto>.Return403("Failed to retrieve branch information.");
                }

                // Retrieve all customer accounts
                var getCustomerAccountsCommand = new GetCustomerAccountsCommand { CustomerId = loanApplication.CustomerId };
                var getCustomerAccountsCommandResult = await _mediator.Send(getCustomerAccountsCommand, cancellationToken);

                if (getCustomerAccountsCommandResult.StatusCode != 200)
                {
                    string errorMessage = $"Failed to retrieve accounts for Customer ID {loanApplication.CustomerId}. Service returned: {getCustomerAccountsCommandResult.Message}";
                    _logger.LogError(errorMessage);

                    // Log the error with Base Utility Logger
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Forbidden,
                        LogAction.LoanApplicationApproval,
                        LogLevelInfo.Warning
                    );

                    return ServiceResponse<LoanApplicationDto>.Return403("Failed to retrieve customer accounts.");
                }

                var accounts = getCustomerAccountsCommandResult.Data;

                // Check if the customer has a loan account
                var loanAccount = accounts.FirstOrDefault(x => x.Product.AccountType.ToLower() == "loan");

                if (loanAccount == null)
                {
                    // Create a loan account if one does not exist
                    var addLoanAccount = new AddLoanAccountCommand
                    {
                        CustomerId = loanApplication.CustomerId,
                        BankId = customer.bankId,
                        BranchId = customer.branchId
                    };
                    var addLoanAccountResult = await _mediator.Send(addLoanAccount, cancellationToken);

                    if (addLoanAccountResult.StatusCode != 200)
                    {
                        string errorMessage = $"Failed to create a loan account for Customer ID {loanApplication.CustomerId}. Service returned: {addLoanAccountResult.Message}";
                        _logger.LogError(errorMessage);

                        // Log the error with Base Utility Logger
                        await BaseUtilities.LogAndAuditAsync(
                            errorMessage,
                            request,
                            HttpStatusCodeEnum.Forbidden,
                            LogAction.LoanApplicationApproval,
                            LogLevelInfo.Warning
                        );

                        return ServiceResponse<LoanApplicationDto>.Return403("Failed to create loan account. Please contact the account manager.");
                    }
                }



                if (request.ApprovalStatus == LoanApplicationStatus.Approved.ToString())
                {
                    // Step 11: Verify if the loan application is validated
                    if (loanApplication.ApprovalStatus == LoanApplicationStatusX.Validated.ToString())
                    {
                        // Step 12: Verify OTP code for security
                        var otpverifyCommand = new VerifyTemporalOTPCommand { UserId = loanApplication.CustomerId, OtpCode = request.OTPCode };
                        var otpVerificationResult = await _mediator.Send(otpverifyCommand);

                        if (otpVerificationResult.StatusCode != 200)
                        {
                            string errorMessage = $"OTP verification failed. The provided OTP code '{request.OTPCode}' is invalid. Please ensure the OTP is correct and try again.";
                            _logger.LogError(errorMessage);

                            await BaseUtilities.LogAndAuditAsync(
                                errorMessage,
                                request,
                                HttpStatusCodeEnum.Forbidden,
                                LogAction.LoanApplicationApproval,
                                LogLevelInfo.Warning
                            );

                            return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                        }

                        // Step 13: Check if the OTP is verified
                        if (otpVerificationResult.Data.IsVerify)
                        {
                            // Step 14: Proceed to create the loan
                            var addLoanCommand = new AddLoanCommand
                            {
                                LoanApplicationId = loanApplication.Id,
                                CustomerName = $"{customer.firstName} {customer.lastName}",
                                BranchCode = _userInfoToken.BranchCode,
                            };
                            var addLoanCommandResult = await _mediator.Send(addLoanCommand, cancellationToken);

                            if (addLoanCommandResult.StatusCode == 200)
                            {
                                // Step 15: Validate the loan application with supporting data
                                var loanGuarantors =  _loanGuarantorRepository.FindBy(x => x.LoanApplicationId == loanApplication.Id).ToList();
                                var documentAttachedToLoans =  _documentAttachedToLoanRepository.FindBy(x => x.LoanApplicationId == loanApplication.Id).ToList();
                                var loanApplicationCollaterals =  _loanCollateralRepository.FindBy(x => x.LoanApplicationId == loanApplication.Id).ToList();

                                _loanApplicationRepository.LoanApplicationValidator(
                                    loanApplication,
                                    product,
                                    loanGuarantors,
                                    loanApplicationCollaterals,
                                    documentAttachedToLoans
                                );

                                decimal loanAmount = 0;

                                // Determine the loan amount based on its type
                                if (loanApplication.LoanApplicationType != LoanApplicationTypes.Normal.ToString())
                                {
                                    loanApplication.RequestedAmount = loanApplication.Amount;
                                    loanApplication.RestructuredBalance = loanApplication.OldLoanAmount;
                                    loanApplication.Amount = addLoanCommandResult.Data.LoanAmount;
                                    loanAmount = addLoanCommandResult.Data.LoanAmount;
                                }
                                else
                                {
                                    loanAmount = addLoanCommandResult.Data.RequestedAmount;
                                    loanApplication.RequestedAmount = addLoanCommandResult.Data.RequestedAmount;
                                    loanApplication.RestructuredBalance = addLoanCommandResult.Data.RestructuredBalance;

                                    if (loanApplication.LoanApplicationType != LoanApplicationTypes.Refinancing.ToString())
                                    {
                                        loanApplication.Amount += addLoanCommandResult.Data.RestructuredBalance + addLoanCommandResult.Data.RequestedAmount;
                                    }
                                    else
                                    {
                                        loanApplication.Amount = addLoanCommandResult.Data.RestructuredBalance + addLoanCommandResult.Data.RequestedAmount;
                                    }
                                }

                                // Step 16: Update loan application with approval details
                                loanApplication.IsDisbursed = false;
                                loanApplication.IsApproved = true;
                                loanApplication.Status = LoanApplicationStatus.Approved.ToString();
                                loanApplication.ApprovalDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                                loanApplication.ApprovalStatus = LoanApplicationStatus.Approved.ToString();
                                loanApplication.DisbursementDate = BaseUtilities.UtcToDoualaTime(DateTime.MinValue);
                                loanApplication.ApprovalComment = request.ApprovalComment;
                                _loanApplicationRepository.Update(loanApplication);

                                // Credit the loan account if necessary
                                if (loanApplication.LoanApplicationType != LoanApplicationTypes.Normal.ToString() ||
                                    loanApplication.LoanApplicationType != LoanApplicationTypes.Refinancing.ToString())
                                {
                                    var creditLoanAccountCommand = new CreditLoanAccountCommand
                                    {
                                        Amount = loanApplication.Amount,
                                        BankId = customer.bankId,
                                        BranchId = customer.branchId,
                                        CustomerId = customer.customerId,
                                        ReferenceNumber = addLoanCommandResult.Data.Id,
                                        LoanApplicationType = loanApplication.LoanApplicationType,
                                        LoanProductName = product.ProductName,
                                        LoanProductId = product.Id
                                    };
                                    var creditLoanAccountResult = await _mediator.Send(creditLoanAccountCommand);

                                    if (creditLoanAccountResult.StatusCode != 200)
                                    {
                                        string errorMessage = $"Loan account crediting failed: {creditLoanAccountResult.Message}. Please try again or contact support.";
                                        _logger.LogError(errorMessage);

                                        await BaseUtilities.LogAndAuditAsync(
                                            errorMessage,
                                            creditLoanAccountCommand,
                                            HttpStatusCodeEnum.InternalServerError,
                                            LogAction.LoanApplicationApproval,
                                            LogLevelInfo.Warning
                                        );

                                        return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                                    }
                                }

                                // Step 18: Save changes to the database
                                await _unitOfWork.SaveAsync();

                                // Step 19: Send approval SMS notification
                                var approvalSms = ApprovalSMS(loanApplication, addLoanCommandResult.Data, customerPICallCommandResult.Data, branchPICallCommandResult.Data);
                                var approvalSmsResult = await _mediator.Send(approvalSms, cancellationToken);

                                string successMessage = $"Loan application ID {loanApplication.Id} has been successfully approved. " +
                                    $"Customer: {customer.firstName} {customer.lastName}, Loan Amount: {BaseUtilities.FormatCurrency(loanAmount)}, " +
                                    $"Loan Type: {loanApplication.LoanApplicationType}, Approval Date: {loanApplication.ApprovalDate:dd/MM/yyyy}. " +
                                    $"A confirmation SMS has been sent to the customer.";

                                _logger.LogInformation(successMessage);

                                // Log approval success with Utility Logger
                                await BaseUtilities.LogAndAuditAsync(
                                    successMessage,
                                    request,
                                    HttpStatusCodeEnum.OK,
                                    LogAction.LoanApplicationApproval,
                                    LogLevelInfo.Information
                                );

                                return ServiceResponse<LoanApplicationDto>.ReturnResultWith200(
                                    _mapper.Map<LoanApplicationDto>(loanApplication),
                                    successMessage
                                );
                            }
                            else
                            {
                                string errorMessage = $"Loan creation failed during the approval process: {addLoanCommandResult.Message}. Please try again or contact support.";
                                _logger.LogError(errorMessage);

                                await BaseUtilities.LogAndAuditAsync(
                                    errorMessage,
                                    request,
                                    HttpStatusCodeEnum.InternalServerError,
                                    LogAction.LoanApplicationApproval,
                                    LogLevelInfo.Warning
                                );

                                return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                            }
                        }
                        else
                        {
                            string errorMessage = $"The provided OTP code '{request.OTPCode}' was not found or has expired. Please request a new OTP and try again.";
                            _logger.LogError(errorMessage);

                            await BaseUtilities.LogAndAuditAsync(
                                errorMessage,
                                request,
                                HttpStatusCodeEnum.Forbidden,
                                LogAction.LoanApplicationApproval,
                                LogLevelInfo.Warning
                            );

                            return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                        }
                    }
                    else
                    {
                        string errorMessage = "Loan application requires validation from the board before proceeding with approval.";
                        _logger.LogError(errorMessage);

                        await BaseUtilities.LogAndAuditAsync(
                            errorMessage,
                            request,
                            HttpStatusCodeEnum.Forbidden,
                            LogAction.LoanApplicationApproval,
                            LogLevelInfo.Warning
                        );

                        return ServiceResponse<LoanApplicationDto>.Return403(errorMessage);
                    }
                }
                else
                {
                    return ServiceResponse<LoanApplicationDto>.ReturnResultWith200(
                        _mapper.Map<LoanApplicationDto>(loanApplication),
                        "Loan application status updated successfully."
                    );
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"An error occurred while processing the loan approval. Error: {ex.Message}";
                // 23. Log and return error response
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                           errorMessage,
                           request,
                           HttpStatusCodeEnum.InternalServerError,
                           LogAction.LoanApplicationApproval,
                           LogLevelInfo.Error
                       );
                return ServiceResponse<LoanApplicationDto>.Return500();
            }
        }


        private SendSMSPICallCommand ApprovalSMS(LoanApplication loanApplication, LoanDto loan, CustomerDto customer, BranchDto branchPICallCommandResult)
        {
            string bankName = branchPICallCommandResult.name;
            string msg;
            string accountCreditMessage = loanApplication.LoanApplicationType == "Refinancing" || loanApplication.LoanApplicationType == "Normal"
                ? "The approved amount will be credited to your account soon."
                : "No additional amount will be credited to your account.";

            if (customer.language.ToLower() == "english")
            {
                var loanTypeMessage = loanApplication.LoanApplicationType switch
                {
                    "Refinancing" => "Your loan has been successfully refinanced.",
                    "Reschedule" => "Your loan has been successfully rescheduled.",
                    "Restructure" => "Your loan has been successfully restructured.",
                    _ => "Your loan application has been approved."
                };

                msg = $"{customer.firstName} {customer.lastName}, {loanTypeMessage}\nLoan Details:\nAmount: {BaseUtilities.FormatCurrency(loanApplication.Amount)}\nDuration: {loanApplication.LoanDuration} Months\nNext Payment: {loan.FirstInstallmentDate}\nMaturity: {loan.MaturityDate}\n{accountCreditMessage}\nThank you for choosing {bankName}.\nFor assistance, contact us at {branchPICallCommandResult.bank.customerServiceContact}.";
            }
            else
            {
                var loanTypeMessage = loanApplication.LoanApplicationType switch
                {
                    "Refinancing" => "Votre prêt a été refinancé avec succès.",
                    "Reschedule" => "Votre prêt a été reprogrammé avec succès.",
                    "Restructure" => "Votre prêt a été restructuré avec succès.",
                    _ => "Votre demande de prêt a été approuvée."
                };

                msg = $"{customer.firstName} {customer.lastName}, {loanTypeMessage}\nDétails du prêt:\nMontant: {BaseUtilities.FormatCurrency(loanApplication.Amount)}\nDurée: {loanApplication.LoanDuration} mois\nProchain paiement: {loan.FirstInstallmentDate}\nMaturité: {loan.MaturityDate}\n{accountCreditMessage}\nMerci d'avoir choisi {bankName}.\nPour toute assistance, contactez-nous au {branchPICallCommandResult.bank.customerServiceContact}.";
            }

            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
            };
        }



        private SendSMSPICallCommand DeclineSMS(CustomerDto customer, BranchDto branchPICallCommandResult)
        {
            string bankName = branchPICallCommandResult.bank.name;
            string msg;

            if (customer.language.ToLower() == "english")
            {
                msg = $"Hello {customer.firstName} {customer.lastName}, We regret to inform you that your loan application has been declined.\nWe appreciate your interest in {bankName}.\nFor more information, contact customer service {branchPICallCommandResult.customerServiceContact}.\nThank you.\n";
            }
            else // Assuming if not English, it's French
            {
                msg = $"Bonjour {customer.firstName} {customer.lastName}, Nous regrettons de vous informer que votre demande de prêt a été refusée.\nNous apprécions votre intérêt pour {bankName}.\nPour plus d'informations, contactez le service clientèle {branchPICallCommandResult.customerServiceContact}.\nMerci.\n";
            }

            return new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
            };
        }

    }



}
