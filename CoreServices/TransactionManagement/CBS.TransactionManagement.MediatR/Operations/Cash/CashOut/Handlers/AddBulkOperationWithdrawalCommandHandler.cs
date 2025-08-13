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
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Commands;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashIn.Services.RemittanceP;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.RemittanceP;
using CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Services.NormalCashOutP;
using System.Text.Json;

namespace CBS.TransactionManagement.MediatR.Operations.Cash.CashOut.Handlers
{

    /// <summary>
    /// Handles bulk operation withdrawal commands.
    /// Manages the processing of withdrawal operations, including retrieving the current accounting day,
    /// fetching configuration settings, processing different withdrawal types (e.g., Normal, Remittance),
    /// and returning the appropriate response. If an error occurs, logs the error and returns a failure response.
    /// </summary>
    public class AddBulkOperationWithdrawalCommandHandler : IRequestHandler<AddBulkOperationWithdrawalCommand, ServiceResponse<PaymentReceiptDto>>
    {
        // Dependencies needed for various services and repositories
        private readonly UserInfoToken _userInfoToken; // User information for audit and access
        private readonly IConfigRepository _configRepository; // Configuration repository
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for retrieving accounting day
        private readonly ILogger<AddBulkOperationWithdrawalCommandHandler> _logger; // Logger for capturing logs
        private readonly IRemittanceCashoutServices _remittanceCashoutServices; // Remittance cashout services
        private readonly INormalCashoutServices _nomalCashoutServices; // Normal cashout services

        /// <summary>
        /// Initializes the handler with the required dependencies.
        /// </summary>
        /// <param name="userInfoToken">User information for auditing and branch-specific operations.</param>
        /// <param name="logger">Logger for capturing logs.</param>
        /// <param name="configRepository">Repository for configuration settings.</param>
        /// <param name="accountingDayRepository">Repository for retrieving the current accounting day.</param>
        /// <param name="remittanceCashoutServices">Service for handling remittance cashouts.</param>
        /// <param name="nomalCashoutServices">Service for handling normal cashouts.</param>
        public AddBulkOperationWithdrawalCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddBulkOperationWithdrawalCommandHandler> logger,
            IConfigRepository configRepository,
            IAccountingDayRepository accountingDayRepository,
            IRemittanceCashoutServices remittanceCashoutServices,
            INormalCashoutServices nomalCashoutServices)
        {
            // Initialize all services and repositories
            _userInfoToken = userInfoToken;
            _logger = logger;
            _configRepository = configRepository;
            _accountingDayRepository = accountingDayRepository;
            _remittanceCashoutServices = remittanceCashoutServices;
            _nomalCashoutServices = nomalCashoutServices;
        }

        /// <summary>
        /// Handles the bulk operation withdrawal command by processing the request and returning a response.
        /// </summary>
        /// <param name="request">The withdrawal command request containing details for bulk withdrawal operations.</param>
        /// <param name="cancellationToken">Token for monitoring cancellation requests.</param>
        /// <returns>A service response containing the payment receipt details or an error message if the operation fails.</returns>
        public async Task<ServiceResponse<PaymentReceiptDto>> Handle(AddBulkOperationWithdrawalCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve current accounting day
                var currentAccountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                // Fetch configuration settings based on source type
                var config = await _configRepository.GetConfigAsync(OperationSourceType.Web_Portal.ToString());

                // Process the withdrawal type through a switch statement for better readability
                var response = await ProcessDepositType(request, currentAccountingDay, config);

                // Deserialize the request object to a JSON string for logging
                var deserializedRequest = JsonSerializer.Serialize(request);
                _logger.LogInformation($"Withdrawal Request Details: {deserializedRequest}");

                return response;
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed Withdrawal. Error: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.WithdrawalProcessed, LogLevelInfo.Error,null);
                _logger.LogError(errorMessage);

                return ServiceResponse<PaymentReceiptDto>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Processes the withdrawal operation based on the type (Normal, Remittance).
        /// </summary>
        /// <param name="request">The withdrawal command request.</param>
        /// <param name="accountingDay">The current accounting day.</param>
        /// <param name="config">The configuration settings for the operation.</param>
        /// <returns>A service response containing the payment receipt details.</returns>
        private async Task<ServiceResponse<PaymentReceiptDto>> ProcessDepositType(AddBulkOperationWithdrawalCommand request, DateTime accountingDay, Config config)
        {
            ServiceResponse<PaymentReceiptDto> response = null;

            switch (request.OperationType)
            {
                case "Withdrawal":
                    response = await _nomalCashoutServices.Cashout(request, accountingDay, config);
                    break;
                case "RemittanceOUT":
                    response = await _remittanceCashoutServices.RemittanceCashout(request, accountingDay, config);
                    break;
            }

            return response;
        }
    }
}
