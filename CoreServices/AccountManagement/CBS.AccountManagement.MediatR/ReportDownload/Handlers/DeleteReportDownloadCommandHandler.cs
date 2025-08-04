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
    /// Handles the command to delete a AccountCategory based on DeleteAccountCategoryCommand.
    /// </summary>
    public class DeleteReportDownloadCommandHandler : IRequestHandler<DeleteReportCommand, ServiceResponse<bool>>
    {
        private readonly IReportDownloadRepository _reportDownloadRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DeleteReportDownloadCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the DeleteAccountCategoryCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteReportDownloadCommandHandler(
            IReportDownloadRepository AccountCategoryRepository, IMapper mapper,
            ILogger<DeleteReportDownloadCommandHandler> logger
, IUnitOfWork<POSContext> uow)
        {
            _reportDownloadRepository = AccountCategoryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the DeleteAccountCategoryCommand to delete a AccountCategory.
        /// </summary>
        /// <param name="request">The DeleteAccountCategoryCommand containing AccountCategory ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteReportCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the AccountCategory entity with the specified ID exists
                var existingAccountCategory = await _reportDownloadRepository.FindAsync(request.Id);
                if (existingAccountCategory == null)
                {
                    errorMessage = $"Report with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

      

                _reportDownloadRepository.Remove(existingAccountCategory);
              await _uow.SaveAsync();
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