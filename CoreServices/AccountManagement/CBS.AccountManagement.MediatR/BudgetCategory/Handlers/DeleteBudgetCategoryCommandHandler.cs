using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to delete a BudgetCategoryName based on DeleteBudgetCategoryNameCommand.
    /// </summary>
    public class DeleteBudgetCategoryCommandHandler : IRequestHandler<DeleteBudgetCategoryCommand, ServiceResponse<bool>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryRepository; // Repository for accessing BudgetCategoryName data.
        private readonly ILogger<DeleteBudgetCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the DeleteBudgetCategoryNameCommandHandler.
        /// </summary>
        /// <param name="BudgetCategoryNameRepository">Repository for BudgetCategoryName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteBudgetCategoryCommandHandler(
            IBudgetCategoryRepository BudgetCategoryRepository, IMapper mapper,
            ILogger<DeleteBudgetCategoryCommandHandler> logger
, IUnitOfWork<POSContext> uow, UserInfoToken? userInfoToken)
        {
            _BudgetCategoryRepository = BudgetCategoryRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteBudgetCategoryNameCommand to delete a BudgetCategoryName.
        /// </summary>
        /// <param name="request">The DeleteBudgetCategoryNameCommand containing BudgetCategoryName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBudgetCategoryCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the BudgetCategoryName entity with the specified ID exists
                var existingBudgetCategoryName = await _BudgetCategoryRepository.FindAsync(request.Id);
                if (existingBudgetCategoryName == null)
                {
                    errorMessage = $"BudgetCategory with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                 await   APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteBudgetCategoryCommand",
            request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    

                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingBudgetCategoryName.IsDeleted = true;

                _BudgetCategoryRepository.Update(existingBudgetCategoryName);
                errorMessage = $"BudgetCategory with ID {request.Id} has been deleted successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteBudgetCategoryCommand",
request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting BudgetCategory: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteBudgetCategoryCommand",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}