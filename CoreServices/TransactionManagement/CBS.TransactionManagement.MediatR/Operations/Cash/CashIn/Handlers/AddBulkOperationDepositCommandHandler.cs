using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Commands;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.NewFolder;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.RemittanceP;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.RemittanceP;
using System.Text.Json;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Handlers
{

    public class AddBulkOperationDepositCommandHandler : IRequestHandler<AddBulkOperationDepositCommand, ServiceResponse<PaymentReceiptDto>>
    {
        // Dependencies needed for various services and repositories
        private readonly UserInfoToken _userInfoToken; // User information for audit and access
        private readonly IConfigRepository _configRepository; // Configuration repository
        private readonly ILoanProcessingFeeServices _loanProcessingFeeServices; // Loan processing fee services
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for retrieving accounting day
        private readonly INormalDepositServices _normalDepositServices; // Normal deposit services
        private readonly IMomokashCollectionServices _momokashCollectionServices; // Momokash collection services
        private readonly IMomokashCollectionLoanRepaymentServices _momokashCollectionLoanRepaymentServices; // Momokash loan repayment services
        private readonly ILoanRepaymentOperationServices _loanRepaymentServices; // Loan repayment services
        private readonly ILogger<AddBulkOperationDepositCommandHandler> _logger; // Logger for capturing logs
        private readonly IRemittanceCashInServices _remittanceCashInServices; // Momokash loan repayment services

        // Constructor for injecting dependencies
        public AddBulkOperationDepositCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddBulkOperationDepositCommandHandler> logger,
            IConfigRepository configRepository,
            IAccountingDayRepository accountingDayRepository,
            ILoanRepaymentOperationServices loanRepaymentServices,
            INormalDepositServices normalDepositServices,
            ILoanProcessingFeeServices loanProcessingFeeServices,
            IMomokashCollectionServices momokashCollectionServices,
            IMomokashCollectionLoanRepaymentServices momokashCollectionLoanRepaymentServices,
            IRemittanceCashInServices remittanceCashInServices)
        {
            // Initialize all services and repositories
            _userInfoToken = userInfoToken;
            _logger = logger;
            _configRepository = configRepository;
            _accountingDayRepository = accountingDayRepository;
            _loanRepaymentServices = loanRepaymentServices;
            _normalDepositServices = normalDepositServices;
            _loanProcessingFeeServices = loanProcessingFeeServices;
            _momokashCollectionServices = momokashCollectionServices;
            _momokashCollectionLoanRepaymentServices = momokashCollectionLoanRepaymentServices;
            _remittanceCashInServices=remittanceCashInServices;
        }

        /// <summary>
        /// Handles the processing of bulk operation deposit commands.
        /// This includes retrieving the current accounting day, fetching configuration settings, processing deposit types,
        /// and logging the request details. If an error occurs during the operation, it logs the error and returns a failure response.
        /// </summary>
        /// <param name="request">The command request containing details for bulk operation deposits.</param>
        /// <param name="cancellationToken">Token for monitoring cancellation requests.</param>
        /// <returns>A service response containing the payment receipt details or an error message if the operation fails.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> Handle(AddBulkOperationDepositCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve current accounting day
                var currentAccountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Fetch configuration settings based on source type
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

                // Process the deposit type through a switch statement for better readability
                var response = await ProcessDepositType(request, currentAccountingDay, config);

                // Deserialize the request object to a JSON string for logging
                var deserializedRequest = JsonSerializer.Serialize(request);
                _logger.LogInformation($"Cashin Request Details: {deserializedRequest}");

                return response;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed Cashin. Error: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.DepositProcessed, LogLevelInfo.Error,null);
                _logger.LogError(errorMessage);
                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }


        // Process the deposit based on the deposit type (Normal, LoanRepayment, etc.)
        private async Task<ServiceResponse<PaymentReceiptDto>> ProcessDepositType(AddBulkOperationDepositCommand request, DateTime accountingDay, Config config)
        {
            ServiceResponse<PaymentReceiptDto> response = null;

            switch (request.DepositType)
            {
                case "Normal":
                    response = await _normalDepositServices.MakeDeposit(request, accountingDay, config);
                    break;
                case "RemittanceIN":
                    response = await _remittanceCashInServices.RemittanceCashIn(request, accountingDay, config);
                    break;
                case "LoanRepayment":
                    response = await _loanRepaymentServices.LoanRepayment(request, accountingDay, config);
                    break;
                case "LoanFeePayment":
                    response = await _loanProcessingFeeServices.LoanProcessingFeePayment(request, accountingDay, config);
                    break;
                case "CashInMomocashCollection":
                    response = await _momokashCollectionServices.MomokashCollection(request, accountingDay);
                    break;
                case "LoanRepaymentMomocashCollection":
                    response = await _momokashCollectionLoanRepaymentServices.LoanRepaymentMomokashCollection(request, accountingDay);
                    break;
                default:
                    _logger.LogError($"Invalid deposit type: {request.DepositType}");
                    break;
            }

            return response;
        }

      
    }
}
