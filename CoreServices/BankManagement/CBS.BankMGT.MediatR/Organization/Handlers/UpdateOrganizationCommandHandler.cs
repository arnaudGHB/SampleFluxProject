using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;

using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a Organization based on UpdateOrganizationCommand.
    /// </summary>
    public class UpdateOrganizationCommandHandler : IRequestHandler<UpdateOrganizationCommand, ServiceResponse<OrganizationDto>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organization data.
        private readonly ILogger<UpdateOrganizationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateOrganizationCommandHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organization data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOrganizationCommandHandler(
            IOrganizationRepository OrganizationRepository,
            ILogger<UpdateOrganizationCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _OrganizationRepository = OrganizationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateOrganizationCommand to update a Organization.
        /// </summary>
        /// <param name="request">The UpdateOrganizationCommand containing updated Organization data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OrganizationDto>> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Organization entity to be updated from the repository
                var existingOrganization = await _OrganizationRepository.FindAsync(request.Id);

                // Check if the Organization entity exists
                if (existingOrganization != null)
                {
                    // Update Organization entity properties with values from the request
                    existingOrganization.Name = request.Name;
                    existingOrganization.CountryId = request.CountryId;
                    existingOrganization.Description = request.Description;
                    // Use the repository to update the existing Organization entity
                    _OrganizationRepository.Update(existingOrganization);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<OrganizationDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<OrganizationDto>.ReturnResultWith200(_mapper.Map<OrganizationDto>(existingOrganization));
                    _logger.LogInformation($"Organization {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Organization entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<OrganizationDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Organization: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<OrganizationDto>.Return500(e);
            }
        }
    }

}
