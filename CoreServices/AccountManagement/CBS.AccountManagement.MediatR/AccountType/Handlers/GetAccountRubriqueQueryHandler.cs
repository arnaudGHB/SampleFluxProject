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
    ///// </summary>
    //public class GetAccountRubriqueQueryHandler : IRequestHandler<GetAccountRubriqueQuery, ServiceResponse<AccountRubricDto>>
    //{
    //    private readonly IAccountRubriqueRepository _accountRubriqueRepository; // Repository for accessing AccountType data.
    //    private readonly IMapper _mapper; // AutoMapper for object mapping.
    //    private readonly UserInfoToken _userInfoToken;
    //    private readonly ILogger<GetAccountRubriqueQueryHandler> _logger; // Logger for logging handler actions and errors.

    //    /// <summary>
    //    /// Constructor for initializing the GetAccountTypeQueryHandler.
    //    /// </summary>
    //    /// <param name="AccountTypeRepository">Repository for AccountType data access.</param>
    //    /// <param name="mapper">AutoMapper for object mapping.</param>
    //    /// <param name="userInfoToken">Repository for ClaimType data access.</param>
    //    /// <param name="logger">Logger for logging handler actions and errors.</param>
    //    public GetAccountRubriqueQueryHandler(
    //        IAccountRubriqueRepository accountRubriqueRepository,
    //        IMapper mapper,
    //        ILogger<GetAccountRubriqueQueryHandler> logger,UserInfoToken userInfoToken)
    //    {
    //        _accountRubriqueRepository = accountRubriqueRepository; 
    //        _mapper = mapper;
    //        _userInfoToken = userInfoToken;
    //        _logger = logger;
    //    }

    //    /// <summary>
    //    /// Handles the GetAccountRubriqueQuery to retrieve a specific AccountType.
    //    /// </summary>
    //    /// <param name="request">The GetAccountRubriqueQuery containing AccountType ID to be retrieved.</param>
    //    /// <param name="cancellationToken">A cancellation token.</param>
    //    public async Task<ServiceResponse<AccountRubricDto>> Handle(GetAccountRubriqueQuery request, CancellationToken cancellationToken)
    //    {
    //        string errorMessage = null;
    //        try
    //        {
    //            // Retrieve the AccountType entity with the specified ID from the repository
    //            var entity = await GetExistingAccountRubric(request);
    //            if (entity != null)
    //            {
                  
    //                if (entity.IsDeleted)
    //                {
    //                     errorMessage = "AccountRubrique has been deleted.";
    //                    _logger.LogError(errorMessage);
    //                    LogAndAuditError(request, errorMessage, LogLevelInfo.Information , 404);
    //                    return ServiceResponse<AccountRubricDto>.Return404(errorMessage);
    //                }
    //                else
    //                {
    //                    // Map the AccountRubrique entity to AccountRubriqueDto and return it with a success response
    //                    var AccountRubriqueDto = _mapper.Map<AccountRubricDto>(entity);
    //                    string successMessage = $"Gotten AccountRubrique for productId {request.Id} successfully.";
    //                    LogAuditSuccess( request, successMessage);
                        
    //                    return ServiceResponse<AccountRubricDto>.ReturnResultWith200(AccountRubriqueDto);
                        
    //                }
    //            }
    //            else
    //            {
    //                // If the AccountType entity was not found, log the error and return a 404 Not Found response
    //                errorMessage="AccountType not found.";
    //                _logger.LogError(errorMessage);
    //                LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  500 );
    //                return ServiceResponse<AccountRubricDto>.Return404(errorMessage);
                    
    //            }
    //        }
    //        catch (Exception e)
    //        {
    //            errorMessage = $"Error occurred while getting AccountType: {e.Message}";
    //            LogAndAuditError(request, errorMessage, LogLevelInfo.Error,   500);
    //            _logger.LogError(errorMessage);
    //            return ServiceResponse<AccountRubricDto>.Return404(errorMessage);
 
    //        }
    //    }
    //    private async Task<AccountRubrique> GetExistingAccountRubric(GetAccountRubriqueQuery request)
    //    {
    //        return await _accountRubriqueRepository.FindAsync(request.Id);
    //    }

    //        private void LogAndAuditError(GetAccountRubriqueQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
    //    {
    //        _logger.LogError(errorMessage);
    //        APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountRubriqueQuery",
    //            JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
    //    }

    //    private void LogAuditSuccess(GetAccountRubriqueQuery request, string successMessage)
    //    {
          
    //        APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountRubriqueQuery",
    //            JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
    //    }
    //}
}