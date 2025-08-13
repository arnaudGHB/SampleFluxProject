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
    /// Handles the command to retrieve all branch details.
    /// </summary>
    public class GetBranchesCommandHandler : IRequestHandler<GetBranchesCommand, ServiceResponse<List<BranchDto>>>
    {
        private readonly ILogger<GetBranchesCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;

        /// <summary>
        /// Constructor for initializing the GetBranchesCommandHandler.
        /// </summary>
        public GetBranchesCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<GetBranchesCommandHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetBranchesCommand to retrieve all branch details.
        /// </summary>
        /// <param name="request">The command requesting branch details.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BranchDto>>> Handle(GetBranchesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Call the API to fetch all branches
                var serviceResponse = await APICallHelper.GetAsynch<ServiceResponse<List<BranchDto>>>(
                    _pathHelper.BankConfigurationBaseUrl,
                    _pathHelper.GetAllBranches,
                    _userInfoToken.Token
                );

                // Log and audit the successful operation
                string successMessage = "Successfully retrieved all branch details.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.Branch,
                    LogLevelInfo.Information
                );

                // Return the response
                return ServiceResponse<List<BranchDto>>.ReturnResultWith200(serviceResponse.Data);
            }
            catch (Exception e)
            {
                // Construct and log error details
                string errorMessage = $"Error occurred while retrieving all branch details: {e.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the failure
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.Branch,
                    LogLevelInfo.Error
                );

                // Return a 500 Internal Server Error response with the error message
                return ServiceResponse<List<BranchDto>>.Return500(errorMessage);
            }
        }
    }

}
