using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new BudgetCategoryName.
    /// </summary>
    public class BlockBudgetCategoryCommandHandler : IRequestHandler<BlockBudgetCategoryCommand, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryNameRepository; // Repository for accessing BudgetCategoryName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<BlockBudgetCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddBudgetCategoryNameCommandHandler.
        /// </summary>
        /// <param name="BudgetCategoryNameRepository">Repository for BudgetCategoryName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public BlockBudgetCategoryCommandHandler(
            IBudgetCategoryRepository BudgetCategoryNameRepository,
            IMapper mapper,
            ILogger<BlockBudgetCategoryCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _BudgetCategoryNameRepository = BudgetCategoryNameRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

        /// <summary>
        /// Handles the AddBudgetCategoryNameCommand to add a new BudgetCategoryName.
        /// </summary>
        /// <param name="request">The AddBudgetCategoryNameCommand containing BudgetCategoryName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(BlockBudgetCategoryCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = "";
             try
            {
                // Check if a BudgetCategoryName with the same name already exists (case-insensitive)
                var existingBudgetCategory =   _BudgetCategoryNameRepository.All.Where(c => c.Id == request.Id&&c.IsDeleted==false && c.BranchId==_userInfoToken.BranchId);

                // Check if the BudgetCategoryName entity exists
                if (existingBudgetCategory.Any())
                {
                    var existingBudgetCategoryName = existingBudgetCategory.FirstOrDefault();
                    // Update BudgetCategoryName entity properties with values from the request
                    //existingBudgetCategoryName.IsBlocked = true;
                    //existingBudgetCategoryName.BlockedBy = _userInfoToken.Id;
                    // Use the repository to update the existing BudgetCategoryName entity
                    _BudgetCategoryNameRepository.Update(existingBudgetCategoryName);
                    await _uow.SaveAsync();

                    errorMessage = $"BudgetCategory {request.Id} was successfully blocked.";
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(_mapper.Map<BudgetCategoryDto>(existingBudgetCategoryName));
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "BlockBudgetCategoryCommand",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return response;
                }
                else
                {
                    // If the BudgetCategoryName entity was not found, return 404 Not Found response with an error message
                    errorMessage = $"BudgetCategory {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "BlockBudgetCategoryCommand",
                request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BudgetCategoryDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while saving BudgetCategoryName: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCategoryCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);


                return ServiceResponse<BudgetCategoryDto>.Return500(e);
            }
        }
    }
}