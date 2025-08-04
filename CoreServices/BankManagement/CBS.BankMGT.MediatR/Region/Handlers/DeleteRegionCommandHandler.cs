using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Repository;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Domain;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a Region based on DeleteRegionCommand.
    /// </summary>
    public class DeleteRegionCommandHandler : IRequestHandler<DeleteRegionCommand, ServiceResponse<bool>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing Region data.
        private readonly ILogger<DeleteRegionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteRegionCommandHandler.
        /// </summary>
        /// <param name="RegionRepository">Repository for Region data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteRegionCommandHandler(
            IRegionRepository RegionRepository, IMapper mapper,
            ILogger<DeleteRegionCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _RegionRepository = RegionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteRegionCommand to delete a Region.
        /// </summary>
        /// <param name="request">The DeleteRegionCommand containing Region ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteRegionCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the Region entity with the specified ID exists
                var existingRegion = await _RegionRepository.FindAsync(request.Id);
                if (existingRegion == null)
                {
                    errorMessage = $"Region with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }
                existingRegion.IsDeleted = true;
                _RegionRepository.Update(existingRegion);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Region: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
