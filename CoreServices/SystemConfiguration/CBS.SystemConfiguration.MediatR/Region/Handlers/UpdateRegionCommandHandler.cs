using AutoMapper;
using CBS.SystemConfiguration.Common;
using CBS.SystemConfiguration.Data;
using CBS.SystemConfiguration.Domain;
using CBS.SystemConfiguration.Helper;
using CBS.SystemConfiguration.MediatR.Commands;
using CBS.SystemConfiguration.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.SystemConfiguration.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a Region based on UpdateRegionCommand.
    /// </summary>
    public class UpdateRegionCommandHandler : IRequestHandler<UpdateRegionCommand, ServiceResponse<RegionDto>>
    {
        private readonly IRegionRepository _RegionRepository; // Repository for accessing Region data.
        private readonly ILogger<UpdateRegionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateRegionCommandHandler.
        /// </summary>
        /// <param name="RegionRepository">Repository for Region data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateRegionCommandHandler(
            IRegionRepository RegionRepository,
            ILogger<UpdateRegionCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<SystemContext> uow = null)
        {
            _RegionRepository = RegionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateRegionCommand to update a Region.
        /// </summary>
        /// <param name="request">The UpdateRegionCommand containing updated Region data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RegionDto>> Handle(UpdateRegionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Region entity to be updated from the repository
                var existingRegion = await _RegionRepository.FindAsync(request.Id);

                // Check if the Region entity exists
                if (existingRegion != null)
                {
                    // Update Region entity properties with values from the request
                    existingRegion.Name= request.Name;
                    existingRegion.CountryId = request.CountryId;
                    existingRegion = _mapper.Map(request, existingRegion);
                    // Use the repository to update the existing Region entity
                    _RegionRepository.Update(existingRegion);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<RegionDto>.ReturnResultWith200(_mapper.Map<RegionDto>(existingRegion));
                    _logger.LogInformation($"Region {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Region entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<RegionDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Region: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<RegionDto>.Return500(e);
            }
        }
    }
}