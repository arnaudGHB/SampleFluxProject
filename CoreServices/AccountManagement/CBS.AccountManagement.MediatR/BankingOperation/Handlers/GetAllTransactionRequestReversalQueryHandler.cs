using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class GetAllTransactionRequestReversalQueryHandler : IRequestHandler<GetAllTransactionRequestReversalQuery, ServiceResponse<List<TransactionReversalRequestDto>>>
    {
        private readonly ITransactionReversalRequestRepository _transactionReversalRequestRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTransactionRequestReversalQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTransactionRequestReversalQueryHandler(
            ITransactionReversalRequestRepository transactionReversalRequestRepository,
            IMapper mapper,
            ILogger<GetAllTransactionRequestReversalQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _transactionReversalRequestRepository = transactionReversalRequestRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransactionReversalRequestDto>>> Handle(GetAllTransactionRequestReversalQuery request, CancellationToken cancellationToken)
        {
            List<TransactionReversalRequestDto> _listMpped = new List<TransactionReversalRequestDto>();
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                List <TransactionReversalRequest> entitiesAccounts = new List<TransactionReversalRequest>();
                switch (request.SearchOption.ToUpper())
                {
                    case SearchOptions.HeadOffice:
                        entitiesAccounts = await _transactionReversalRequestRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync(); break;
                    case SearchOptions.Zone:
                        entitiesAccounts = await _transactionReversalRequestRepository.All.Where(x => x.BranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false)).ToListAsync(); break;
                    case SearchOptions.Branch:
                        entitiesAccounts = await _transactionReversalRequestRepository.All.Where(x => x.BranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false)).ToListAsync(); break;
                    case SearchOptions.User:
                        entitiesAccounts = await _transactionReversalRequestRepository.All.Where(x => x.CreatedBy.Equals(_userInfoToken.Id) && x.BranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false)).ToListAsync(); break;
                }
                if (entitiesAccounts == null)
                {
                    errorMessage = $"No cash replenishment request was found";
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetCashReplenishmentQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return  ServiceResponse<List<TransactionReversalRequestDto>>.Return404(errorMessage);
                }
                else
                {
                  _listMpped = _mapper.Map(entitiesAccounts, _listMpped);
                    return ServiceResponse<List<TransactionReversalRequestDto>>.ReturnResultWith200(_listMpped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByReferenceQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<TransactionReversalRequestDto>>.Return500(e);
            }
        }
    }
}
