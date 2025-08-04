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
    public class AddAccountCartegoriesCommandHandler : IRequestHandler<AddAccountCartegoriesCommand, ServiceResponse<List<AccountCartegoryDto>>>
    {
        private readonly IAccountCategoryRepository _AccountCategoryRepository; // Repository for accessing AccountCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountCategoryCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountCategory data.

        /// <summary>
        /// Constructor for initializing the AddAccountCategoryCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountCartegoriesCommandHandler(
            IAccountCategoryRepository AccountCategoryRepository,
             UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<AddAccountCategoryCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IAccountClassRepository? accountClassRepository)
        {
            _AccountCategoryRepository = AccountCategoryRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _AccountClassRepository = accountClassRepository;

        }

        /// <summary>
        /// Handles the AddAccountCategoryCommand to add a new AccountCategory.
        /// </summary>
        /// <param name="request">The AddAccountCategoryCommand containing AccountCategory data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountCartegoryDto>>> Handle(AddAccountCartegoriesCommand request, CancellationToken cancellationToken)
        {
            List<AccountCartegoryDto> ListCartegories = new List<AccountCartegoryDto>();
            List<AccountClass> ClassOfAccounts = new List<AccountClass>();
            try
            {
                foreach (var item in request.AccountCartegories)
                {
                    var existingAccountCategory = await _AccountCategoryRepository.FindBy(c => c.Name == item.Name && c.IsDeleted == false).FirstOrDefaultAsync();

                    // If a AccountCategory with the same name already exists, return a conflict response
                    if (existingAccountCategory != null)
                    {
                        var errorMessage = $"AccountCategory {item.Name} already exists.";
                        _logger.LogError(errorMessage);
                        //return ServiceResponse<AccountCartegoryDto>.Return409(errorMessage);
                    }

                    // Map the AddAccountCartegoryCommand to a AccountCartegory entity
                    var AccountCartegoryEntity = _mapper.Map<AccountCategory>(item);
                    // Convert UTC to local time and set it in the entity
                    AccountCartegoryEntity = this.SetAccountCategoryEntity(AccountCartegoryEntity, _userInfoToken);
                    // Add the new AccountCategory entity to the repository
                    AccountCartegoryEntity.Id = item.Id;
                    _AccountCategoryRepository.Add(AccountCartegoryEntity);

                    var AccountCategoryDto = _mapper.Map<AccountCartegoryDto>(AccountCartegoryEntity);

                    ListCartegories.Add(AccountCategoryDto);
                }
                // Check if a AccountCategory with the same name already exists (case-insensitive)
                int count = 0; AccountClass model = null;
                foreach (var item in ListCartegories)
                {
                    if (item.Name.ToLower() == "equity")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "9"

                        };
                    }
                    else if (item.Name.ToLower() == "liability")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "3"

                        };
                        ClassOfAccounts.Add(model);
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "4"

                        };
                        ClassOfAccounts.Add(model);
                    }
                    else if (item.Name.ToLower() == "asset")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "4"

                        };
                        ClassOfAccounts.Add(model);
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "1"

                        };
                        ClassOfAccounts.Add(model);
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "5"

                        };
                        ClassOfAccounts.Add(model);
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "2"

                        };
                        ClassOfAccounts.Add(model);
                    }
                    else if (item.Name.ToLower() == "revenue")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "7"

                        };
                        ClassOfAccounts.Add(model);
                    }
                    else if (item.Name.ToLower() == "expense")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "6"

                        };
                        ClassOfAccounts.Add(model);
                    }
                    else if (item.Name.ToLower() == "extra-ordinary")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "8"

                        };
                        ClassOfAccounts.Add(model);
                    }

                    else if (item.Name.ToLower() == "off-balance-sheet")
                    {
                        model = new AccountClass
                        {
                            Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AC"),
                            AccountCategoryId = item.Id,
                            AccountNumber = "9"

                        };
                        ClassOfAccounts.Add(model);
                    }

                }
                _AccountClassRepository.AddRange(ClassOfAccounts);
                await _uow.SaveAsync();
                return ServiceResponse<List<AccountCartegoryDto>>.ReturnResultWith200(ListCartegories);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountCategory: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AccountCartegoryDto>>.Return500(e);
            }
        }

        public List<AccountCategory> SetAccountCategoriesEntities(List<AccountCategory> accountCategories, UserInfoToken _userInfoToken)
        {
            List<AccountCategory> categories = new List<AccountCategory>();
            foreach (AccountCategory entity in accountCategories)
            {
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
                entity.ModifiedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
                entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CT");
                categories.Add(entity);
            }
            return categories;
        }

        public AccountCategory SetAccountCategoryEntity(AccountCategory entity, UserInfoToken _userInfoToken)
        {


            entity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
            entity.ModifiedDate = BaseUtilities.UtcToLocal(DateTime.Now.ToLocalTime());
            entity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "CT");


            return entity;
        }

    }
}