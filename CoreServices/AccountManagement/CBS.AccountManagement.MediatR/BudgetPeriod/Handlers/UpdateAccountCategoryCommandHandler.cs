using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a AccountFeature based on UpdateAccountFeatureCommand.
    /// </summary>
    public class UpdateAccountFeatureCommandHandler : IRequestHandler<UpdateAccountFeatureCommand, ServiceResponse<AccountFeatureDto>>
    {
        private readonly IAccountFeatureRepository _AccountFeatureRepository; // Repository for accessing AccountFeature data.
        private readonly ILogger<UpdateAccountFeatureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateAccountFeatureCommandHandler(
            IAccountFeatureRepository AccountFeatureRepository,
            ILogger<UpdateAccountFeatureCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _AccountFeatureRepository = AccountFeatureRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateAccountFeatureCommand to update a AccountFeature.
        /// </summary>
        /// <param name="request">The UpdateAccountFeatureCommand containing updated AccountFeature data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountFeatureDto>> Handle(UpdateAccountFeatureCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the AccountFeature entity to be updated from the repository
                var existingAccountFeature = await _AccountFeatureRepository.FindAsync(request.Id);

                // Check if the AccountFeature entity exists
                if (existingAccountFeature != null)
                {
                    // Update AccountFeature entity properties with values from the request
                    existingAccountFeature.Name = request.Name.ToUpper();
                    existingAccountFeature.Description = request.Description;
                    // Use the repository to update the existing AccountFeature entity
                    _AccountFeatureRepository.Update(existingAccountFeature);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<AccountFeatureDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<AccountFeatureDto>.ReturnResultWith200(_mapper.Map<AccountFeatureDto>(existingAccountFeature));
                    _logger.LogInformation($"AccountFeature {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the AccountFeature entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountFeatureDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating AccountFeature: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountFeatureDto>.Return500(e);
            }
        }
    }
}