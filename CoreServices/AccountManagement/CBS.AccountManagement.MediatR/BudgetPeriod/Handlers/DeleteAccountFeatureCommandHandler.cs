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
    /// Handles the command to delete a AccountFeature based on DeleteAccountFeatureCommand.
    /// </summary>
    public class DeleteAccountFeatureCommandHandler : IRequestHandler<DeleteAccountFeatureCommand, ServiceResponse<bool>>
    {
        private readonly IAccountFeatureRepository _AccountFeatureRepository; // Repository for accessing AccountFeature data.
        private readonly ILogger<DeleteAccountFeatureCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAccountFeatureCommandHandler.
        /// </summary>
        /// <param name="AccountFeatureRepository">Repository for AccountFeature data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountFeatureCommandHandler(
            IAccountFeatureRepository AccountFeatureRepository, IMapper mapper,
            ILogger<DeleteAccountFeatureCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _AccountFeatureRepository = AccountFeatureRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountFeatureCommand to delete a AccountFeature.
        /// </summary>
        /// <param name="request">The DeleteAccountFeatureCommand containing AccountFeature ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountFeatureCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountFeature entity with the specified ID exists
                var existingAccountFeature = await _AccountFeatureRepository.FindAsync(request.Id);
                if (existingAccountFeature == null)
                {
                    errorMessage = $"AccountFeature with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccountFeature.IsDeleted = true;

                _AccountFeatureRepository.Update(existingAccountFeature);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AccountFeature: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}