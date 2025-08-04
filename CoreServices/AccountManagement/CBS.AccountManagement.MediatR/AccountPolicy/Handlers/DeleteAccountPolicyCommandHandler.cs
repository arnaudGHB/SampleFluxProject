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
    /// Handles the command to delete a AccountPolicyName based on DeleteAccountPolicyNameCommand.
    /// </summary>
    public class DeleteAccountPolicyCommandHandler : IRequestHandler<DeleteAccountPolicyCommand, ServiceResponse<bool>>
    {
        private readonly IAccountPolicyRepository _AccountPolicyNameRepository; // Repository for accessing AccountPolicyName data.
        private readonly ILogger<DeleteAccountPolicyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAccountPolicyNameCommandHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountPolicyCommandHandler(
            IAccountPolicyRepository AccountPolicyNameRepository, IMapper mapper,
            ILogger<DeleteAccountPolicyCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _AccountPolicyNameRepository = AccountPolicyNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountPolicyNameCommand to delete a AccountPolicyName.
        /// </summary>
        /// <param name="request">The DeleteAccountPolicyNameCommand containing AccountPolicyName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountPolicyCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountPolicyName entity with the specified ID exists
                var existingAccountPolicyName = await _AccountPolicyNameRepository.FindAsync(request.Id);
                if (existingAccountPolicyName == null)
                {
                    errorMessage = $"AccountPolicyName with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccountPolicyName.IsDeleted = true;

                _AccountPolicyNameRepository.Update(existingAccountPolicyName);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AccountPolicyName: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}