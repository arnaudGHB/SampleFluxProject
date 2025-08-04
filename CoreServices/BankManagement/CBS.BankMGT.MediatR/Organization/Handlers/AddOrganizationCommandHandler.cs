using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Organization.
    /// </summary>
    public class AddOrganizationCommandHandler : IRequestHandler<AddOrganizationCommand, ServiceResponse<OrganizationDto>>
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
        public async Task<ServiceResponse<OrganizationDto>> Handle(AddOrganizationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Organization with the same name already exists (case-insensitive)
                var existingOrganization = await _OrganizationRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Organization with the same name already exists, return a conflict response
                if (existingOrganization!=null)
                {
                    var errorMessage = $"Organization {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OrganizationDto>.Return409(errorMessage);
                }
                // Map the AddOrganizationCommand to a Organization entity
                var OrganizationEntity = _mapper.Map<Organization>(request);
                // Convert UTC to local time and set it in the entity
                OrganizationEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                OrganizationEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Organization entity to the repository
                _OrganizationRepository.Add(OrganizationEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<OrganizationDto>.Return500();
                }
                // Map the Organization entity to OrganizationDto and return it with a success response
                var OrganizationDto = _mapper.Map<OrganizationDto>(OrganizationEntity);
                return ServiceResponse<OrganizationDto>.ReturnResultWith200(OrganizationDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Organization: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OrganizationDto>.Return500(e);
            }
        }
    }

}
