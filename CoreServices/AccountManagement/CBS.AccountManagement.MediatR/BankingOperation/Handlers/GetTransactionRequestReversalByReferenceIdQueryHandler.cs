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

namespace CBS.AccountManagement.MediatR.BankingOperation.Handlers
{
    public class GetTransactionRequestReversalByReferenceIdQueryHandler : IRequestHandler<GetTransactionRequestReversalByReferenceIdQuery, ServiceResponse<TransactionReversalRequestDto>>
    {
        private readonly ITransactionReversalRequestRepository _transactionReversalRequestRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransactionRequestReversalByReferenceIdQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;


        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTransactionRequestReversalByReferenceIdQueryHandler(
            ITransactionReversalRequestRepository transactionReversalRequestRepository,
            IMapper mapper,
            ILogger<GetTransactionRequestReversalByReferenceIdQueryHandler> logger, UserInfoToken userInfoToken)
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
        public async Task<ServiceResponse<TransactionReversalRequestDto>> Handle(GetTransactionRequestReversalByReferenceIdQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entitiesAccounts = _transactionReversalRequestRepository.FindBy(x=>x.ReferenceId.Equals(request.ReferenceId)&&x.IsDeleted.Equals(false)).FirstOrDefault();
                if (entitiesAccounts == null)
                {
                    errorMessage = $"No TransactionReversal Request was found";
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetTransactionRequestReversalByReferenceIdQuery",
                 request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return  ServiceResponse<TransactionReversalRequestDto>.Return404(errorMessage);
                }
                else
                {
                   var _mapped = _mapper.Map<TransactionReversalRequestDto>(entitiesAccounts);
                    return ServiceResponse<TransactionReversalRequestDto>.ReturnResultWith200(_mapped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByReferenceQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionReversalRequestDto>.Return500(e);
            }
        }
    }
}
