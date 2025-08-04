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
    public class AddBudgetCategoryCommandHandler : IRequestHandler<AddBudgetCategoryCommand, ServiceResponse<BudgetCategoryDto>>
    {
        private readonly IBudgetCategoryRepository _BudgetCategoryNameRepository; // Repository for accessing BudgetCategoryName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBudgetCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddBudgetCategoryNameCommandHandler.
        /// </summary>
        /// <param name="BudgetCategoryNameRepository">Repository for BudgetCategoryName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBudgetCategoryCommandHandler(
            IBudgetCategoryRepository BudgetCategoryNameRepository,
            IMapper mapper,
            ILogger<AddBudgetCategoryCommandHandler> logger,
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
        public async Task<ServiceResponse<BudgetCategoryDto>> Handle(AddBudgetCategoryCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = "";
             try
            {
                // Check if a BudgetCategoryName with the same name already exists (case-insensitive)
                var existingBudgetCategoryName =   _BudgetCategoryNameRepository.All.Where(c => c.CategoryName == request.CategoryName&&c.IsDeleted==false && c.BranchId==_userInfoToken.BranchId);

                // If a BudgetCategoryName with the same name already exists, return a conflict response
                if (existingBudgetCategoryName.Any())
                {
                      errorMessage = $"BudgetCategory of {request.CategoryName} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCategoryCommand",
                      request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
       
                    return ServiceResponse<BudgetCategoryDto>.Return409(errorMessage);
                }

                // Map the AddBudgetCategoryNameAttributesCommand to a BudgetCategoryNameAttributes entity
                var BudgetCategoryNameAttributesEntity = _mapper.Map<BudgetCategory>(request);
                BudgetCategoryNameAttributesEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "BGTC");
                _BudgetCategoryNameRepository.Add(BudgetCategoryNameAttributesEntity);
                await _uow.SaveAsync();
                errorMessage = $"BudgetCategory of {request.CategoryName} was successfully created.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddBudgetCategoryCommand",
                        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);


                // Map the BudgetCategoryName entity to BudgetCategoryNameDto and return it with a success response
                var BudgetCategoryNameDto = _mapper.Map<BudgetCategoryDto>(BudgetCategoryNameAttributesEntity);
                return ServiceResponse<BudgetCategoryDto>.ReturnResultWith200(BudgetCategoryNameDto);
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