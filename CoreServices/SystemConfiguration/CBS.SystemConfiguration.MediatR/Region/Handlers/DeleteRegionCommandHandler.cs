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
    /// Handles the command to delete a RegionName based on DeleteRegionNameCommand.
    /// </summary>
    public class DeleteRegionCommandHandler : IRequestHandler<DeleteRegionCommand, ServiceResponse<bool>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing RegionName data.
        private readonly ILogger<DeleteRegionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteRegionNameCommandHandler.
        /// </summary>
        /// <param name="RegionNameRepository">Repository for RegionName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteRegionCommandHandler(
            IRegionRepository RegionNameRepository, IMapper mapper,
            ILogger<DeleteRegionCommandHandler> logger
, IUnitOfWork<SystemContext> uow)
        {
            _RegionRepository = RegionNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteRegionNameCommand to delete a RegionName.
        /// </summary>
        /// <param name="request">The DeleteRegionNameCommand containing RegionName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the RegionName entity with the specified ID exists
                var existingRegionName = await _RegionRepository.FindAsync(request.Id);
                if (existingRegionName == null)
                {
                    errorMessage = $"RegionName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingRegionName.IsDeleted = true;

                _RegionRepository.Update(existingRegionName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting RegionName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}