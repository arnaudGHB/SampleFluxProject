using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
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
    public class GetBankTransactionByIdQueryHandler : IRequestHandler<GetBankTransactionByReferenceIdQuery, ServiceResponse<BankTransactionDto>>
    {
        private readonly ICashReplenishmentRepository _cashReplenishmentRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBankTransactionByIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IBankTransactionRepository _bankTransactionRepository;
        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBankTransactionByIdQueryHandler(
            IBankTransactionRepository bankTransactionRepository,
            ICashReplenishmentRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<GetBankTransactionByIdQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _bankTransactionRepository= bankTransactionRepository;  
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
        public async Task<ServiceResponse<BankTransactionDto>> Handle(GetBankTransactionByReferenceIdQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entitiesAccounts =   _bankTransactionRepository.FindBy(cop=>cop.ReferenceId.Equals(request.ReferenceId));
                if (entitiesAccounts.Any())
                {
                    var _mapped = _mapper.Map<BankTransactionDto>(entitiesAccounts.FirstOrDefault());
                    return ServiceResponse<BankTransactionDto>.ReturnResultWith200(_mapped);
                   
                }
                else
                {
                    errorMessage = $"No Banktransaction was found with Id:{request.ReferenceId}";

                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetCashReplenishmentQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<BankTransactionDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByReferenceQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<BankTransactionDto>.Return500(e);
            }
        }
    }
}
