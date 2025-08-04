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
    /// Handles the command to delete a AccountCategory based on DeleteAccountCategoryCommand.
    /// </summary>
    public class DeleteAccountCategoryCommandHandler : IRequestHandler<DeleteAccountCategoryCommand, ServiceResponse<bool>>
    {
        private readonly IAccountCategoryRepository _AccountCategoryRepository; // Repository for accessing AccountCategory data.
        private readonly ILogger<DeleteAccountCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteAccountCategoryCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteAccountCategoryCommandHandler(
            IAccountCategoryRepository AccountCategoryRepository, IMapper mapper,
            ILogger<DeleteAccountCategoryCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _AccountCategoryRepository = AccountCategoryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountCategoryCommand to delete a AccountCategory.
        /// </summary>
        /// <param name="request">The DeleteAccountCategoryCommand containing AccountCategory ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteAccountCategoryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountCategory entity with the specified ID exists
                var existingAccountCategory = await _AccountCategoryRepository.FindAsync(request.Id);
                if (existingAccountCategory == null)
                {
                    errorMessage = $"AccountCategory with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingAccountCategory.IsDeleted = true;

                _AccountCategoryRepository.Update(existingAccountCategory);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<bool>.Return500();
                }
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting AccountCategory: {e.Message}";

                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}