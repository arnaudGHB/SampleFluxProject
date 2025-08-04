using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountType based on its unique identifier.
    /// </summary>
    public class GetAccountTypeQueryHandler : IRequestHandler<GetAccountTypeQuery, ServiceResponse<AccountTypeDto>>
    {
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetAccountTypeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="userInfoToken">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountTypeQueryHandler(
            IAccountTypeRepository AccountTypeRepository,
            IMapper mapper,
            ILogger<GetAccountTypeQueryHandler> logger,UserInfoToken userInfoToken)
        {
            _AccountTypeRepository = AccountTypeRepository; 
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAccountTypeQuery to retrieve a specific AccountType.
        /// </summary>
        /// <param name="request">The GetAccountTypeQuery containing AccountType ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountTypeDto>> Handle(GetAccountTypeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the AccountType entity with the specified ID from the repository
                var entity = await GetExistingAccountType(request);
                if (entity != null)
                {
                  
                    if (entity.IsDeleted)
                    {
                         errorMessage = "AccountType has been deleted.";
                        _logger.LogError(errorMessage);
                        LogAndAuditError(request, errorMessage, LogLevelInfo.Information, request.IdType == "ACCOUNTTYPE" ? 409 : 404);
                        return ServiceResponse<AccountTypeDto>.Return404(errorMessage);
                    }
                    else
                    {
                        // Map the AccountType entity to AccountTypeDto and return it with a success response
                        var AccountTypeDto = _mapper.Map<AccountTypeDto>(entity);
                        string successMessage = $"Gotten AccountType for productId {request.IdType} successfully.";
                        LogAuditSuccess( request, successMessage);
                        
                        return ServiceResponse<AccountTypeDto>.ReturnResultWith200(AccountTypeDto);
                        
                    }
                }
                else
                {
                    // If the AccountType entity was not found, log the error and return a 404 Not Found response
                    errorMessage="AccountType not found.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Error, request.IdType == "ACCOUNTTYPE" ? 500 : 400);
                    return ServiceResponse<AccountTypeDto>.Return404(errorMessage);
                    
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Error, request.IdType == "ACCOUNTTYPE" ? 500 : 500);
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountTypeDto>.Return404(errorMessage);
 
            }
        }
        private async Task<AccountType> GetExistingAccountType(GetAccountTypeQuery request)
        {
            return await _AccountTypeRepository.FindAsync(request.Id);
               
        }

        private void LogAndAuditError(GetAccountTypeQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountTypeQuery",
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(GetAccountTypeQuery request, string successMessage)
        {
          
            APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountTypeQuery",
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}