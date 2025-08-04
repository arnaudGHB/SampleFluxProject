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
    /// Handles the command to add a new AccountType.
    /// </summary>
    public class AddAccountTypeCommandHandler : IRequestHandler<AddAccountTypeCommand, ServiceResponse<AccountTypeDto>>
    {
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddAccountTypeCommandHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountTypeCommandHandler(
            IAccountTypeRepository AccountTypeRepository,
            IMapper mapper,
            UserInfoToken userInfoToken,
            ILogger<AddAccountTypeCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _AccountTypeRepository = AccountTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountTypeCommand to add a new AccountType.
        /// </summary>
        /// <param name="request">The AddAccountTypeCommand containing AccountType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountTypeDto>> Handle(AddAccountTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AccountType with the same name already exists (case-insensitive)
                var existingAccountType = await _AccountTypeRepository.FindBy(c => c.Name == request.name).FirstOrDefaultAsync();

                // If a AccountType with the same name already exists, return a conflict response
                if (existingAccountType != null)
                {
                    var errorMessage = $"AccountType {request.name} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountTypeCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<AccountTypeDto>.Return409(errorMessage);
                }

                // Map the AddAccountTypeCommand to a AccountType entity
                var AccountTypeEntity = _mapper.Map<Data.AccountType>(request);
                // Convert UTC to local time and set it in the entity
                //AccountTypeEntity = AccountType.SetAccountAccountTypeEntity(AccountTypeEntity, _userInfoToken);
                // Add the new AccountType entity to the repository
                _AccountTypeRepository.Add(AccountTypeEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    var errorMessage = $"A ROLLBACK processed occured during the AccountType action.";
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountTypeCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<AccountTypeDto>.Return500();
                }
                // Map the AccountType entity to AccountTypeDto and return it with a success response
                var AccountTypeDto = _mapper.Map<AccountTypeDto>(AccountTypeEntity);
                var errorMessag = $"new AccountType created Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommand",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<AccountTypeDto>.ReturnResultWith200(AccountTypeDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountType: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountTypeCommand",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AccountTypeDto>.Return500(e);
            }
        }
    }
}