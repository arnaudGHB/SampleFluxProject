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
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to process Loan Repayment through Momo cash collection.
    /// </summary>
    public class MomocashCollectionLoanRepaymentCommandHandler : IRequestHandler<MomocashCollectionLoanRepaymentCommand, ServiceResponse<bool>>
    {
        private readonly ILogger<MomocashCollectionLoanRepaymentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow; // Unit of work for managing database transactions.
        private readonly UserInfoToken _userInfoToken; // Token containing user-specific information.
        private readonly PathHelper _pathHelper; // Helper for constructing API paths.
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository; // Repository for utility services operations.

        /// <summary>
        /// Constructor for initializing the MomocashCollectionLoanRepaymentCommandHandler.
        /// </summary>
        public MomocashCollectionLoanRepaymentCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<MomocashCollectionLoanRepaymentCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IAPIUtilityServicesRepository utilityServicesRepository)
        {
            _userInfoToken = userInfoToken; // Assign user info token
            _logger = logger; // Assign logger instance
            _uow = uow; // Assign unit of work
            _pathHelper = pathHelper; // Assign path helper
            _utilityServicesRepository = utilityServicesRepository; // Assign utility services repository
        }

        /// <summary>
        /// Handles the Loan Repayment through Momo cash collection.
        /// </summary>
        /// <param name="request">The command containing Loan Repayment data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(MomocashCollectionLoanRepaymentCommand request, CancellationToken cancellationToken)
        {
            // Filter out any zero amounts in the request
            var filteredRequest = FilterZeros(request);

            try
            {
                // Serialize filtered data for API call
                string serializedData = JsonConvert.SerializeObject(filteredRequest);

                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.AccountingPostingLoanRefundURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.MakeNonCashAccountAdjustmentCommand,
                    request.TransactionReferenceId,
                    request.TransactionDate,
                    destinationUrl);

                // Call the accounting API
                var apiResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<bool>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    serializedData,
                    _pathHelper.AccountingPostingLoanRefundURL
                );

                // Log and audit successful operation
                string successMessage = $"Loan Repayment (Momo cash collection) processed successfully. Operator: {request.MomoOperatorType}. Transaction Reference: {request.TransactionReferenceId}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.OK,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Information,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.ReturnResultWith200(apiResponse.Data, "Accounting posting was successful.");
            }
            catch (Exception ex)
            {
                // Log and audit error
                    string errorMessage = $"Error during Loan Repayment (Momo cash collection). Operator: {request.MomoOperatorType}. Transaction Reference: {request.TransactionReferenceId}. Error: {ex.Message}.";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    filteredRequest,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.AccountingPosting,
                    LogLevelInfo.Error,
                    request.TransactionReferenceId
                );

                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out zero-amount entries from the request.
        /// </summary>
        /// <param name="request">The original command request.</param>
        /// <returns>A new command with filtered data.</returns>
        private MomocashCollectionLoanRepaymentCommand FilterZeros(MomocashCollectionLoanRepaymentCommand request)
        {
            var filteredAmountCollections = request.AmountCollection?
                .Where(detail => Math.Abs(detail.Amount) > 0)
                .ToList();

            return new MomocashCollectionLoanRepaymentCommand
            {
                TransactionDate = request.TransactionDate,
                TransactionReferenceId = request.TransactionReferenceId,
                Amount = request.Amount,
                AmountCollection = filteredAmountCollections,
                LoanRefundCollectionAlpha = request.LoanRefundCollectionAlpha,
                AccountNumber = request.AccountNumber,
                BranchId = request.BranchId,
                IsOldSystemLoan = request.IsOldSystemLoan,
                LoanProductId = request.LoanProductId,
                Naration = request.Naration,
                TellerSource = request.TellerSource,
                MomoOperatorType = request.MomoOperatorType
            };
        }
    }

}
