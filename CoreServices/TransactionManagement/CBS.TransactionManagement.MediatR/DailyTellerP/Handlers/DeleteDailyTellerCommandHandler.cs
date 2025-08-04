using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.DailyTellerP.Commands;

namespace CBS.TransactionManagement.DailyTellerP.Handlers
{
    /// <summary>
    /// Handles the command to delete a DailyTeller based on DeleteDailyTellerCommand.
    /// </summary>
    public class DeleteDailyTellerCommandHandler : IRequestHandler<DeleteDailyTellerCommand, ServiceResponse<bool>>
    {
        private readonly IDailyTellerRepository _DailyTellerRepository; // Repository for accessing DailyTeller data.
        private readonly ILogger<DeleteDailyTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteDailyTellerCommandHandler.
        /// </summary>
        /// <param name="DailyTellerRepository">Repository for DailyTeller data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDailyTellerCommandHandler(
            IDailyTellerRepository DailyTellerRepository, IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<DeleteDailyTellerCommandHandler> logger
, IUnitOfWork<TransactionContext> uow)
        {
            _DailyTellerRepository = DailyTellerRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDailyTellerCommand to delete a DailyTeller.
        /// </summary>
        /// <param name="request">The DeleteDailyTellerCommand containing DailyTeller ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDailyTellerCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DailyTeller entity with the specified ID exists
                var existingDailyTeller = await _DailyTellerRepository.FindAsync(request.Id);
                if (existingDailyTeller == null)
                {
                    errorMessage = $"DailyTeller with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Delete.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDailyTeller.IsDeleted = true;
                _DailyTellerRepository.Update(existingDailyTeller);
                await _uow.SaveAsync();
                string msg = $"{existingDailyTeller.UserName} was successfully removed or deleted from teller operations";
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<bool>.ReturnResultWith200(true, msg);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting DailyTeller: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
