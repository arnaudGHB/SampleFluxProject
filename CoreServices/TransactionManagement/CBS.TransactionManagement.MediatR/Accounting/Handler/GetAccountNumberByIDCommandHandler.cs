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
using CBS.TransactionManagement.MediatR.Accounting.Queries;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to retrieve an account number by its ID from the accounting service.
    /// </summary>
    public class GetAccountNumberByIDCommandHandler : IRequestHandler<GetAccountNumberByIDCommand, ServiceResponse<GetAccountNumberByIdResponse>>
    {
        private readonly ILogger<GetAccountNumberByIDCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the GetAccountNumberByIDCommandHandler.
        /// </summary>
        public GetAccountNumberByIDCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GetAccountNumberByIDCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetAccountNumberByIDCommand to retrieve an account number by ID.
        /// </summary>
        /// <param name="request">The command containing the ID for which the account number is requested.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetAccountNumberByIdResponse>> Handle(GetAccountNumberByIDCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Call the accounting service API to get the account number by ID
                var serviceResponse = await APICallHelper.GetObjectById<ServiceResponse<GetAccountNumberByIdResponse>>(
                    _pathHelper.AccountingBaseURL,
                    _pathHelper.GetAccountNumberByIdURL,
                    _userInfoToken.Token,
                    request.Id
                );

                // Log the successful operation
                string successMessage = $"Successfully retrieved account number for ID: {request.Id}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.Read,
                    LogLevelInfo.Information,
                    request.Id
                );

                // Return the service response
                return ServiceResponse<GetAccountNumberByIdResponse>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Log and audit the error
                string errorMessage = $"Error occurred while retrieving account number for ID: {request.Id}. Details: {e.Message}.";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.Read,
                    LogLevelInfo.Error,
                    request.Id
                );

                // Return a 500 Internal Server Error response
                return ServiceResponse<GetAccountNumberByIdResponse>.Return500(e, errorMessage);
            }
        }
    }

}
