using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the command to add a new TellerHistory.
    /// </summary>
    public class AddTellerHistoryCommandHandler : IRequestHandler<AddTellerHistoryCommand, ServiceResponse<PrimaryTellerProvisioningHistoryDto>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _TellerHistoryRepository; // Repository for accessing TellerHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTellerHistoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddTellerHistoryCommandHandler.
        /// </summary>
        /// <param name="TellerHistoryRepository">Repository for TellerHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTellerHistoryCommandHandler(
            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddTellerHistoryCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow)
        {
            _TellerHistoryRepository = TellerHistoryRepository;
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddTellerHistoryCommand to add a new TellerHistory.
        /// </summary>
        /// <param name="request">The AddTellerHistoryCommand containing TellerHistory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PrimaryTellerProvisioningHistoryDto>> Handle(AddTellerHistoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Map the AddTellerHistoryCommand to a TellerHistory entity
                var TellerHistoryEntity = _mapper.Map<PrimaryTellerProvisioningHistory>(request);
                // Convert UTC to local time and set it in the entity
                TellerHistoryEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                TellerHistoryEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new TellerHistory entity to the repository
                _TellerHistoryRepository.Add(TellerHistoryEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "An error occured creating a new TellerHistory", LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500();
                }
                // Map the TellerHistory entity to TellerHistoryDto and return it with a success response
                var TellerHistoryDto = _mapper.Map<PrimaryTellerProvisioningHistoryDto>(TellerHistoryEntity);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "TellerHistory created successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.ReturnResultWith200(TellerHistoryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving TellerHistory: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                return ServiceResponse<PrimaryTellerProvisioningHistoryDto>.Return500(e);
            }
        }
    }

}
