using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;
using CBS.BankingZoneMGT.MediatR.Commands;
using CBS.BankMGT.Data;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a BankingZone based on UpdateBankingZoneCommand.
    /// </summary>
    public class UpdateBankingZoneCommandHandler : IRequestHandler<UpdateBankingZoneCommand, ServiceResponse<BankingZoneDto>>
    {
  
        private readonly IBankingZoneRepository _BankingZoneRepository; // Repository for accessing BankingZone data.
        private readonly ILogger<UpdateBankingZoneCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;
        /// <summary>
        /// Constructor for initializing the UpdateBankingZoneCommandHandler.
        /// </summary>
        /// <param name="BankingZoneRepository">Repository for BankingZone data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateBankingZoneCommandHandler(
            IBankingZoneRepository BankingZoneRepository,
            ILogger<UpdateBankingZoneCommandHandler> logger,
            IMapper mapper, IMongoUnitOfWork? mongoUnitOfWork, UserInfoToken? userInfoToken,
            IUnitOfWork<POSContext> uow = null)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _BankingZoneRepository = BankingZoneRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateBankingZoneCommand to update a BankingZone.
        /// </summary>
        /// <param name="request">The UpdateBankingZoneCommand containing updated BankingZone data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankingZoneDto>> Handle(UpdateBankingZoneCommand request, CancellationToken cancellationToken)
        {  
         string errorMessage = string.Empty;    
            try
            {
                // Retrieve the BankingZone entity to be updated from the repository
                 
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankingZone>();
                //var existingBankingZone = await _BankingZoneRepository.FindAsync(request.Id);
                var existingnRecord = await _recordRepository.GetByIdAsync(request.Id);
                if (existingnRecord == null)
                {
                    errorMessage = $"record with ID: {request.Id}  not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.UpdateBankingZoneCommand, LogLevelInfo.Warning);
                    return ServiceResponse<BankingZoneDto>.Return409(errorMessage);
                }
                BaseUtilities.PrepareMonoDBDataForCreation(existingnRecord, _userInfoToken, TrackerState.Modified);
                  await _recordRepository.UpdateAsync(existingnRecord.Id, existingnRecord);
                errorMessage = "BankingZone entry updated successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                           request, HttpStatusCodeEnum.OK, LogAction.UpdateBankingZoneCommand, LogLevelInfo.Information);
                var BankingZoneDto = _mapper.Map<BankingZoneDto>(existingnRecord);
                return ServiceResponse<BankingZoneDto>.ReturnResultWith200(BankingZoneDto, errorMessage);
            
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with an error message
                  errorMessage = $"Error occurred while updating BankingZone: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BankingZoneDto>.Return500(e, errorMessage);
            }
        }
    }

}
