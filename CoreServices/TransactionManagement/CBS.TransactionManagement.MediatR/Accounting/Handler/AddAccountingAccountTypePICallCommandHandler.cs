using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Command;
using Newtonsoft.Json;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Commands;
using AutoMapper.Internal;
using CBS.TransactionManagement.MediatR.UtilityServices;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to create a new Accounting Account Type via API call.
    /// </summary>
    public class AddAccountingAccountTypePICallCommandHandler : IRequestHandler<AddAccountingAccountTypePICallCommand, ServiceResponse<AddAccountingAccountTypeResponse>>
    {
        private readonly ILogger<AddAccountingAccountTypePICallCommandHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IAPIUtilityServicesRepository _utilityServicesRepository;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Constructor for initializing the AddAccountingAccountTypePICallCommandHandler.
        /// </summary>
        public AddAccountingAccountTypePICallCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<AddAccountingAccountTypePICallCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IMediator mediator,
            IAPIUtilityServicesRepository utilityServicesRepository)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _mediator = mediator;
            _utilityServicesRepository=utilityServicesRepository;
        }

        /// <summary>
        /// Handles the AddAccountingAccountTypePICallCommand by making an API call to create an accounting account type.
        /// </summary>
        public async Task<ServiceResponse<AddAccountingAccountTypeResponse>> Handle(AddAccountingAccountTypePICallCommand request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber();  // Generate unique reference for tracking logs and audits
            try
            {
                // 1. Filter out null ChartOfAccountId entries from the provided details
                var filteredDetails = FilterNullChartOfAccountIds(request.ProductAccountBookDetails);

                if (!filteredDetails.Any())
                {
                    string errorMessage = "No valid Chart of Account IDs found in the request.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.AddAccountingAccountType, LogLevelInfo.Error, logReference);
                    return ServiceResponse<AddAccountingAccountTypeResponse>.Return400(errorMessage);
                }
                // Serialize the filtered request data for logging and API purposes
                string serializedData = JsonConvert.SerializeObject(filteredDetails);
                // Construct the destination URL for the API call
                string destinationUrl = $"{_pathHelper.AccountingBaseURL}{_pathHelper.MakeBulkAccountPostingURL}";

                // Log the serialized request data to the utility services repository
                await _utilityServicesRepository.CreatAccountingRLog(
                    serializedData,
                    CommandDataType.AddAccountingAccountType,
                    request.ProductAccountBookId,
                   BaseUtilities.UtcNowToDoualaTime(),
                    destinationUrl);

                // 2. Create a new command with the filtered details
                var commandWithFilteredData = CreateFilteredCommand(request, filteredDetails);

                // 3. Serialize the request data for logging and API calls
                string requestData = JsonConvert.SerializeObject(commandWithFilteredData);
                _logger.LogInformation($"Serialized Request Data: {requestData}");

                // 4. Make an API call to the accounting service
                var serviceResponse = await APICallHelper.AccountingAPICalls<ServiceResponse<AddAccountingAccountTypeResponse>>(
                    _pathHelper,
                    _userInfoToken.Token,
                    requestData,
                    _pathHelper.AccountingAccoutTypeCreateURL
                );

                // 5. If the API call is successful, log and audit success
                string successMessage = $"Accounting account type created successfully for Product ID: {request.ProductAccountBookId}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AddAccountingAccountType, LogLevelInfo.Information, logReference);

                // 6. Return a 200 OK response with the result from the accounting service
                return ServiceResponse<AddAccountingAccountTypeResponse>.ReturnResultWith200(serviceResponse.Data, successMessage);
            }
            catch (Exception ex)
            {
                // 7. Handle and log any exceptions during the process
                string errorMessage = $"Error occurred while creating accounting account type: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AddAccountingAccountType, LogLevelInfo.Error, logReference);

                // 8. Return a 500 Internal Server Error response with the exception message
                return ServiceResponse<AddAccountingAccountTypeResponse>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Filters out null ChartOfAccountId from the product account book details.
        /// </summary>
        private List<ProductAccountBookDetail> FilterNullChartOfAccountIds(IEnumerable<ProductAccountBookDetail> details)
        {
            return details.Where(detail => !string.IsNullOrEmpty(detail.ChartOfAccountId)).ToList();
        }

        /// <summary>
        /// Creates a new command with filtered product account book details.
        /// </summary>
        private AddAccountingAccountTypePICallCommand CreateFilteredCommand(AddAccountingAccountTypePICallCommand originalRequest, List<ProductAccountBookDetail> filteredDetails)
        {
            return new AddAccountingAccountTypePICallCommand
            {
                ProductAccountBookDetails = filteredDetails,
                ProductAccountBookId = originalRequest.ProductAccountBookId,
                ProductAccountBookName = originalRequest.ProductAccountBookName,
                ProductAccountBookType = originalRequest.ProductAccountBookType
            };
        }

       
    }

}
