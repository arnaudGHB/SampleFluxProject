using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new AccountCategory.
    /// </summary>
    public class AddChartOfAccountManagementPositionCommandHandler : IRequestHandler<AddChartOfAccountManagementPositionCommand, ServiceResponse<ChartOfAccountManagementPositionDto>>
    {
 
        private readonly IChartOfAccountManagementPositionRepository _accountCategoryRepository; // Repository for accessing AccountCategory data.
        private readonly IChartOfAccountRepository _chartOfAccountRepository; // Repository for accessing AccountCategory data.

        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddChartOfAccountManagementPositionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the AddAccountClassCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddChartOfAccountManagementPositionCommandHandler(IChartOfAccountManagementPositionRepository AccountCartegoryRepository, 
            ILogger<AddChartOfAccountManagementPositionCommandHandler> logger,
          IChartOfAccountRepository chartOfAccountRepository,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> work)
        {
            _accountCategoryRepository = AccountCartegoryRepository;
            _chartOfAccountRepository= chartOfAccountRepository;
               _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = work;
        }

        /// <summary>
        /// Handles the AddChartOfAccountManagementPositionCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddChartOfAccountManagementPositionCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChartOfAccountManagementPositionDto>> Handle(AddChartOfAccountManagementPositionCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Check if a AccountCategory with the same name already exists (case-insensitive)
                var existingAccountCategory =  _accountCategoryRepository.FindBy(c=>c.IsDeleted == false&&c.ChartOfAccountId==request.ChartOfAccountId).ToList();

                request.PositionNumber = request.PositionNumber.PadRight(3, '0');
                
                 var existingAccountCategoryx = existingAccountCategory.Where(c =>  c.Description == request.Description && c.IsDeleted == false);

                // If a AccountCategory with the same name already exists, return a conflict response
                if (existingAccountCategoryx.Any())
                {
                    var errorMessage = $"ChartOfAccountManagementPosition {request.Description} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<ChartOfAccountManagementPositionDto>.Return409(errorMessage);
                }

                var existingAccountCategor = existingAccountCategory.Where(c => c.PositionNumber == request.PositionNumber && c.IsDeleted == false);

                // If a AccountCategory with the same name already exists, return a conflict response
                if (existingAccountCategor.Any())
                {
                    var errorMessage = $"ChartOfAccountManagementPosition {request.PositionNumber} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<ChartOfAccountManagementPositionDto>.Return409(errorMessage);
                }

                var _chartOfAccount = await _chartOfAccountRepository.FindAsync(request.ChartOfAccountId);
                request.Description = request.Description.ToUpper();
                // If a AccountCategory with the same name already exists, return a conflict response
                
                // Map the AddSubAccountCartegoryCommand to a SubAccountCartegory entity
                var Entity = _mapper.Map<ChartOfAccountManagementPosition>(request);
                Entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "CMP");
                Entity.AccountNumber= _chartOfAccount.AccountNumber;
                Entity.TempData = _chartOfAccount.AccountNumber.PadRight(6,'0')+ request.PositionNumber + "000";
                Entity.New_AccountNumber = _chartOfAccount.AccountNumber.PadRight(6, '0') + request.PositionNumber + "000";
                Entity.Old_AccountNumber =  "000000000000" ;
                Entity.IsUniqueAccount = false;
                // Add the new AccountClass entity to the repository

                _accountCategoryRepository.Add(Entity);
             
             
                    await _uow.SaveAsync();
               
        
               
                // Map the AccountClass entity to ChartOfAccountManagementPositionDto and return it with a success response
                var AccountClassDto = _mapper.Map<ChartOfAccountManagementPositionDto>(Entity);
                return ServiceResponse<ChartOfAccountManagementPositionDto>.ReturnResultWith200(AccountClassDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving ChartOfAccountManagementPosition: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ChartOfAccountManagementPositionDto>.Return500(e+errorMessage);
            }
        }
    


    
    }


}