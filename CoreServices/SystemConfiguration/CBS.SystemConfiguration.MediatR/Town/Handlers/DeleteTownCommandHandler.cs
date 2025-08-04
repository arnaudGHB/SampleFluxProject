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
    /// Handles the command to delete a TownName based on DeleteTownNameCommand.
    /// </summary>
    public class DeleteTownCommandHandler : IRequestHandler<DeleteTownCommand, ServiceResponse<bool>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing TownName data.
        private readonly ILogger<DeleteTownCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteTownNameCommandHandler.
        /// </summary>
        /// <param name="TownNameRepository">Repository for TownName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteTownCommandHandler(
            ITownRepository TownNameRepository, IMapper mapper,
            ILogger<DeleteTownCommandHandler> logger
, IUnitOfWork<SystemContext> uow)
        {
            _TownRepository = TownNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteTownNameCommand to delete a TownName.
        /// </summary>
        /// <param name="request">The DeleteTownNameCommand containing TownName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteTownCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the TownName entity with the specified ID exists
                var existingTownName = await _TownRepository.FindAsync(request.Id);
                if (existingTownName == null)
                {
                    errorMessage = $"TownName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingTownName.IsDeleted = true;

                _TownRepository.Update(existingTownName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting TownName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}