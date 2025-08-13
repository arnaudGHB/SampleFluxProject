using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository.ReopenFeeParameterP;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to delete a ReopenFeeParameter based on DeleteReopenFeeParameterCommand.
    /// </summary>
    public class DeleteReopenFeeParameterCommandHandler : IRequestHandler<DeleteReopenFeeParameterCommand, ServiceResponse<bool>>
    {
        private readonly IReopenFeeParameterRepository _ReopenFeeParameterRepository; // Repository for accessing ReopenFeeParameter data.
        private readonly ILogger<DeleteReopenFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteReopenFeeParameterCommandHandler.
        /// </summary>
        /// <param name="ReopenFeeParameterRepository">Repository for ReopenFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteReopenFeeParameterCommandHandler(
            IReopenFeeParameterRepository ReopenFeeParameterRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteReopenFeeParameterCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _ReopenFeeParameterRepository = ReopenFeeParameterRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteReopenFeeParameterCommand to delete a ReopenFeeParameter.
        /// </summary>
        /// <param name="request">The DeleteReopenFeeParameterCommand containing ReopenFeeParameter ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteReopenFeeParameterCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the ReopenFeeParameter entity with the specified ID exists
                var existingReopenFeeParameter = await _ReopenFeeParameterRepository.FindAsync(request.Id);
                if (existingReopenFeeParameter == null)
                {
                    errorMessage = $"ReopenFeeParameter with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingReopenFeeParameter.IsDeleted = true;
                _ReopenFeeParameterRepository.Update(existingReopenFeeParameter);
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
                errorMessage = $"Error occurred while deleting ReopenFeeParameter: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
