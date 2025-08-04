using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using MediatR;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class ConfirmPostedEntriesCommandHandler : IRequestHandler<ConfirmPostedEntriesCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly IMapper _mapper;
        private readonly ILogger<ConfirmPostedEntriesCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken; //
        private readonly PathHelper _pathHelper;
        private readonly IEntryTempDataRepository _entryTempDataRepository;
        private readonly IPostedEntryRepository _postedEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountingEntryRepository _accountEntryRepository;
        public ConfirmPostedEntriesCommandHandler(
           IEntryTempDataRepository entryTempDataRepository,
            IAccountingEntriesServices accountingEntriesServices,
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper,
             IPostedEntryRepository postedEntryRepository,
            IAccountRepository accountRepository,
            ILogger<ConfirmPostedEntriesCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, PathHelper pathHelper)
        {
            _accountingEntriesServices = accountingEntriesServices;
            _mapper = mapper;
            _logger = logger;
            _pathHelper = pathHelper;
            _postedEntryRepository = postedEntryRepository;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _entryTempDataRepository = entryTempDataRepository;
            _accountRepository = accountRepository;
            _accountEntryRepository = accountingEntryRepository;
        }


        public async Task<ServiceResponse<bool>> Handle(ConfirmPostedEntriesCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = $"Transaction Entry Passed Successfully";
            try
            {
                List<Data.Account> accounts= new List<Data.Account>();
                List<Data.AccountingEntry> accountingEntries = new List<Data.AccountingEntry>();
                var entries = _entryTempDataRepository.FindBy(c => c.Reference == request.Id).ToList();
                PostedEntry postedEntry = await _postedEntryRepository.FindAsync(request.Id);
                if (postedEntry != null)
                {
                    if (_userInfoToken.Id != entries[0].CreatedBy||request.ValidationIsNotRequired.Value)
                    {
                        if (EntryTempData.EvaluateEntry(entries))
                        {
                            if (request.HasApproved == false)
                            {
                                postedEntry.status = PostingStatus.Rejected.ToString();
                                postedEntry.HasValidated = true;
                                postedEntry.ApprovedBy = _userInfoToken.Id;
                                postedEntry.ApprovedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                                postedEntry.EntryDetail = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(entries));

                            }
                            else
                            {
                                postedEntry.Amount = entries.Where(x => x.BookingDirection.ToLower() == "debit").Sum(x => x.Amount);
                          
                                #region MyRegion
                                foreach (var item in entries)
                                {
                              
                                    var account = _accountRepository.Find(item.AccountId);
                                    account= await   _accountingEntriesServices.UpdateAccountAsync(account, item.Amount, GetoperationType(item.BookingDirection) );
                                    accounts.Add(account);
                                   // item.Description = GenerateManualEntryDescription(item, entries[0].AccountingEventId== "MANUAL USER");
                                    Data.AccountingEntry accountEntry = _accountingEntriesServices.CreateEntry(account, item, _userInfoToken.Id,request.TransactionDate.Value);
                                    accountingEntries.Add(accountEntry);
                                }
                                postedEntry.status = PostingStatus.Approved.ToString();
                                postedEntry.HasValidated = true;
                                postedEntry.ApprovedBy = _userInfoToken.Id;
                                postedEntry.ApprovedDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                                postedEntry.EntryDetail = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(entries));
                                _accountRepository.UpdateRangeInCaseCade(accounts);
                                _accountEntryRepository.AddRange(accountingEntries);
                                _entryTempDataRepository.RemoveRangeAsync(entries);
                                #endregion
                            }

                            _postedEntryRepository.UpdateInCasecade(postedEntry);
                            await _uow.SaveAsyncWithOutAffectingBranchId();

                            _logger.LogInformation(errorMessage);
                            await APICallHelper.AuditLogger(_userInfoToken.Email, "ConfirmPostedEntriesCommand",
                            request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                            return ServiceResponse<bool>.ReturnResultWith200(true);
                        }
                        else
                        {
                            errorMessage = $"The double entry rule has not been respected";

                            _logger.LogError(errorMessage);
                            await APICallHelper.AuditLogger(_userInfoToken.Email, "ConfirmPostedEntriesCommand",
                            request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                            return ServiceResponse<bool>.Return409(errorMessage);
                        }
                    }
                    else
                    {
                        errorMessage = $"You cannot approved a journal entry you initiated please kindly contact your manager for approval";

                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "ConfirmPostedEntriesCommand",
                        request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                        return ServiceResponse<bool>.Return409(errorMessage);
                    }
                    
                }
                else
                {
                    errorMessage = $"Posted entries not found";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "ConfirmPostedEntriesCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }

            }
            catch ( Exception  ex)
            {

                throw;
            }
        }

        private string? GenerateManualEntryDescription(EntryTempData item, bool IsDoubbleValidationRequired)
        {
            string message = string.Empty;
            message = $"[{item.PositingSource}] entries passed according to {item.AccountingEventId} posted by {item.FullName} with ";
            if (IsDoubbleValidationRequired)
            {
                message = message + $"double validation is required";
            }
           else
            {
                message = message + $"double validation is not required";
            }    
               message = message + $" | Amount: XAF{item.Amount}  | Reference: {item.Reference} | Date: {item.CreatedDate}";
          return message ;
        }

        private AccountOperationType GetoperationType(string bookingDirection)
        {
            if (bookingDirection.ToLower()=="debit")
            {
                return AccountOperationType.DEBIT;
            }
            else
            {
                return AccountOperationType.CREDIT;
            }
        }
    }
}
