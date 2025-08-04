using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.BankingOperation.Handlers
{
    public class GetTransactionRequestReversalDataQueryHandler : IRequestHandler<GetTransactionRequestReversalDataQuery, ServiceResponse<TransactionReversalRequestDataDto>>
    {
        private readonly ITransactionReversalRequestDataRepository _transactionReversalRequestDataRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransactionRequestReversalDataQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        
        public GetTransactionRequestReversalDataQueryHandler(
          ITransactionReversalRequestDataRepository transactionReversalRequestRepository,
          IMapper mapper,
          ILogger<GetTransactionRequestReversalDataQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _transactionReversalRequestDataRepository = transactionReversalRequestRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        
        public async Task<ServiceResponse<TransactionReversalRequestDataDto>> Handle(GetTransactionRequestReversalDataQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entitiesAccounts = await _transactionReversalRequestDataRepository.FindAsync(request.Id);
                if (entitiesAccounts == null)
                {
                    errorMessage = $"No TransactionReversalRequestData was found";
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetTransactionRequestReversalDataQuery",
                        request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return  ServiceResponse<TransactionReversalRequestDataDto>.Return404(errorMessage);
                }
                else
                {
                   var _mapped = _mapper.Map<TransactionReversalRequestDataDto>(entitiesAccounts);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetTransactionRequestReversalDataQuery",
                      request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<TransactionReversalRequestDataDto>.ReturnResultWith200(_mapped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetTransactionRequestReversalDataQuery",
                    request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<TransactionReversalRequestDataDto>.Return500(e);
            }
        }
    }
}
