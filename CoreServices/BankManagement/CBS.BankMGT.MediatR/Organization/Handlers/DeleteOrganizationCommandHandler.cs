using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Organization based on DeleteOrganizationCommand.
    /// </summary>
    public class DeleteOrganizationCommandHandler : IRequestHandler<DeleteOrganizationCommand, ServiceResponse<bool>>
    {
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organization data.
        private readonly ILogger<DeleteOrganizationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteOrganizationCommandHandler.
        /// </summary>
        /// <param name="OrganizationRepository">Repository for Organization data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOrganizationCommandHandler(
            IOrganizationRepository OrganizationRepository, IMapper mapper,
            ILogger<DeleteOrganizationCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _OrganizationRepository = OrganizationRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteOrganizationCommand to delete a Organization.
        /// </summary>
        /// <param name="request">The DeleteOrganizationCommand containing Organization ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Organization entity with the specified ID exists
                var existingOrganization = await _OrganizationRepository.FindAsync(request.Id);
                if (existingOrganization == null)
                {
                    errorMessage = $"Organization with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingOrganization.IsDeleted = true;
                _OrganizationRepository.Update(existingOrganization);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Organization: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
