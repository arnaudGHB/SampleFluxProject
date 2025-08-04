using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class VaultTransferCommandHandler : IRequestHandler<VaultTransferCommand, ServiceResponse<bool>>
    {
  
        private readonly IMapper _mapper;
        private readonly ILogger<VaultTransferCommandHandler> _logger;
 
        private readonly ITransactionDataRepository _transactionDataRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly IConfiguration _configuration;
        private readonly IAccountingEntriesServices _accountingService;
        private readonly IMediator _mediator;
        private readonly AccountManagement.Helper.PathHelper _pathHelper;

        public VaultTransferCommandHandler(
            ITransactionDataRepository transactionDataRepository,
             IAccountingEntryRepository accountingEntryRepository,
    IOperationEventAttributeRepository operationEventAttributeRepository,
    IAccountingRuleEntryRepository accountingRuleEntryRepository,
            IMapper mapper,
             IAccountRepository accountRepository,
            IConfiguration configuration,
            ILogger<VaultTransferCommandHandler> logger,
          UserInfoToken userInfoToken,
            IChartOfAccountManagementPositionRepository chartOfAccountRepository, IAccountingEntriesServices accountingEntriesServices, IMediator mediator, PathHelper? pathHelper)
        {
           
            _mapper = mapper;
            _logger = logger;
            _pathHelper = pathHelper;
                 _transactionDataRepository = transactionDataRepository;
            _userInfoToken = userInfoToken;
            _configuration = configuration;
         
            _mediator = mediator;
            _accountingService = accountingEntriesServices;
        }

        public async Task<ServiceResponse<bool>> Handle(VaultTransferCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string strigifyData = JsonConvert.SerializeObject(request);
                var customerData = await APICallHelper.PostCashInOrCashdenominationOperation<ServiceResponse<bool>>(_pathHelper, _userInfoToken.Token, strigifyData,  _pathHelper.VaultTransferDenomination);
                await BaseUtilities.LogAndAuditAsync($"Posting CashIn Or Cash Denomination", request, HttpStatusCodeEnum.OK, LogAction.DenominationPosting, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(customerData.Data, "Accounting posting was successfull");

            }
            catch (Exception ex)
            {
                await BaseUtilities.LogAndAuditAsync($"Posting CashIn Or Cash Denomination", request, HttpStatusCodeEnum.InternalServerError, LogAction.DenominationPosting, LogLevelInfo.Error);

                return ServiceResponse<bool>.Return403(false, ex.Message);
            }
        }

        private bool CheckIfCommissionIsApplied(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsInterBankOperationPrincipalCommission == true).Any() && amountCollection.Count() > 0;
        }


 
        private OperationEventAttributeTypes GetOperationEventAttributeTypes(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return OperationEventAttributeTypes.deposit;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return OperationEventAttributeTypes.withdrawal;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return OperationEventAttributeTypes.transfer;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return OperationEventAttributeTypes.cashreplenishment;
            }
            else
            {
                return OperationEventAttributeTypes.none;
            }
        }

        private TransactionCode GetTransCode(string operationType)
        {
            if (operationType.ToLower().Contains(OperationEventAttributeTypes.deposit.ToString()))
            {
                return TransactionCode.CINT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.withdrawal.ToString()))
            {
                return TransactionCode.COUT;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.transfer.ToString()))
            {
                return TransactionCode.TRANS;
            }
            else if (operationType.ToLower().Equals(OperationEventAttributeTypes.cashreplenishment.ToString()))
            {
                return TransactionCode.CRP;
            }
            else
            {
                return TransactionCode.None;
            }
        }
        private decimal GetPrincipal(List<AmountCollection> amountCollection)
        {
            return amountCollection.Where(x => x.IsPrincipal == true).FirstOrDefault().Amount;
        }

        private async Task LogError(string message, Exception ex, MakeAccountPostingCommand command)
        {
            // Common logic for logging errors and auditing
            _logger.LogError(ex, message);
            // Assuming APICallHelper.AuditLogger is a static utility method for auditing
            await APICallHelper.AuditLogger(_userInfoToken.Email, nameof(MakeAccountPostingCommand), command, message, "Error", 500, _userInfoToken.Token);
        }



    }
}
