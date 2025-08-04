using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Queries;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Organization based on its unique identifier.
    /// </summary>
    public class GetOrganizationQueryHandler : IRequestHandler<GetOrganizationQuery, ServiceResponse<OrganizationDto>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organization data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOrganizationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetOrganizationQueryHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organization data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOrganizationQueryHandler(
            IOrganizationRepository OrganizationRepository,
            IMapper mapper,
            ILogger<GetOrganizationQueryHandler> logger)
        {
            _OrganizationRepository = OrganizationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetOrganizationQuery to retrieve a specific Organization.
        /// </summary>
        /// <param name="request">The GetOrganizationQuery containing Organization ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OrganizationDto>> Handle(GetOrganizationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Organization entity with the specified ID from the repository
                var existingOrganization = await _OrganizationRepository.AllIncluding(c => c.Banks, cy => cy.Country).FirstOrDefaultAsync(c => c.Id == request.Id);
                if (existingOrganization != null)
                {
                    // Map the Organization entity to OrganizationDto and return it with a success response
                    var OrganizationDto = _mapper.Map<OrganizationDto>(existingOrganization);
                    return ServiceResponse<OrganizationDto>.ReturnResultWith200(OrganizationDto);
                }
                else
                {
                    // If the Organization entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Organization not found.");
                    return ServiceResponse<OrganizationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Organization: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<OrganizationDto>.Return500(e);
            }
        }
    }

}
