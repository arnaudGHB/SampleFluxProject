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
    /// Handles the retrieval of all Currencys based on the GetAllCurrencyQuery.
    /// </summary>
    public class GetAllSubcriptionAggregatQueryHandler : IRequestHandler<GetAllSubcriptionAggregatQuery, ServiceResponse<SubcriptionAggregateDto>>
    {
        private readonly ICountryRepository _countryRepository; // Repository for accessing Currencys data.
        private readonly IEconomicActivityRepository _economicActivityRepository; // Repository for accessing Currencys data.
        private readonly IOrganizationRepository _organizationRepository; // Repository for accessing Currencys data.
        private readonly IBankRepository _bankRepository; // Repository for accessing Currencys data.
        private readonly IBranchRepository _branchRepository; // Repository for accessing Currencys data.
        private readonly IRegionRepository _regionRepository; // Repository for accessing Currencys data.
        private readonly IDivisionRepository _divisionRepository; // Repository for accessing Currencys data.
        private readonly ISubdivisionRepository _subdivisionRepository; // Repository for accessing Currencys data.
        private readonly ITownRepository _townRepository; // Repository for accessing Currencys data.

        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSubcriptionAggregatQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllSubcriptionPackageQueryHandler.
        /// </summary>
        /// <param name="CurrencyRepository">Repository for Currencys data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSubcriptionAggregatQueryHandler(
            ICountryRepository countryRepository,
            IMapper mapper, ILogger<GetAllSubcriptionAggregatQueryHandler> logger, IEconomicActivityRepository economicActivityRepository = null, IOrganizationRepository organizationRepository = null, IBankRepository bankRepository = null, IBranchRepository branchRepository = null, IRegionRepository regionRepository = null, IDivisionRepository divisionRepository = null, ISubdivisionRepository subdivisionRepository = null, ITownRepository townRepository = null)
        {
            // Assign provided dependencies to local variables.
            _countryRepository = countryRepository;
            _mapper = mapper;
            _logger = logger;
            _economicActivityRepository = economicActivityRepository;
            _organizationRepository = organizationRepository;
            _bankRepository = bankRepository;
            _branchRepository = branchRepository;
            _regionRepository = regionRepository;
            _divisionRepository = divisionRepository;
            _subdivisionRepository = subdivisionRepository;
            _townRepository = townRepository;
        }

        /// <summary>
        /// Handles the GetAllCurrencyQuery to retrieve all Currencys.
        /// </summary>
        /// <param name="request">The GetAllCurrencyQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubcriptionAggregateDto>> Handle(GetAllSubcriptionAggregatQuery request, CancellationToken cancellationToken)
        {
            try
            {
                
                // Retrieve all Currencys entities from the repository
                var countries = _mapper.Map<List<CountryDto>>(await _countryRepository.All.ToListAsync());
                var economicActivities = _mapper.Map<List<EconomicActivityDto>>(await _economicActivityRepository.All.ToListAsync());
                var organizations = _mapper.Map<List<OrganizationDto>>(await _organizationRepository.All.ToListAsync());
                var banks = _mapper.Map<List<BankDto>>(await _bankRepository.All.ToListAsync());
                var branches = _mapper.Map<List<BranchDto>>(await _branchRepository.All.ToListAsync());
                var divisions = _mapper.Map<List<DivisionDto>>(await _divisionRepository.All.ToListAsync());
                var subdivisions = _mapper.Map<List<SubdivisionDto>>(await _subdivisionRepository.All.ToListAsync());
                var towns = _mapper.Map<List<TownDto>>(await _townRepository.All.ToListAsync());
                var regions = _mapper.Map<List<RegionDto>>(await _regionRepository.All.ToListAsync());
                var data = new SubcriptionAggregateDto(economicActivities, organizations, banks, branches, countries,regions,subdivisions,divisions,towns,new List<SubcriptionPackageDto>());
                return ServiceResponse<SubcriptionAggregateDto>.ReturnResultWith200(data);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all aggregate: {e.Message}");
                return ServiceResponse<SubcriptionAggregateDto>.Return500(e, "Failed to get all aggregate");
            }
        }
    }
}
