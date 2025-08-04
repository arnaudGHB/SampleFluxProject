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

namespace CBS.AccountManagement.MediatR.BankingOperation.Handlers
{
    public class GetAllCashRequestApprovalQueryHandler : IRequestHandler<GetAllCashRequestApprovalQuery, ServiceResponse<List<CashReplenishmentDto>>>
    {
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCashRequestApprovalQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCashRequestApprovalQueryHandler(
            ICashReplenishmentRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<GetAllCashRequestApprovalQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _cashReplenishmentRepository = cashReplenishmentRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CashReplenishmentDto>>> Handle(GetAllCashRequestApprovalQuery request, CancellationToken cancellationToken)
        {
            List<CashReplenishmentDto> _listMpped = new List<CashReplenishmentDto>();
            List<CashReplenishment> entitiesAccounts = new List<CashReplenishment>();
          string errorMessage = null;
            try
            {
                if (_userInfoToken.IsHeadOffice)
                {
                    entitiesAccounts = await _cashReplenishmentRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                }
                else 
                {


                    entitiesAccounts = await _cashReplenishmentRepository.All.Where(x => x.CorrespondingBranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false)).ToListAsync();

                }

                if (entitiesAccounts == null)
                {
                    errorMessage = $"No cash replenishment request was found";
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetCashReplenishmentQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return  ServiceResponse<List<CashReplenishmentDto>>.Return404(errorMessage);
                }
                else
                {
                  _listMpped = _mapper.Map(entitiesAccounts, _listMpped);
                    return ServiceResponse<List<CashReplenishmentDto>>.ReturnResultWith200(_listMpped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByReferenceQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<CashReplenishmentDto>>.Return500(e);
            }
        }
    }
}
