using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.BankP;
using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Data.Helper;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.BankP.Command;
using CBS.NLoan.MediatR.Command;
using CBS.NLoan.MediatR.CustomerP.Command;
using CBS.NLoan.MediatR.CustomerP.Query;
using CBS.NLoan.MediatR.LoanMediaR.Commands;
using CBS.NLoan.MediatR.SMS.Command;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.DisburstLoan;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    public class AddLoanDisbumentCommandHandler : IRequestHandler<AddLoanDisbumentCommand, ServiceResponse<bool>>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMediator _mediator;
        private readonly ILoanApplicationFeeRepository _loanApplicationFeeRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IDisburstedLoanRepository _disburstedLoanRepository;
        private readonly ILogger<AddLoanDisbumentCommandHandler> _logger;
        private readonly IUnitOfWork<LoanContext> _unitOfWork;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILoanProductRepository _loanProductRepository;


        /// <summary>
        /// Constructor for initializing the AddLoanCommandHandler.
        /// </summary>
        public AddLoanDisbumentCommandHandler(
            ILoanRepository loanRepository,

            ILogger<AddLoanDisbumentCommandHandler> logger,
            IUnitOfWork<LoanContext> unitOfWork,
            IDisburstedLoanRepository disburstedLoanRepository = null,
            UserInfoToken userInfoToken = null,
            IMediator mediator = null,
            ILoanApplicationFeeRepository loanApplicationFeeRepository = null,
            ILoanApplicationRepository loanApplicationRepository = null,
            ILoanProductRepository loanProductRepository = null)
        {
            _loanRepository = loanRepository ?? throw new ArgumentNullException(nameof(loanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _disburstedLoanRepository = disburstedLoanRepository;
            _userInfoToken = userInfoToken;
            _mediator = mediator;
            _loanApplicationFeeRepository = loanApplicationFeeRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _loanProductRepository = loanProductRepository;
        }

        /// <summary>
        /// Handles the AddLoanCommand to add a new Loan.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(AddLoanDisbumentCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Step 1: Retrieve the loan without tracking to avoid unintended updates
                var loan = _loanRepository.FindBy(x => x.Id == request.LoanId).AsNoTracking().FirstOrDefault();
                if (loan == null)
                {
                    var errorMessage = $"Loan with id {request.LoanId} does not exist.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                // Step 2: Retrieve the associated loan application and fees without tracking
                var loanApplication = await _loanApplicationRepository.FindBy(x => x.Id == loan.LoanApplicationId).AsNoTracking().FirstOrDefaultAsync();
                var loanApplicationFees = await _loanApplicationFeeRepository.FindBy(x => x.LoanApplicationId == loan.LoanApplicationId && x.IsPaid == false).AsNoTracking().ToListAsync();
                var loanProduct = await _loanProductRepository.FindBy(x => x.Id == loanApplication.LoanProductId).AsNoTracking().FirstOrDefaultAsync();
                if (loanApplication.BranchId != _userInfoToken.BranchID)
                {
                    string errorMessage =
                        "Loan disbursements can only be processed in the branch where the loan application was initiated & approved. " +
                        "Please ensure that loan disbursements are handled by the originating branch.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Perform necessary checks as before...
                if (loan.IsLoanDisbursted)
                {
                    var errorMessage = "The loan has already been disbursed, and no further disbursement actions can be taken.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }

                // Step 2: Retrieve customer information
                var customerPICallCommandResult = await _mediator.Send(new GetCustomerCallCommand { CustomerId = loanApplication.CustomerId }, cancellationToken);
                if (customerPICallCommandResult.StatusCode != 200)
                {
                    var errorMessage = $"Failed to retrieve member's information for CustomerId: {loanApplication.CustomerId}. Status code: {customerPICallCommandResult.StatusCode}.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                var customer = customerPICallCommandResult.Data;

                // Step 3: Retrieve branch information
                var branchPICallCommandResult = await _mediator.Send(new BranchPICallCommand { BranchId = loanApplication.BranchId }, cancellationToken);
                if (branchPICallCommandResult.StatusCode != 200)
                {
                    var errorMessage = $"Failed to retrieve branch information for BranchId: {customer.branchId}. Status code: {branchPICallCommandResult.StatusCode}.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return403(errorMessage);
                }
                var branch = branchPICallCommandResult.Data;

                // If everything is correct, perform updates
                loan.DisbursmentStatus = request.Status;
                loan.IsLoanDisbursted = request.Status == DisbursmentStatus.Disbursed.ToString();
                loan.DisbursementDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                loan.LoanStatus = LoanStatus.Open.ToString();
                loanApplication.IsDisbursed = true;
                loanApplication.DisburstmentType = "Cash";
                loanApplication.DisbursementDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                if (loanProduct.StartGeneratingInterestAfterDisbustment)
                {
                    loan.LastInterestCalculatedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                }
                loan.LastCalculatedInterest = 0;

                // Now proceed to add the disbursed loan to the repository
                var disbursedLoan = new DisburstedLoan
                {
                    LoanId = loan.Id,
                    DisbursmentDate = loan.DisbursementDate,
                    DisbursedBy = _userInfoToken.FullName,
                    DisbursementStatus = request.Status,
                    Comment = request.Comment,
                    Id = BaseUtilities.GenerateUniqueNumber()
                };


                // If status is Disbursed, handle fees and disbursement command
                if (request.Status == DisbursmentStatus.Disbursed.ToString())
                {
                    var feeCollections = new List<ChargCollection>();
                    // Check if any EventCode is null
                    if (loanApplicationFees.Any(fee => fee.EventCode == null))
                    {
                        throw new InvalidOperationException("One or more fees have a null EventCode.");
                    }

                    // Map the fees to feeCollections
                    feeCollections = loanApplicationFees.Select(fee => new ChargCollection
                    {
                        Amount = fee.FeeAmount,
                        ChargeName = fee.FeeLable,
                        EventCode = fee.EventCode
                    }).ToList();
                    var oldLoanPayment = new OldLoanPayment { Amount=loanApplication.OldLoanAmount, Capital=loanApplication.OldLoanCapital, Interest=loanApplication.OldLoanInterest, LoanId=loanApplication.LoanId, Penalty=loanApplication.OldLoanPenalty, VAT=loanApplication.OldLoanVat };

                    var makeDisbursementRequest = new MakeDisbursmentCommand
                    {
                        Amount = loanApplication.Amount,
                        ReceiverAccountNumber = request.AccountNumber,
                        CustomerId = loan.CustomerId,
                        Note = request.Comment, 
                        LoanId = loan.Id,
                        IsNormal = loanApplication.LoanApplicationType == LoanApplicationTypes.Normal.ToString() ? true : false,
                        LoanApplicationType = loanApplication.LoanApplicationType,
                        RequestedAmount = loanApplication.RequestedAmount,
                        LoanProductId = loanProduct.Id, OldLoanPayment=oldLoanPayment,
                        RestructuredBalance = loanApplication.RestructuredBalance,
                        IsChargeInclussive = loanApplication.IsPaidFeeAfterProcessing == false ? true : false,
                        ChargCollections = feeCollections,
                    };

                    if (!request.IsAlreadyManage)
                    {
                        var makeDisbursementRequestResult = await _mediator.Send(makeDisbursementRequest);
                        if (makeDisbursementRequestResult.StatusCode != 200)
                        {
                            return ServiceResponse<bool>.Return403(makeDisbursementRequestResult.Message);
                        }
                    }
                    else
                    {
                        loan.LastInterestCalculatedDate = BaseUtilities.UtcNowToDoualaTime();
                    }
                }
                loanApplication.LoanProduct = null;
                loanApplication.LoanPurpose = null;
                // Only update and save changes if no errors occurred
                _loanApplicationRepository.Update(loanApplication);
                _loanRepository.Update(loan);
                _disburstedLoanRepository.Add(disbursedLoan);
                await _unitOfWork.SaveAsync();
                SendSMS(customer, branch, request.AccountNumber, request.Status);

                return ServiceResponse<bool>.ReturnResultWith200(true, "Loan disbursement has been successfully recorded.");

            }
            catch (Exception e)
            {
                // Log error and rollback the transaction
                _logger.LogError($"Error occurred while creating Loan: {e.Message}");
                return ServiceResponse<bool>.Return500(e, $"Error occurred while creating Loan: {e.Message}");
            }
        }
        private async Task SendSMS(CustomerDto customer, BranchDto branch, string accountNumber, string Status)
        {
            string msg = null;
            if (Status == DisbursmentStatus.Disbursed.ToString())
            {
                msg = GenerateLoanDisbursementMessage(customer, branch, accountNumber);
            }
            else
            {
                msg = GenerateLoanDisbursementCancellationMessage(customer, branch);

            }
            var sMSPICallCommand = new SendSMSPICallCommand
            {
                messageBody = msg,
                recipient = customer.phone
            };
            // Send command to mediator
            await _mediator.Send(sMSPICallCommand);
        }
        private string GenerateLoanDisbursementMessage(CustomerDto customer, BranchDto branch, string accountNumber)
        {
            string bankName = branch.bank.name;
            string language = customer.language.ToLower();
            string msg;

            if (language == "english")
            {
                msg = $"Dear {customer.firstName} {customer.lastName},\n" +
                      $"Your loan has been disbursed to your account ending in {accountNumber}.\n" +
                      $"Thank you for choosing {bankName}. For assistance, contact us at {branch.customerServiceContact}.";
            }
            else // French
            {
                msg = $"Cher {customer.firstName} {customer.lastName},\n" +
                      $"Votre prêt a été versé sur votre compte se terminant par {accountNumber}.\n" +
                      $"Merci d'avoir choisi {bankName}. Pour toute assistance, contactez-nous au {branch.customerServiceContact}.";
            }

            return msg;
        }
        private string GenerateLoanDisbursementCancellationMessage(CustomerDto customer, BranchDto branch)
        {
            string bankName = branch.bank.name;
            string language = customer.language.ToLower();
            string msg;

            if (language == "english")
            {
                msg = $"Dear {customer.firstName} {customer.lastName},\n" +
                      $"We regret to inform you that your loan application has been cancelled. For any questions, please contact us at {branch.customerServiceContact}.";
            }
            else // French
            {
                msg = $"Cher {customer.firstName} {customer.lastName},\n" +
                      $"Nous regrettons de vous informer que votre demande de prêt a été annulée. Pour toute question, veuillez nous contacter au {branch.customerServiceContact}.";
            }

            return msg;
        }

    }

}
//{
//    "transactionReferenceId": "string",
//  "accountNumber": "string",
//  "accountHolder": "string",
//  "savingProductId": "string",
//  "loanProductId": "string",
//  "savingProductName": "string",
//  "branchId": "string",
//  "naration": "string",
//  "amount": 0,
//  "transactionDate": "2024-12-06T16:15:22.183Z"
//}
