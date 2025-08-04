using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AccountFeatures based on the GetAllAccountFeatureQuery.
    /// </summary>
    public class GetAllAccountFeatureQueryHandler : IRequestHandler<GetAllAccountFeatureQuery, ServiceResponse<List<AccountFeatureDto>>>
    {
        private readonly IAccountFeatureRepository _AccountFeatureRepository; // Repository for accessing AccountFeatures data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountFeatureQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountFeatureQueryHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeatures data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountFeatureQueryHandler(
            IAccountFeatureRepository AccountFeatureRepository,
            IMapper mapper, ILogger<GetAllAccountFeatureQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountFeatureRepository = AccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountFeatureQuery to retrieve all AccountFeatures.
        /// </summary>
        /// <param name="request">The GetAllAccountFeatureQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountFeatureDto>>> Handle(GetAllAccountFeatureQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountFeatures entities from the repository
                var entities = await _AccountFeatureRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<AccountFeatureDto>>.ReturnResultWith200(_mapper.Map<List<AccountFeatureDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all AccountFeatures: {e.Message}");
                return ServiceResponse<List<AccountFeatureDto>>.Return500(e, "Failed to get all AccountFeatures");
            }
        }
    }
}