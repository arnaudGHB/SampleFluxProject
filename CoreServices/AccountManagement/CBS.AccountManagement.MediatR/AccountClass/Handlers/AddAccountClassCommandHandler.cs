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
    public class AddAccountClassCommandHandler : IRequestHandler<AddAccountClassCommand, ServiceResponse<AccountClassDto>>
    {
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountCategory data.

        private readonly IAccountCategoryRepository _accountCategoryRepository; // Repository for accessing AccountCategory data.

        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountClassCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the AddAccountClassCommandHandler.
        /// </summary>
        /// <param name="AccountCategoryRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountClassCommandHandler(IAccountCategoryRepository AccountCartegoryRepository, IAccountClassRepository AccountClassRepository,
            ILogger<AddAccountClassCommandHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> work)
        {
            _accountCategoryRepository = AccountCartegoryRepository;
            _AccountClassRepository = AccountClassRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _uow = work;
        }

        /// <summary>
        /// Handles the AddAccountClassCommand to add a new AccountClass.
        /// </summary>
        /// <param name="request">The AddAccountClassCommand containing AccountClass data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountClassDto>> Handle(AddAccountClassCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AccountCategory with the same name already exists (case-insensitive)
                var existingAccountCategory = await _AccountClassRepository.FindBy(c => c.AccountNumber == request.AccountNumber && c.IsDeleted == false && c.AccountCategoryId == request.AccountCategoryId).FirstOrDefaultAsync();

                // If a AccountCategory with the same name already exists, return a conflict response
                //if (existingAccountCategory != null)
                //{
                //    var errorMessage = $"AccountClass {request.AccountNumber} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<AccountClassDto>.Return409(errorMessage);
                //}

                // Map the AddSubAccountCartegoryCommand to a SubAccountCartegory entity
                var Entity = _mapper.Map<AccountClass>(request);
                // Convert UTC to local time and set it in the entity
                Entity = AccountClass.SetAccountClassEntity(Entity, _userInfoToken);
                Entity.AccountCategoryId = request.AccountCategoryId;
                // Add the new AccountClass entity to the repository
 
                _AccountClassRepository.Add(Entity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<AccountClassDto>.Return500();
                }
                // Map the AccountClass entity to AccountClassDto and return it with a success response
                var AccountClassDto = _mapper.Map<AccountClassDto>(Entity);
                return ServiceResponse<AccountClassDto>.ReturnResultWith200(AccountClassDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountClass: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountClassDto>.Return500(e+errorMessage);
            }
        }
    }
}