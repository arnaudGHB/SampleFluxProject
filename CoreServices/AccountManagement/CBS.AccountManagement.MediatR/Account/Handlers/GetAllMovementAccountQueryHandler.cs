using AutoMapper;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Helper.DataModel;

namespace CBS.AccountManagement.MediatR.Account.Handlers
{
    public class GetAccountMovementQueryHandler : IRequestHandler<GetAccountMovementBalanceQuery, ServiceResponse<double>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountMovementQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountMovementQueryHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetAccountMovementQueryHandler> logger, UserInfoToken userInfoToken,PathHelper pathHelper)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetAllMovementAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<double>> Handle(GetAccountMovementBalanceQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                string branchId = _userInfoToken.IsHeadOffice ? request.BranchId : _userInfoToken.BranchId;
                // Retrieve the Account entity with the specified ID from the repository
                var Accounts = _AccountRepository.FindBy(x => x.ChartOfAccountManagementPositionId.Equals(request.TellerSource)&&x.AccountOwnerId.Equals(branchId));
                if (Accounts.Any())
                {
                    errorMessage = $"Return AccountDto with a success response";
     
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return ServiceResponse<double>.ReturnResultWith200(Convert.ToDouble(Accounts.FirstOrDefault().CurrentBalance));
                }
                else
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    errorMessage = "Account not found.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                       request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<double>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByAccountNumberQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<double>.Return500(e);
            }
        }
    }
}
