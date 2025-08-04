using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a LinkAccountTypeAccountFeature based on DeleteLinkAccountTypeAccountFeatureCommand.
    /// </summary>
    public class DeleteLinkAccountTypeAccountFeatureCommandHandler : IRequestHandler<DeleteLinkAccountTypeAccountFeatureCommand, ServiceResponse<bool>>
    {
        private readonly ILinkAccountTypeAccountFeatureRepository _LinkAccountTypeAccountFeatureRepository; // Repository for accessing LinkAccountTypeAccountFeature data.
        private readonly ILogger<DeleteLinkAccountTypeAccountFeatureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteLinkAccountTypeAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="LinkAccountTypeAccountFeatureRepository">Repository for LinkAccountTypeAccountFeature data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteLinkAccountTypeAccountFeatureCommandHandler(
            ILinkAccountTypeAccountFeatureRepository LinkAccountTypeAccountFeatureRepository, IMapper mapper,
            ILogger<DeleteLinkAccountTypeAccountFeatureCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _LinkAccountTypeAccountFeatureRepository = LinkAccountTypeAccountFeatureRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteLinkAccountTypeAccountFeatureCommand to delete a LinkAccountTypeAccountFeature.
        /// </summary>
        /// <param name="request">The DeleteLinkAccountTypeAccountFeatureCommand containing LinkAccountTypeAccountFeature ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteLinkAccountTypeAccountFeatureCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the LinkAccountTypeAccountFeature entity with the specified ID exists
                var existingLinkAccountTypeAccountFeature = await _LinkAccountTypeAccountFeatureRepository.FindAsync(request.Id);
                if (existingLinkAccountTypeAccountFeature == null)
                {
                    errorMessage = $"LinkAccountTypeAccountFeature with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingLinkAccountTypeAccountFeature.IsDeleted = true;

                _LinkAccountTypeAccountFeatureRepository.Update(existingLinkAccountTypeAccountFeature);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting LinkAccountTypeAccountFeature: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}