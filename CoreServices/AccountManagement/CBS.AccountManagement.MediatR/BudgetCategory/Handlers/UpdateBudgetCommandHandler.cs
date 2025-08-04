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
    /// Handles the command to update a BudgetCategoryName based on UpdateBudgetCategoryNameCommand.
    /// </summary>
    public class UpdateBudgetCategoryCommandHandler : IRequestHandler<UpdateBudgetCategoryCommand, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryNameRepository; // Repository for accessing BudgetCategoryName data.
        private readonly ILogger<UpdateBudgetCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the UpdateBudgetCategoryNameCommandHandler.
        /// </summary>
        /// <param name="BudgetCategoryNameRepository">Repository for BudgetCategoryName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateBudgetCategoryCommandHandler(
            IBudgetCategoryRepository BudgetCategoryNameRepository,
            ILogger<UpdateBudgetCategoryCommandHandler> logger,
            IMapper mapper,
            UserInfoToken? userInfoToken,
            IUnitOfWork<POSContext> uow = null)
        {
            _BudgetCategoryNameRepository = BudgetCategoryNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;

            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateBudgetCategoryNameCommand to update a BudgetCategoryName.
        /// </summary>
        /// <param name="request">The UpdateBudgetCategoryNameCommand containing updated BudgetCategoryName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(UpdateBudgetCategoryCommand request, CancellationToken cancellationToken)
        {
           string errorMessage = string.Empty;
            try
            {
                // Retrieve the BudgetCategoryName entity to be updated from the repository
                var existingBudgetCategoryName = await _BudgetCategoryNameRepository.FindAsync(request.Id);

                // Check if the BudgetCategoryName entity exists
                if (existingBudgetCategoryName != null)
                {
                    // Update BudgetCategoryName entity properties with values from the request

                    existingBudgetCategoryName= _mapper.Map(request, existingBudgetCategoryName);
                    // Use the repository to update the existing BudgetCategoryName entity
                    _BudgetCategoryNameRepository.Update(existingBudgetCategoryName);
                    await _uow.SaveAsync();

                    errorMessage = $"BudgetCategory {request.Id} was successfully updated.";
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(_mapper.Map<BudgetCategoryDto>(existingBudgetCategoryName));
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateBudgetCategoryCommand",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return response;
                }
                else
                {
                    // If the BudgetCategoryName entity was not found, return 404 Not Found response with an error message
                      errorMessage = $"BudgetCategory {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateBudgetCategoryCommand",
                request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetCategoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                  errorMessage = $"Error occurred while updating BudgetCategoryName: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateBudgetCategoryCommand",
          request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<BudgetCategoryDto>.Return500(e);
            }
        }
    }
}