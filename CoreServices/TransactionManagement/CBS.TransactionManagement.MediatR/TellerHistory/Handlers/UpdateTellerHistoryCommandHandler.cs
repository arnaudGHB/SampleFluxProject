using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a TellerHistory based on UpdateTellerHistoryCommand.
    /// </summary>
    public class UpdateTellerHistoryCommandHandler : IRequestHandler<UpdateTransferCommand, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _TellerHistoryRepository; // Repository for accessing TellerHistory data.
        private readonly ILogger<UpdateTellerHistoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateTellerHistoryCommandHandler.
        /// </summary>
        /// <param name="TellerHistoryRepository">Repository for TellerHistory data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateTellerHistoryCommandHandler(
            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            ILogger<UpdateTellerHistoryCommandHandler> logger,
            UserInfoToken userInfoToken,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null)
        {
            _TellerHistoryRepository = TellerHistoryRepository;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateTellerHistoryCommand to update a TellerHistory.
        /// </summary>
        /// <param name="request">The UpdateTellerHistoryCommand containing updated TellerHistory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(UpdateTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the TellerHistory entity to be updated from the repository
                var existingTellerHistory = await _TellerHistoryRepository.FindAsync(request.Id);

                // Check if the TellerHistory entity exists
                if (existingTellerHistory != null)
                {
                    existingTellerHistory.EndOfDayAmount = request.EndOfDayAmount;
                    existingTellerHistory.CashAtHand = request.BalanceAtHand;
                    existingTellerHistory.ClossedDate = DateTime.Now;
                    existingTellerHistory.ModifiedDate = DateTime.Now;

                    // Use the repository to update the existing TellerHistory entity
                    _TellerHistoryRepository.Update(existingTellerHistory);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, "An error occurred updating TellerHistory", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                        return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 Status code
                    var response = ServiceResponse<PrimaryTellerProvisioningHistoryDto>.ReturnResultWith200(_mapper.Map<PrimaryTellerProvisioningHistoryDto>(existingTellerHistory));
                    _logger.LogInformation($"TellerHistory {request.Id} was successfully updated.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, $"TellerHistory {request.Id} was successfully updated.", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return response;
                }
                else
                {
                    // If the TellerHistory entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Update.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating TellerHistory: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(e);
            }
        }
    }

}
