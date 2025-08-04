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
    /// Handles the command to retrieve an account number from the accounting service.
    /// </summary>
    public class GetAccountNumberCommandHandler : IRequestHandler<GetAccountNumberCommand, ServiceResponse<GetAccountNumberResponse>>
    {
        private readonly ILogger<GetAccountNumberCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        public IMediator _mediator { get; set; }

        /// <summary>
        /// Initializes a new instance of the GetAccountNumberCommandHandler.
        /// </summary>
        public GetAccountNumberCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GetAccountNumberCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper,
            IMediator mediator)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetAccountNumberCommand to retrieve an account number.
        /// </summary>
        /// <param name="request">The command containing the ProductId for which the account number is requested.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<GetAccountNumberResponse>> Handle(GetAccountNumberCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Call the accounting service API to get the account number
                var serviceResponse = await APICallHelper.GetObjectById<ServiceResponse<GetAccountNumberResponse>>(
                    _pathHelper.AccountingBaseURL,
                    _pathHelper.GetAccountNumberURL,
                    _userInfoToken.Token,
                    request.ProductId
                );

                // Log the successful operation
                string successMessage = $"Successfully retrieved account number for Product ID: {request.ProductId}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.Read,
                    LogLevelInfo.Information,
                    request.ProductId
                );

                // Return the service response
                return ServiceResponse<GetAccountNumberResponse>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Log and audit the error
                string errorMessage = $"Error occurred while retrieving account number for Product ID: {request.ProductId}. Details: {e.Message}.";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.Read,
                    LogLevelInfo.Error,
                    request.ProductId
                );

                // Return a 500 Internal Server Error response
                return ServiceResponse<GetAccountNumberResponse>.Return500(e, errorMessage);
            }
        }
    }

}
