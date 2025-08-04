using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Common.Repository.Uow;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;


namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a BankingZone based on DeleteBankingZoneCommand.
    /// </summary>
    public class DeleteBankingZoneCommandHandler : IRequestHandler<DeleteBankingZoneCommand, ServiceResponse<bool>>
    {
        private readonly IBankingZoneRepository _BankingZoneRepository; // Repository for accessing BankingZone data.
        private readonly ILogger<DeleteBankingZoneCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;
        /// <summary>
        /// Constructor for initializing the DeleteBankingZoneCommandHandler.
        /// </summary>
        /// <param name="BankingZoneRepository">Repository for BankingZone data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteBankingZoneCommandHandler(
            IBankingZoneRepository BankingZoneRepository, IMapper mapper,
            ILogger<DeleteBankingZoneCommandHandler> logger
, IUnitOfWork<POSContext> uow, IMongoUnitOfWork? mongoUnitOfWork, UserInfoToken? userInfoToken)
        {
            _userInfoToken = userInfoToken;
            _BankingZoneRepository = BankingZoneRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the DeleteBankingZoneCommand to delete a BankingZone.
        /// </summary>
        /// <param name="request">The DeleteBankingZoneCommand containing BankingZone ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBankingZoneCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the BankingZone entity with the specified ID exists
                //var existingBankingZone = await _BankingZoneRepository.FindAsync(request.Id);
                //if (existingBankingZone == null)
                //{
                //    errorMessage = $"BankingZone with ID {request.Id} not found.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<bool>.Return404(errorMessage);
                //}
                //existingBankingZone.IsDeleted = true;
                //_BankingZoneRepository.Update(existingBankingZone);
                //if (await _uow.SaveAsync() <= 0)
                //{
                //    return ServiceResponse<bool>.Return500();
                //}
                //return ServiceResponse<bool>.ReturnResultWith200(true);
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankingZone>();
                //var existingBankingZone = await _BankingZoneRepository.FindAsync(request.Id);
                var existingnRecord = await _recordRepository.GetByIdAsync(request.Id);
                if (existingnRecord == null)
                {
                    errorMessage = $"record with ID: {request.Id} already exist.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.AddBankingZoneCommand, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                BaseUtilities.PrepareMonoDBDataForCreation(existingnRecord, _userInfoToken, TrackerState.Deleted);
                await _recordRepository.UpdateAsync(existingnRecord.Id, existingnRecord);
                errorMessage = "BankingZone entry delted successfully";
                await BaseUtilities.LogAndAuditAsync(errorMessage,
                           request, HttpStatusCodeEnum.OK, LogAction.AddBankingZoneCommand, LogLevelInfo.Information);
                var BankingZoneDto = _mapper.Map<BankingZoneDto>(existingnRecord);
                return ServiceResponse<bool>.ReturnResultWith200(true, errorMessage);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting BankingZone: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
