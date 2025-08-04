
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;

namespace CBS.Organization.MEDIATR.OrganizationMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Organization.
    /// </summary>
    public class AddOrganizationCommandHandler : IRequestHandler<AddOrganizationCommand, ServiceResponse<CreateOrganization>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organization data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOrganizationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddOrganizationCommandHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organization data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOrganizationCommandHandler(
            IOrganizationRepository OrganizationRepository,
            IMapper mapper,
            ILogger<AddOrganizationCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _OrganizationRepository = OrganizationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddOrganizationCommand to add a new Organization.
        /// </summary>
        /// <param name="request">The AddOrganizationCommand containing Organization data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateOrganization>> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Organization with the same name already exists (case-insensitive)
                var existingOrganization = await _OrganizationRepository.FindBy(c => c.OrganizationName == (request.OrganizationName)).FirstOrDefaultAsync();

                // If a Organization with the same name already exists, return a conflict response
                if (existingOrganization != null)
                {
                    var errorMessage = $"Organization {(request.OrganizationName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateOrganization>.Return409(errorMessage);
                }

                // Map the AddOrganizationCommand to a Organization entity
                var OrganizationEntity = _mapper.Map<CUSTOMER.DATA.Entity.Organization>(request);

               

                OrganizationEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                OrganizationEntity.OrganizationId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new Organization entity to the repository
                _OrganizationRepository.Add(OrganizationEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateOrganization>.Return500();
                }
                // Map the Organization entity to CreateOrganization and return it with a success response
                var CreateOrganization = _mapper.Map<CreateOrganization>(OrganizationEntity);
                return ServiceResponse<CreateOrganization>.ReturnResultWith200(CreateOrganization);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Organization: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateOrganization>.Return500(e);
            }
        }
    }

}
