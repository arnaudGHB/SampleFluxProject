using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;
using CBS.BankMGT.Data;
using CBS.BankMGT.Common.Repository.Uow;

namespace CBS.BankmGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BankingZones based on the GetAllBankingZoneQuery.
    /// </summary>
    public class GetAllBankingZoneQueryHandler : IRequestHandler<GetAllBankingZoneQuery, ServiceResponse<List<BankingZoneDto>>>
    {
        private readonly IBankingZoneRepository _BankingZoneRepository; // Repository for accessing BankingZones data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBankingZoneQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMongoUnitOfWork _mongoUnitOfWork; // Repository for accessing BankingZones data.
        /// <summary>
        /// Constructor for initializing the GetAllBankingZoneQueryHandler.
        /// </summary>
        /// <param name="BankingZoneRepository">Repository for BankingZones data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBankingZoneQueryHandler(
            IBankingZoneRepository BankingZoneRepository,
            IMapper mapper, ILogger<GetAllBankingZoneQueryHandler> logger, IMongoUnitOfWork? mongoUnitOfWork)
        {
            // Assign provided dependencies to local variables.
            _BankingZoneRepository = BankingZoneRepository;
            _mapper = mapper;
            _logger = logger;
            _mongoUnitOfWork = mongoUnitOfWork;
        }

        /// <summary>
        /// Handles the GetAllBankingZoneQuery to retrieve all BankingZones.
        /// </summary>
        /// <param name="request">The GetAllBankingZoneQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BankingZoneDto>>> Handle(GetAllBankingZoneQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var _recordRepository = _mongoUnitOfWork.GetRepository<BankingZone>();
                var entities = await _recordRepository.GetAllAsync();
                //entities = await _BankingZoneRepository.All.ToListAsync();
               return ServiceResponse<List<BankingZoneDto>>.ReturnResultWith200(_mapper.Map<List<BankingZoneDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BankingZones: {e.Message}");
                return ServiceResponse<List<BankingZoneDto>>.Return500(e, "Failed to get all BankingZones");
            }
        }
    }
}
