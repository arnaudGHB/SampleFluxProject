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
    /// Handles the command to add a new AccountPolicyName.
    /// </summary>
    public class AddAccountPolicyCommandHandler : IRequestHandler<AddAccountPolicyCommand, ServiceResponse<AccountPolicyDto>>
    {
        private readonly IAccountPolicyRepository _AccountPolicyNameRepository; // Repository for accessing AccountPolicyName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountPolicyCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddAccountPolicyNameCommandHandler.
        /// </summary>
        /// <param name="AccountPolicyNameRepository">Repository for AccountPolicyName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountPolicyCommandHandler(
            IAccountPolicyRepository AccountPolicyNameRepository,
            IMapper mapper,
            ILogger<AddAccountPolicyCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _AccountPolicyNameRepository = AccountPolicyNameRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountPolicyNameCommand to add a new AccountPolicyName.
        /// </summary>
        /// <param name="request">The AddAccountPolicyNameCommand containing AccountPolicyName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountPolicyDto>> Handle(AddAccountPolicyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AccountPolicyName with the same name already exists (case-insensitive)
                var existingAccountPolicyName =   _AccountPolicyNameRepository.All.Where(c => c.AccountId == request.AccountId).ToList();

                // If a AccountPolicyName with the same name already exists, return a conflict response
                if (existingAccountPolicyName.Any())
                {
                    var errorMessage = $"AccountPolicy already exists on this account.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<AccountPolicyDto>.Return409(errorMessage);
                }

                // Map the AddAccountPolicyNameAttributesCommand to a AccountPolicyNameAttributes entity
                var AccountPolicyNameAttributesEntity = _mapper.Map<AccountPolicy>(request);

                AccountPolicyNameAttributesEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(8, "AP");
                _AccountPolicyNameRepository.Add(AccountPolicyNameAttributesEntity);
                await _uow.SaveAsync();
              
                // Map the AccountPolicyName entity to AccountPolicyNameDto and return it with a success response
                var AccountPolicyNameDto = _mapper.Map<AccountPolicyDto>(AccountPolicyNameAttributesEntity);
                return ServiceResponse<AccountPolicyDto>.ReturnResultWith200(AccountPolicyNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountPolicyName: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountPolicyDto>.Return500(e);
            }
        }
    }
}