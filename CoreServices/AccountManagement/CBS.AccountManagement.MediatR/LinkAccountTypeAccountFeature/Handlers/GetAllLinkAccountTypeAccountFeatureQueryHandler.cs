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
    /// Handles the retrieval of all LinkAccountTypeAccountFeatures based on the GetAllLinkAccountTypeAccountFeatureQuery.
    /// </summary>
    public class GetAllLinkAccountTypeAccountFeatureQueryHandler : IRequestHandler<GetAllLinkAccountTypeAccountFeatureQuery, ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>>
    {
        private readonly ILinkAccountTypeAccountFeatureRepository _LinkAccountTypeAccountFeatureRepository; // Repository for accessing LinkAccountTypeAccountFeatures data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLinkAccountTypeAccountFeatureQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLinkAccountTypeAccountFeatureQueryHandler.
        /// </summary>
        /// <param name="LinkAccountTypeAccountFeatureRepository">Repository for LinkAccountTypeAccountFeatures data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLinkAccountTypeAccountFeatureQueryHandler(
            ILinkAccountTypeAccountFeatureRepository LinkAccountTypeAccountFeatureRepository,
            IMapper mapper, ILogger<GetAllLinkAccountTypeAccountFeatureQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LinkAccountTypeAccountFeatureRepository = LinkAccountTypeAccountFeatureRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLinkAccountTypeAccountFeatureQuery to retrieve all LinkAccountTypeAccountFeatures.
        /// </summary>
        /// <param name="request">The GetAllLinkAccountTypeAccountFeatureQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>> Handle(GetAllLinkAccountTypeAccountFeatureQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LinkAccountTypeAccountFeatures entities from the repository
                var entities = await _LinkAccountTypeAccountFeatureRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>.ReturnResultWith200(_mapper.Map<List<LinkAccountTypeAccountFeatureDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LinkAccountTypeAccountFeatures: {e.Message}");
                return ServiceResponse<List<LinkAccountTypeAccountFeatureDto>>.Return500(e, "Failed to get all LinkAccountTypeAccountFeatures");
            }
        }
    }
}