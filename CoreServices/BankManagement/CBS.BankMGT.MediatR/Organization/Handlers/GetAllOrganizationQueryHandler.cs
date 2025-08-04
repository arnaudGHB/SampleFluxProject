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
    /// Handles the retrieval of all Organizations based on the GetAllOrganizationQuery.
    /// </summary>
    public class GetAllOrganizationQueryHandler : IRequestHandler<GetAllOrganizationQuery, ServiceResponse<List<OrganizationDto>>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organizations data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOrganizationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllOrganizationQueryHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organizations data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOrganizationQueryHandler(
            IOrganizationRepository OrganizationRepository,
            IMapper mapper, ILogger<GetAllOrganizationQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _OrganizationRepository = OrganizationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllOrganizationQuery to retrieve all Organizations.
        /// </summary>
        /// <param name="request">The GetAllOrganizationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OrganizationDto>>> Handle(GetAllOrganizationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Organizations entities from the repository
                var entities = await _OrganizationRepository.AllIncluding(c => c.Banks, x=>x.Country).ToListAsync();
                return ServiceResponse<List<OrganizationDto>>.ReturnResultWith200(_mapper.Map<List<OrganizationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Organizations: {e.Message}");
                return ServiceResponse<List<OrganizationDto>>.Return500(e, "Failed to get all Organizations");
            }
        }
    }
}
