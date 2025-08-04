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
    /// Handles the command to update a Subdivision based on UpdateSubdivisionCommand.
    /// </summary>
    public class UpdateSubdivisionCommandHandler : IRequestHandler<UpdateSubdivisionCommand, ServiceResponse<SubdivisionDto>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing Subdivision data.
        private readonly ILogger<UpdateSubdivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<SystemContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateSubdivisionCommandHandler.
        /// </summary>
        /// <param name="SubdivisionRepository">Repository for Subdivision data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateSubdivisionCommandHandler(
            ISubdivisionRepository SubdivisionRepository,
            ILogger<UpdateSubdivisionCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<SystemContext> uow = null)
        {
            _SubdivisionRepository = SubdivisionRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateSubdivisionCommand to update a Subdivision.
        /// </summary>
        /// <param name="request">The UpdateSubdivisionCommand containing updated Subdivision data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubdivisionDto>> Handle(UpdateSubdivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Subdivision entity to be updated from the repository
                var existingSubdivision = await _SubdivisionRepository.FindAsync(request.Id);

                // Check if the Subdivision entity exists
                if (existingSubdivision != null)
                {
                    // Update Subdivision entity properties with values from the request
                    existingSubdivision.Name= request.Name;
                    existingSubdivision.DivisionId = request.DivisionId;
                    existingSubdivision = _mapper.Map(request, existingSubdivision);
                    // Use the repository to update the existing Subdivision entity
                    _SubdivisionRepository.Update(existingSubdivision);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<SubdivisionDto>.ReturnResultWith200(_mapper.Map<SubdivisionDto>(existingSubdivision));
                    _logger.LogInformation($"Subdivision {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Subdivision entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<SubdivisionDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Subdivision: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SubdivisionDto>.Return500(e);
            }
        }
    }
}