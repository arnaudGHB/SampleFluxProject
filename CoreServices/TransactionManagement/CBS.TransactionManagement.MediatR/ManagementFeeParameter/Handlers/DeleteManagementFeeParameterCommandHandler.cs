using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to delete a ManagementFeeParameter based on DeleteManagementFeeParameterCommand.
    /// </summary>
    public class DeleteManagementFeeParameterCommandHandler : IRequestHandler<DeleteManagementFeeParameterCommand, ServiceResponse<bool>>
    {
        private readonly IManagementFeeParameterRepository _ManagementFeeParameterRepository; // Repository for accessing ManagementFeeParameter data.
        private readonly ILogger<DeleteManagementFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteManagementFeeParameterCommandHandler.
        /// </summary>
        /// <param name="ManagementFeeParameterRepository">Repository for ManagementFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteManagementFeeParameterCommandHandler(
            IManagementFeeParameterRepository ManagementFeeParameterRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteManagementFeeParameterCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _ManagementFeeParameterRepository = ManagementFeeParameterRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteManagementFeeParameterCommand to delete a ManagementFeeParameter.
        /// </summary>
        /// <param name="request">The DeleteManagementFeeParameterCommand containing ManagementFeeParameter ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteManagementFeeParameterCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the ManagementFeeParameter entity with the specified ID exists
                var existingManagementFeeParameter = await _ManagementFeeParameterRepository.FindAsync(request.Id);
                if (existingManagementFeeParameter == null)
                {
                    errorMessage = $"ManagementFeeParameter with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingManagementFeeParameter.IsDeleted = true;
                _ManagementFeeParameterRepository.Update(existingManagementFeeParameter);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "An error occurred deleting management fee", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return500();
                }
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Management fee deleted successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting ManagementFeeParameter: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
