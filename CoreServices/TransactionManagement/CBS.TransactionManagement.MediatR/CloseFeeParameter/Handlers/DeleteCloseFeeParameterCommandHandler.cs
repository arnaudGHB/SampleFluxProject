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
    /// Handles the command to delete a CloseFeeParameter based on DeleteCloseFeeParameterCommand.
    /// </summary>
    public class DeleteCloseFeeParameterCommandHandler : IRequestHandler<DeleteCloseFeeParameterCommand, ServiceResponse<bool>>
    {
        private readonly ICloseFeeParameterRepository _CloseFeeParameterRepository; // Repository for accessing CloseFeeParameter data.
        private readonly ILogger<DeleteCloseFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteCloseFeeParameterCommandHandler.
        /// </summary>
        /// <param name="CloseFeeParameterRepository">Repository for CloseFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteCloseFeeParameterCommandHandler(
            ICloseFeeParameterRepository CloseFeeParameterRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteCloseFeeParameterCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _CloseFeeParameterRepository = CloseFeeParameterRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteCloseFeeParameterCommand to delete a CloseFeeParameter.
        /// </summary>
        /// <param name="request">The DeleteCloseFeeParameterCommand containing CloseFeeParameter ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteCloseFeeParameterCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the CloseFeeParameter entity with the specified ID exists
                var existingCloseFeeParameter = await _CloseFeeParameterRepository.FindAsync(request.Id);
                if (existingCloseFeeParameter == null)
                {
                    errorMessage = $"CloseFeeParameter with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingCloseFeeParameter.IsDeleted = true;
                _CloseFeeParameterRepository.Update(existingCloseFeeParameter);
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
                errorMessage = $"Error occurred while deleting CloseFeeParameter: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
