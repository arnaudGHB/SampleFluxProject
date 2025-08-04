using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
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
    public class AddTempDataEntriesCommandHandler : IRequestHandler<AddTempDataEntriesCommand, ServiceResponse<bool>>
    {
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly IMapper _mapper;
        private readonly ILogger<AddTempDataEntriesCommandHandler> _logger;
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IEntryTempDataRepository _entryTempDataRepository;
        private readonly IPostedEntryRepository _postedEntryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountingEntryRepository _accountEntryRepository;
        public AddTempDataEntriesCommandHandler(
           IEntryTempDataRepository entryTempDataRepository,
            IAccountingEntriesServices accountingEntriesServices,
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper,
             IPostedEntryRepository postedEntryRepository,
            IAccountRepository accountRepository,
            ILogger<AddTempDataEntriesCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _accountingEntriesServices = accountingEntriesServices;
            _mapper = mapper;
            _logger = logger;
            _postedEntryRepository = postedEntryRepository;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _entryTempDataRepository = entryTempDataRepository;
            _accountRepository = accountRepository;
            _accountEntryRepository = accountingEntryRepository;
        }


        public async Task<ServiceResponse<bool>> Handle(AddTempDataEntriesCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = $"Transaction Entry Passed Successfully";
            try
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };

                List<EntryTempData> entryTempDatas = new List<EntryTempData>();
                 
                List<Data.Account> accounts= new List<Data.Account>();
                List<Data.AccountingEntry> accountingEntries = new List<Data.AccountingEntry>();
                var entries = _entryTempDataRepository.FindBy(c => c.Reference == request.Reference && c.CreatedBy == _userInfoToken.Id && c.BranchId == request.BranchId).ToList();
                if (EntryTempData.EvaluateEntry(entries))
                {
                    PostedEntry postedEntry= new PostedEntry();
                    postedEntry.Amount= entries.Where(x=>x.BookingDirection.ToLower()=="debit").Sum(x => x.Amount);
                    postedEntry.status = "Pending";
                    postedEntry.Description = request.Description;
                    postedEntry.PostingSource= request.IsSyteme.Value? TypeOfEntry.TRUST_SYSTEM.ToString():TypeOfEntry.USER.ToString();
                    postedEntry.HasValidated = true;
                    postedEntry.ApprovedBy = "NOT-SET";
                    postedEntry.ApprovedDate = new DateTime();
                    postedEntry.Id= request.Reference;
                    postedEntry.BranchId = request.BranchId;
                    postedEntry.EntryDetail = JsonConvert.DeserializeObject<JToken>(JsonConvert.SerializeObject(entries), settings);
                    #region MyRegion
                    foreach (var item in entries)
                    {
                        item.Status = "Review";
                        entryTempDatas.Add(item);
                    }
                 
                    #endregion
                    _postedEntryRepository.Add(postedEntry);
                    _entryTempDataRepository.UpdateRangeInCaseCade(entries);
                   await _uow.SaveAsyncWithOutAffectingBranchId();

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTempDataEntriesCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return ServiceResponse<bool>.ReturnResultWith200(true,errorMessage);
                }
                else
                {
                      errorMessage = $"The double entry rule has not been respected";

                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddTempDataEntriesCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);

                    return ServiceResponse<bool>.Return409(errorMessage);
                }
                
            }
            catch ( Exception  ex)
            {

                throw;
            }
        }
    }
}
