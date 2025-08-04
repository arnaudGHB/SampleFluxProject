using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Budget.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a BudgetName based on UpdateBudgetNameCommand.
    /// </summary>
    public class AddBudgetApprovalCommandHandler : IRequestHandler<AddBudgetApprovalCommand, ServiceResponse<BudgetDto>>
    {
        private readonly IBudgetRepository _BudgetNameRepository; // Repository for accessing BudgetName data.
        private readonly ILogger<AddBudgetApprovalCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the UpdateBudgetNameCommandHandler.
        /// </summary>
        /// <param name="BudgetNameRepository">Repository for BudgetName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public AddBudgetApprovalCommandHandler(
            IBudgetRepository BudgetNameRepository,
            ILogger<AddBudgetApprovalCommandHandler> logger,
            IMapper mapper,
            UserInfoToken? userInfoToken,
            IUnitOfWork<POSContext> uow = null)
        {
            _BudgetNameRepository = BudgetNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;

            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateBudgetNameCommand to update a BudgetName.
        /// </summary>
        /// <param name="request">The UpdateBudgetNameCommand containing updated BudgetName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetDto>> Handle(AddBudgetApprovalCommand request, CancellationToken cancellationToken)
        {
           string errorMessage = string.Empty;
            try
            {
                // Retrieve the BudgetName entity to be updated from the repository
                var existingBudgetName = await _BudgetNameRepository.FindAsync(request.Id);

                // Check if the BudgetName entity exists
                if (existingBudgetName != null)
                {
                    // Update BudgetName entity properties with values from the request

                   
                    if (existingBudgetName.IsApproved)
                    {
                        errorMessage = $"Budget {request.Id} was successfully already APPROVED.";
                        // Prepare the response and return a successful response with 200 status code
                      
                        _logger.LogInformation(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetApprovalCommand",
                       request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                        return ServiceResponse<BudgetDto>.Return409();
                    }
                    existingBudgetName.IsApproved= true;
                    existingBudgetName.ApprovalDate =BaseUtilities.UtcToDoualaTime( DateTime.Now);
                    existingBudgetName.ApprovedBy = _userInfoToken.Id;
                    _BudgetNameRepository.Update(existingBudgetName);
                    await _uow.SaveAsync();

                    errorMessage = $"Budget {request.Id} was successfully APPROVED.";
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<BudgetDto>.ReturnResultWith200(_mapper.Map<BudgetDto>(existingBudgetName));
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetApprovalCommand",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return response;
                }
                else
                {
                    // If the BudgetName entity was not found, return 404 Not Found response with an error message
                      errorMessage = $"Budget {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetApprovalCommand",
                request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                  errorMessage = $"Error occurred while updating BudgetName: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateBudgetCommand",
          request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<BudgetDto>.Return500(e);
            }
        }
    }
}