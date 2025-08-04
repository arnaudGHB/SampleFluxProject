using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.MediatR.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.AccountManagement.Helper.DataModel;

namespace CBS.AccountManagement.MediatR 
{

    public class UploadAccountQueryHandler : IRequestHandler<UploadAccountQuery, ServiceResponse<List<AccountUploadDto>>>
    {
        // Dependencies
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<UploadAccountQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        // Constructor to inject dependencies
        public UploadAccountQueryHandler(IAccountRepository accountRepository, ILogger<UploadAccountQueryHandler> logger, IMapper mapper, UserInfoToken userInfoToken)
        {
            _accountRepository = accountRepository; 
            _logger = logger;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
        }

        // Handle method implementation
        public async Task<ServiceResponse<List<AccountUploadDto>>> Handle(UploadAccountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                List < AccountUploadDto > Modelist = new List<AccountUploadDto>();
              
                foreach (var item in request.Accounts)
                {
                    var existingAccount =   _accountRepository.FindBy(c => c.AccountNumber == item.AccountNumber);
                    if (existingAccount.Any())
                    {
                        var model = existingAccount.FirstOrDefault();
                        Modelist.Add(new AccountUploadDto(model.AccountNumber, model.AccountName,model.CurrentBalance.ToString()));
                    }
                    else
                    {
                        // Log an error if OperationEventName list is empty
                        Modelist.Add(new AccountUploadDto(item.AccountNumber, item.AccountName, 0.ToString()));
                    }
                }
                string errorMessage = $" Acccount upload successful. Success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UploadAccountQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<AccountUploadDto>>.ReturnResultWith200(Modelist);
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
          
                string errorMessage = $"An error occured: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UploadAccountQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<AccountUploadDto>>.Return500(e);
            }
        }
    }
}
