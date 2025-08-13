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
    /// Handles the command to delete a EntryFeeParameter based on DeleteEntryFeeParameterCommand.
    /// </summary>
    public class DeleteEntryFeeParameterCommandHandler : IRequestHandler<DeleteEntryFeeParameterCommand, ServiceResponse<bool>>
    {
        private readonly IEntryFeeParameterRepository _EntryFeeParameterRepository; // Repository for accessing EntryFeeParameter data.
        private readonly ILogger<DeleteEntryFeeParameterCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteEntryFeeParameterCommandHandler.
        /// </summary>
        /// <param name="EntryFeeParameterRepository">Repository for EntryFeeParameter data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteEntryFeeParameterCommandHandler(
            IEntryFeeParameterRepository EntryFeeParameterRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteEntryFeeParameterCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _EntryFeeParameterRepository = EntryFeeParameterRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteEntryFeeParameterCommand to delete a EntryFeeParameter.
        /// </summary>
        /// <param name="request">The DeleteEntryFeeParameterCommand containing EntryFeeParameter ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteEntryFeeParameterCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the EntryFeeParameter entity with the specified ID exists
                var existingEntryFeeParameter = await _EntryFeeParameterRepository.FindAsync(request.Id);
                if (existingEntryFeeParameter == null)
                {
                    errorMessage = $"EntryFeeParameter with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingEntryFeeParameter.IsDeleted = true;
                _EntryFeeParameterRepository.Update(existingEntryFeeParameter);
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
                errorMessage = $"Error occurred while deleting EntryFeeParameter: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
