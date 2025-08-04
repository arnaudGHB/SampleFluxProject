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

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to retrieve branch information by branch ID.
    /// </summary>
    public class GetBranchByIdCommandHandler : IRequestHandler<GetBranchByIdCommand, ServiceResponse<BranchDto>>
    {
        private readonly ILogger<GetBranchByIdCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the GetBranchByIdCommandHandler.
        /// </summary>
        public GetBranchByIdCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GetBranchByIdCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _uow = uow;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetBranchByIdCommand to retrieve branch details.
        /// </summary>
        /// <param name="request">The command containing the branch ID for which information is requested.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BranchDto>> Handle(GetBranchByIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Call the branch API to get the branch details
                var serviceResponse = await APICallHelper.GetBranch<ServiceResponse<BranchDto>>(
                    _pathHelper,
                    request.BranchId,
                    _userInfoToken.Token
                );

                // Log and audit successful operation
                string successMessage = $"Successfully retrieved branch details for Branch ID: {request.BranchId}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.Branch,
                    LogLevelInfo.Information,
                    request.BranchId
                );

                // Return the response
                return ServiceResponse<BranchDto>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Log and audit error
                string errorMessage = $"Error occurred while retrieving branch details for Branch ID: {request.BranchId}. Details: {e.Message}.";
                _logger.LogError(errorMessage);

                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.Branch,
                    LogLevelInfo.Error,
                    request.BranchId
                );

                // Return a 500 Internal Server Error response with the error message
                return ServiceResponse<BranchDto>.Return500(e, errorMessage);
            }
        }
    }
}
