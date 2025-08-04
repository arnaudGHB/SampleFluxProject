using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;

namespace CBS.Organisation.MEDIATR.OrganisationMediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a Organisation based on UpdateOrganisationCommand.
    /// </summary>
    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, ServiceResponse<UpdateOrganization>>
    {
        private readonly IOrganizationRepository _OrganisationRepository; // Repository for accessing Organisation data.
        private readonly ILogger<UpdateOrganizationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateOrganisationCommandHandler.
        /// </summary>
        /// <param name="OrganisationRepository">Repository for Organisation data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOrganizationCommandHandler(
            IOrganizationRepository OrganisationRepository,
            ILogger<UpdateOrganizationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _OrganisationRepository = OrganisationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOrganisationCommand to update a Organisation.
        /// </summary>
        /// <param name="request">The UpdateOrganisationCommand containing updated Organisation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateOrganization>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Organisation entity to be updated from the repository
                var existingOrganisation = await _OrganisationRepository.FindAsync(request.Id);

                // Check if the Organisation entity exists
                if (existingOrganisation != null)
                {
                    // Update Organisation entity properties with values from the request
                  /*  existingOrganisation.Name = request.Name;
                    existingOrganisation.Address = request.Address;
                    existingOrganisation.Telephone = request.Telephone;
                    existingOrganisation.CreatedBy = request.UserID;*/
                    // Use the repository to update the existing Organisation entity
                    _OrganisationRepository.Update(existingOrganisation);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateOrganization>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateOrganization>.ReturnResultWith200(_mapper.Map<UpdateOrganization>(existingOrganisation));
                    _logger.LogInformation($"Organisation {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Organisation entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateOrganization>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Organisation: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateOrganization>.Return500(e);
            }
        }
    }

}
