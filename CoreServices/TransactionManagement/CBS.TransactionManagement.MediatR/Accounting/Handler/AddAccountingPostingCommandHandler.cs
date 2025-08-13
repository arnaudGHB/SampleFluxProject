using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Commands;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to process accounting postings for cash transactions.
    /// </summary>
    public class AddAccountingPostingCommandHandler : IRequestHandler<AddAccountingPostingCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<AddAccountingPostingCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IAccountingPostingConfirmationRepository _accountingPostingConfirmationRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the AddAccountingPostingCommandHandler.
        /// </summary>
        public AddAccountingPostingCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddAccountingPostingCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAccountingPostingConfirmationRepository accountingPostingConfirmationRepository = null)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _accountingPostingConfirmationRepository = accountingPostingConfirmationRepository;
        }

        /// <summary>
        /// Handles the accounting posting request for cash operations.
        /// </summary>
        public async Task<ServiceResponse<bool>> Handle(AddAccountingPostingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Filter amounts and events with non-zero values
                var filteredRequest = FilterZeros(request);

                // Convert filtered request to JSON
                string requestData = JsonConvert.SerializeObject(filteredRequest);

                // Call the accounting API
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.AccountingPostingURL
                );

                // Log and audit success
                string successMessage = $"Accounting posting successful for Cash {request.OperationType}. Transaction Reference: {request.TransactionReferenceId}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountingPosting, LogLevelInfo.Information, request.TransactionReferenceId);

                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, "Accounting posting was successful.");
            }
            catch (Exception ex)
            {
                // Serialize request data for debugging
                string requestData = JsonConvert.SerializeObject(request);

                // Construct error message
                string errorMessage = $"Error during accounting posting for Cash {request.OperationType}. Transaction Reference: {request.TransactionReferenceId}. Details: {ex.Message}";

                // Log and audit error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingPosting, LogLevelInfo.Error, request.TransactionReferenceId);

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out zero or insignificant amounts from the request.
        /// </summary>
        private AddAccountingPostingCommand FilterZeros(AddAccountingPostingCommand request)
        {
            return new AddAccountingPostingCommand
            {
                AmountCollection = request.AmountCollection.Where(item => Math.Abs(item.Amount) > 0).ToList(),
                AmountEventCollections = request.AmountEventCollections.Where(item => Math.Abs(item.Amount) > 0).ToList(),
                Source = request.Source,
                ExternalBranchCode = request.ExternalBranchCode,
                ProductId = request.ProductId,
                AccountNumber = request.AccountNumber,
                AccountHolder = request.AccountHolder,
                ExternalBranchId = request.ExternalBranchId,
                IsInterBranchTransaction = request.IsInterBranchTransaction,
                Naration = request.Naration,
                OperationType = request.OperationType,
                ProductName = request.ProductName,
                TransactionDate = request.TransactionDate,
                TransactionReferenceId = request.TransactionReferenceId
            };
        }
    }

}
