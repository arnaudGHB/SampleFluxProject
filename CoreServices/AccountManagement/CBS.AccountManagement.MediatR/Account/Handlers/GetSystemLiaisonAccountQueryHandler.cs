using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetSystemLiaisonAccountQueryHandler : IRequestHandler<GetSystemLiaisonAccountQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSystemLiaisonAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly string BranchId = "XXXXXX";
        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSystemLiaisonAccountQueryHandler( UserInfoToken userInfoToken,
            IAccountRepository AccountRepository,
            IMapper mapper, ILogger<GetSystemLiaisonAccountQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllAccountQuery to retrieve all Accounts.
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
    
        public async Task<ServiceResponse<List<AccountDto>>> Handle(GetSystemLiaisonAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List < Data.Account > entities = new List < Data.Account > ();
                // Retrieve all Accounts entities from the repository
                if (request.BranchId.Equals(BranchId))
                {
                      entities = await _AccountRepository.All.Where(x =>x.IsDeleted.Equals(false) && x.AccountNumber.StartsWith("451")).ToListAsync();

                }
                else
                {
                      entities = await _AccountRepository.All.Where(x => x.BranchId.Equals(request.BranchId) && x.IsDeleted.Equals(false) && x.AccountNumber.StartsWith("451")).ToListAsync();

                }
            
                string errorMessage = $"Return AccountDto with a success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByMFIBankAccountNumberQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(AccountMapper.Map(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByMFIBankAccountNumberQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}