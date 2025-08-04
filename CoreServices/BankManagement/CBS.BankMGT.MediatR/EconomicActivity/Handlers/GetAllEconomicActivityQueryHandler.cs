using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using Microsoft.EntityFrameworkCore;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all EconomicActivitys based on the GetAllEconomicActivityQuery.
    /// </summary>
    public class GetAllEconomicActivityQueryHandler : IRequestHandler<GetAllEconomicActivityQuery, ServiceResponse<List<EconomicActivityDto>>>
    {
        private readonly IEconomicActivityRepository _EconomicActivityRepository; // Repository for accessing EconomicActivitys data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllEconomicActivityQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllEconomicActivityQueryHandler.
        /// </summary>
        /// <param name="EconomicActivityRepository">Repository for EconomicActivitys data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllEconomicActivityQueryHandler(
            IEconomicActivityRepository EconomicActivityRepository,
            IMapper mapper, ILogger<GetAllEconomicActivityQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _EconomicActivityRepository = EconomicActivityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllEconomicActivityQuery to retrieve all EconomicActivitys.
        /// </summary>
        /// <param name="request">The GetAllEconomicActivityQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<EconomicActivityDto>>> Handle(GetAllEconomicActivityQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all EconomicActivitys entities from the repository
                var entities = await _EconomicActivityRepository.All.Where(x=>x.IsDeleted==false).ToListAsync();
                return ServiceResponse<List<EconomicActivityDto>>.ReturnResultWith200(_mapper.Map<List<EconomicActivityDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all EconomicActivitys: {e.Message}");
                return ServiceResponse<List<EconomicActivityDto>>.Return500(e, "Failed to get all EconomicActivitys");
            }
        }
    }
}
