using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Account based on its unique identifier.
    /// </summary>
    public class GetAccountByBranchIDQueryHandler : IRequestHandler<GetAccountByBranchIDQuery, ServiceResponse<AccountDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountByBranchIDQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountByBranchIDQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetAccountByBranchIDQueryHandler> logger,UserInfoToken userInfoToken)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountDto>> Handle(GetAccountByBranchIDQuery  request, CancellationToken cancellationToken)
        {
            string BranchID = _userInfoToken.IsHeadOffice? request.BranchID: _userInfoToken.BranchId;
            string errorMessage = null;
            try
            {
               
                // Retrieve the Account entity with the specified ID from the repository
                var Account = _AccountRepository.FindBy(x=>x.Id.Equals(BranchID) && x.IsDeleted==false).FirstOrDefault();
                if (Account != null)
                {
                    errorMessage = $"Return AccountDto with a success response";
                        var AccountDtos = _mapper.Map<AccountDto>(Account);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByBranchIDQuery",
                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return ServiceResponse<AccountDto>.ReturnResultWith200(AccountDtos);
                }
                else
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    errorMessage = "Account not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByBranchIDQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<AccountDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByAccountNumberQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<AccountDto>.Return500(e);
            }
        }
    }
}