using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a LinkAccountTypeAccountFeature based on UpdateLinkAccountTypeAccountFeatureCommand.
    /// </summary>
    public class UpdateLinkAccountTypeAccountFeatureCommandHandler : IRequestHandler<UpdateLinkAccountTypeAccountFeatureCommand, ServiceResponse<LinkAccountTypeAccountFeatureDto>>
    {
        private readonly ILinkAccountTypeAccountFeatureRepository _LinkAccountTypeAccountFeatureRepository; // Repository for accessing LinkAccountTypeAccountFeature data.
        private readonly ILogger<UpdateLinkAccountTypeAccountFeatureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateLinkAccountTypeAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="LinkAccountTypeAccountFeatureRepository">Repository for LinkAccountTypeAccountFeature data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLinkAccountTypeAccountFeatureCommandHandler(
            ILinkAccountTypeAccountFeatureRepository LinkAccountTypeAccountFeatureRepository,
            ILogger<UpdateLinkAccountTypeAccountFeatureCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _LinkAccountTypeAccountFeatureRepository = LinkAccountTypeAccountFeatureRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateLinkAccountTypeAccountFeatureCommand to update a LinkAccountTypeAccountFeature.
        /// </summary>
        /// <param name="request">The UpdateLinkAccountTypeAccountFeatureCommand containing updated LinkAccountTypeAccountFeature data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LinkAccountTypeAccountFeatureDto>> Handle(UpdateLinkAccountTypeAccountFeatureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LinkAccountTypeAccountFeature entity to be updated from the repository
                var existingLinkAccountTypeAccountFeature = await _LinkAccountTypeAccountFeatureRepository.FindAsync(request.Id);

                // Check if the LinkAccountTypeAccountFeature entity exists
                if (existingLinkAccountTypeAccountFeature != null)
                {
                    // Update LinkAccountTypeAccountFeature entity properties with values from the request

                    existingLinkAccountTypeAccountFeature = LinkAccountTypeAccountFeature.UpdateLinkAccountTypeAccountFeature(JsonConvert.SerializeObject(request), existingLinkAccountTypeAccountFeature);
                    // Use the repository to update the existing LinkAccountTypeAccountFeature entity
                    _LinkAccountTypeAccountFeatureRepository.Update(existingLinkAccountTypeAccountFeature);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LinkAccountTypeAccountFeatureDto>.ReturnResultWith200(_mapper.Map<LinkAccountTypeAccountFeatureDto>(existingLinkAccountTypeAccountFeature));
                    _logger.LogInformation($"LinkAccountTypeAccountFeature {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the LinkAccountTypeAccountFeature entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LinkAccountTypeAccountFeature: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LinkAccountTypeAccountFeatureDto>.Return500(e);
            }
        }
    }
}