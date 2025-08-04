using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;
using CBS.BankingZoneMGT.MediatR.Commands;
using CBS.BankMGT.Common.Repository.Uow;
namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new BankingZone.
    /// </summary>
    public class AddBankingZoneCommandHandler : IRequestHandler<AddBankingZoneCommand, ServiceResponse<BankingZoneDto>>
    {
        private readonly IBankingZoneRepository _BankingZoneRepository; // Repository for accessing BankingZone data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBankingZoneCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // MongoDB Unit of Work.
        /// <summary>
        /// Constructor for initializing the AddBankingZoneCommandHandler.
        /// </summary>
        /// <param name="BankingZoneRepository">Repository for BankingZone data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBankingZoneCommandHandler(
            IBankingZoneRepository BankingZoneRepository,
            IMapper mapper,
            ILogger<AddBankingZoneCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IMongoUnitOfWork? mongoUnitOfWork, UserInfoToken? userInfoToken)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _BankingZoneRepository = BankingZoneRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddBankingZoneCommand to add a new BankingZone.
        /// </summary>
        /// <param name="request">The AddBankingZoneCommand containing BankingZone data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BankingZoneDto>> Handle(AddBankingZoneCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = "";
            try
            {
                // Check if a BankingZone with the same name already exists (case-insensitive)
              
              var   bankModel = request.ToBankingZone();
                // Get the MongoDB repository for TransacuytionTracker
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankingZone>();
                //var model = await _transactionTrackerRepository.FindAsync(request.TransactionReference);
 
                var existingnRecord = await _recordRepository.GetByIdAsync(bankModel.Id);
                if (existingnRecord != null)
                {
                      errorMessage = $"record with ID: {existingnRecord.Id} already exist.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.AddBankingZoneCommand, LogLevelInfo.Warning);
                    return ServiceResponse<BankingZoneDto>.Return409(errorMessage);
                }
                BaseUtilities.PrepareMonoDBDataForCreation(bankModel, _userInfoToken, TrackerState.Created);
                await _recordRepository.InsertAsync(bankModel);
                errorMessage = "Accounting event entries created successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                           request, HttpStatusCodeEnum.OK, LogAction.AddBankingZoneCommand, LogLevelInfo.Information);
                var BankingZoneDto = _mapper.Map<BankingZoneDto>(bankModel);
                return ServiceResponse<BankingZoneDto>.ReturnResultWith200(BankingZoneDto, errorMessage);

              
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                 errorMessage = $"Error occurred while saving BankingZone: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BankingZoneDto>.Return500(e, errorMessage);
            }
        }
    }

}
