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
    /// Handles the command to delete a DivisionName based on DeleteDivisionNameCommand.
    /// </summary>
    public class DeleteDivisionCommandHandler : IRequestHandler<DeleteDivisionCommand, ServiceResponse<bool>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing DivisionName data.
        private readonly ILogger<DeleteDivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteDivisionNameCommandHandler.
        /// </summary>
        /// <param name="DivisionNameRepository">Repository for DivisionName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteDivisionCommandHandler(
            IDivisionRepository DivisionNameRepository, IMapper mapper,
            ILogger<DeleteDivisionCommandHandler> logger
, IUnitOfWork<SystemContext> uow)
        {
            _DivisionRepository = DivisionNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteDivisionNameCommand to delete a DivisionName.
        /// </summary>
        /// <param name="request">The DeleteDivisionNameCommand containing DivisionName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteDivisionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the DivisionName entity with the specified ID exists
                var existingDivisionName = await _DivisionRepository.FindAsync(request.Id);
                if (existingDivisionName == null)
                {
                    errorMessage = $"DivisionName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingDivisionName.IsDeleted = true;

                _DivisionRepository.Update(existingDivisionName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting DivisionName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}