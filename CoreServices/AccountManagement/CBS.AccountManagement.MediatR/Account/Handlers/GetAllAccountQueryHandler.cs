using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAllAccountQueryHandler : IRequestHandler<GetAllAccountQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountQueryHandler( UserInfoToken userInfoToken,
            IAccountRepository AccountRepository,
            IMapper mapper, ILogger<GetAllAccountQueryHandler> logger)
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
    
        public async Task<ServiceResponse<List<AccountDto>>> Handle(GetAllAccountQuery request, CancellationToken cancellationToken)
        {
            List <AccountDto>  listAccountData = new List<AccountDto>();
            try
            {
                var entities = new List<Data.Account>();
                // Retrieve all Accounts entities from the repository&&
                if (_userInfoToken.IsHeadOffice)
                {
                  
                        entities = await _AccountRepository.All.Where(x => x.IsDeleted.Equals(false) ).ToListAsync();

                  

                }
                else
                {
                    entities = await _AccountRepository.All.Where(x => x.IsDeleted.Equals(false) && x.AccountOwnerId == _userInfoToken.BranchId).ToListAsync();

                }
                string    errorMessage = $"Return AccountDto with a success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByAccountNumberQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                listAccountData = _mapper.Map(entities, listAccountData); 
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(listAccountData);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }


    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAllLiasonAccountByBranchIdQueryHandler : IRequestHandler<GetAllLiasonAccountByBranchIdQuery, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLiasonAccountByBranchIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLiasonAccountByBranchIdQueryHandler(UserInfoToken userInfoToken,
            IAccountRepository AccountRepository,
            IMapper mapper, ILogger<GetAllLiasonAccountByBranchIdQueryHandler> logger)
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

        public async Task<ServiceResponse<List<AccountDto>>> Handle(GetAllLiasonAccountByBranchIdQuery request, CancellationToken cancellationToken)
        {
            List<AccountDto> listAccountData = new List<AccountDto>();
            try
            {
                var entities = new List<Data.Account>();
                // Retrieve all Accounts entities from the repository&&
                
                    entities = await _AccountRepository.All.Where(x => x.IsDeleted.Equals(false) && x.Account3 == "451").ToListAsync();

                
                string errorMessage = $"Return AccountDto with a success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByAccountNumberQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                listAccountData = _mapper.Map(entities, listAccountData);
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(listAccountData);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AccountDto>>.Return500(e, "Failed to get all Accounts");
            }
        }
    }
}