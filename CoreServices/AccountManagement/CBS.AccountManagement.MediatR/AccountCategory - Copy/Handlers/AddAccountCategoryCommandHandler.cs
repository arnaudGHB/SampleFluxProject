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
    public class AddAccountCategoryCommandHandler : IRequestHandler<AddAccountCategoryCommand, ServiceResponse<AccountCartegoryDto>>
    {
        private readonly IAccountCategoryRepository _AccountCategoryRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddAccountCategoryCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountCategoryCommandHandler(
            IAccountCategoryRepository AccountCategoryRepository,
             UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddAccountCategoryCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _AccountCategoryRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;

        }

        /// <summary>
        /// Handles the AddAccountCategoryCommand to add a new AccountCategory.
        /// </summary>
        /// <param name="request">The AddAccountCategoryCommand containing AccountCategory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountCartegoryDto>> Handle(AddAccountCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AccountCategory with the same name already exists (case-insensitive)
                var existingAccountCategory = await _AccountCategoryRepository.FindBy(c => c.Name == request.Name&& c.IsDeleted==false).FirstOrDefaultAsync();

                // If a AccountCategory with the same name already exists, return a conflict response
                if (existingAccountCategory != null)
                {
                    var errorMessage = $"AccountCategory {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountCartegoryDto>.Return409(errorMessage);
                }

                // Map the AddAccountCartegoryCommand to a AccountCartegory entity
                var AccountCartegoryEntity = _mapper.Map<AccountCategory>(request);
                // Convert UTC to local time and set it in the entity
                AccountCartegoryEntity = this.SetAccountCategoryEntity(AccountCartegoryEntity, _userInfoToken);
                // Add the new AccountCategory entity to the repository
                _AccountCategoryRepository.Add(AccountCartegoryEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<AccountCartegoryDto>.Return500();
                }
                // Map the AccountCategory entity to AccountCategoryDto and return it with a success response
                var AccountCategoryDto = _mapper.Map<AccountCartegoryDto>(AccountCartegoryEntity);
                return ServiceResponse<AccountCartegoryDto>.ReturnResultWith200(AccountCategoryDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountCartegoryDto>.Return500(e);
            }
        }

        public  List<AccountCategory> SetAccountCategoriesEntities(List<AccountCategory> accountCategories, UserInfoToken _userInfoToken)
        {
            List<AccountCategory> categories = new List<AccountCategory>();
            foreach (AccountCategory entity in accountCategories)
            {
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.CreatedDate = BaseUtilities.UtcToLocal( DateTime.Now.ToLocalTime());
                entity.ModifiedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
                entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CT");
                categories.Add(entity);
            }
            return categories;
        }

        public  AccountCategory SetAccountCategoryEntity(AccountCategory entity, UserInfoToken _userInfoToken)
        {


            entity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
            entity.ModifiedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
            entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CT");


            return entity;
        }

    }
}