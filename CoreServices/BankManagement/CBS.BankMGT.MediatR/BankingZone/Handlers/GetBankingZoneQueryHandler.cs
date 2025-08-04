using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
 
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BankingZone based on its unique identifier.
    /// </summary>
    public class GetBankingZoneQueryHandler : IRequestHandler<GetBankingZoneQuery, ServiceResponse<BankingZoneDto>>
    {
        private readonly IBankingZoneRepository _BankingZoneRepository; // Repository for accessing BankingZone data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankingZoneQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken? _userInfoToken;
        private readonly IMongoUnitOfWork? _mongoUnitOfWork;
        /// <summary>
        /// Constructor for initializing the GetBankingZoneQueryHandler.
        /// </summary>
        /// <param name="BankingZoneRepository">Repository for BankingZone data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBankingZoneQueryHandler(
            IBankingZoneRepository BankingZoneRepository,
            IMapper mapper,
            ILogger<GetBankingZoneQueryHandler> logger,
            IMongoUnitOfWork? mongoUnitOfWork)
        {
            _BankingZoneRepository = BankingZoneRepository;
            _mapper = mapper;
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        public async Task<ServiceResponse<BankingZoneDto>> Handle(GetBankingZoneQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankingZone>();
                //var existingBankingZone = await _BankingZoneRepository.FindAsync(request.Id);
                var existingnRecord = await _recordRepository.GetByIdAsync(request.Id);
                if (existingnRecord == null)
                {
                    errorMessage = $"record with ID: {request.Id} already exist.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage,
                          request, HttpStatusCodeEnum.NotFound, LogAction.AddBankingZoneCommand, LogLevelInfo.Warning);
                    return ServiceResponse<BankingZoneDto>.Return409(errorMessage);
                }
                var BankingZoneDto = _mapper.Map<BankingZoneDto>(existingnRecord);
                return ServiceResponse<BankingZoneDto>.ReturnResultWith200(BankingZoneDto);
            
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting BankingZone: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<BankingZoneDto>.Return500(e);
            }
        }
    }

}
