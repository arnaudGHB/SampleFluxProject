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
    /// Handles the command to delete a BudgetName based on DeleteBudgetNameCommand.
    /// </summary>
    public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, ServiceResponse<bool>>
    {
        private readonly IBudgetRepository _BudgetRepository; // Repository for accessing BudgetName data.
        private readonly ILogger<DeleteBudgetCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the DeleteBudgetNameCommandHandler.
        /// </summary>
        /// <param name="BudgetNameRepository">Repository for BudgetName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteBudgetCommandHandler(
            IBudgetRepository BudgetRepository, IMapper mapper,
            ILogger<DeleteBudgetCommandHandler> logger
, IUnitOfWork<POSContext> uow, UserInfoToken? userInfoToken)
        {
            _BudgetRepository = BudgetRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteBudgetNameCommand to delete a BudgetName.
        /// </summary>
        /// <param name="request">The DeleteBudgetNameCommand containing BudgetName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the BudgetName entity with the specified ID exists
                var existingBudgetName = await _BudgetRepository.FindAsync(request.Id);
                if (existingBudgetName == null)
                {
                    errorMessage = $"Budget with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                 await   APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteBudgetCommand",
            request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    

                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingBudgetName.IsDeleted = true;

                _BudgetRepository.Update(existingBudgetName);
                errorMessage = $"Budget with ID {request.Id} has been deleted successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteBudgetCommand",
request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting Budget: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteBudgetCommand",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}