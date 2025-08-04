using AutoMapper;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a SubdivisionName based on DeleteSubdivisionNameCommand.
    /// </summary>
    public class DeleteSubdivisionCommandHandler : IRequestHandler<DeleteSubdivisionCommand, ServiceResponse<bool>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing SubdivisionName data.
        private readonly ILogger<DeleteSubdivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteSubdivisionNameCommandHandler.
        /// </summary>
        /// <param name="SubdivisionNameRepository">Repository for SubdivisionName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteSubdivisionCommandHandler(
            ISubdivisionRepository SubdivisionNameRepository, IMapper mapper,
            ILogger<DeleteSubdivisionCommandHandler> logger
, IUnitOfWork<SystemContext> uow)
        {
            _SubdivisionRepository = SubdivisionNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteSubdivisionNameCommand to delete a SubdivisionName.
        /// </summary>
        /// <param name="request">The DeleteSubdivisionNameCommand containing SubdivisionName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteSubdivisionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the SubdivisionName entity with the specified ID exists
                var existingSubdivisionName = await _SubdivisionRepository.FindAsync(request.Id);
                if (existingSubdivisionName == null)
                {
                    errorMessage = $"SubdivisionName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingSubdivisionName.IsDeleted = true;

                _SubdivisionRepository.Update(existingSubdivisionName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting SubdivisionName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}