using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class BranchTransactionAcknowledgementCommandHandler : IRequestHandler<BranchTransactionAcknowledgementCommand, ServiceResponse<bool>>
    {
        private readonly IEntryTempDataRepository _entryTempDataRepository; // Repository for accessing AccountFeature data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<BranchTransactionAcknowledgementCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;
        private readonly IAccountRepository _accountRepository;

        public BranchTransactionAcknowledgementCommandHandler(
            IEntryTempDataRepository AccountFeatureRepository,
            IMapper mapper,
            ILogger<BranchTransactionAcknowledgementCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IMediator? mediator, PathHelper pathHelper, IAccountRepository? accountRepository)
        {
            _entryTempDataRepository = AccountFeatureRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _logger = logger;
            _mediator = mediator;
            _uow = uow;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<bool>> Handle(BranchTransactionAcknowledgementCommand collection, CancellationToken cancellationToken)
        {

            var branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
            DateTime dateTime = await GetAccountingDate(branches);// 
            string message = string.Empty;
            try
            {
                var modelAER = new GetAccountingRuleQuery
                { Id = collection.AccountingEventRuleId };
                var resultData = await _mediator.Send(modelAER);
                if (collection.IsDoubleValidationNeeded)
                {
                    foreach (var item in collection.EntryTempDatas)
                    {
                        var modelCmd = new AddEntryTempDataCommand
                        {
                            Id = item.Id,
                            AccountId = item.AccountId,
                            AccountBalance = item.AccountBalance,
                            AccountName = item.AccountName,
                            AccountNumber = item.AccountNumber,
                            Amount = item.Amount,
                            BookingDirection = item.BookingDirection,
                            Description = item.Description,
                            Reference = item.Reference,
                            AccountingEventId = collection.EntryTempDatas[0].AccountingEventId,
                            PositingSource = collection.IsSystem ? TypeOfEntry.TRUST_SYSTEM.ToString() : TypeOfEntry.USER.ToString(),
                            ExternalBranchId = item.ExternalBranchId,
                            BranchId = collection.BranchId

                        };
                        var result = await _mediator.Send(modelCmd);
                        if (!result.StatusCode.Equals(200))
                        {
                            var messageXX = ("An error occured while preparing for positing " + result.Message);
                            await BaseUtilities.LogAndAuditAsync(messageXX, collection, HttpStatusCodeEnum.OK, LogAction.AddTempDataEntriesCommand, LogLevelInfo.Error);
                        }
                    }
                    var modelPosting = new AddTempDataEntriesCommand
                    {
                        Reference = collection.EntryTempDatas[0].Reference,
                        Description = collection.EntryTempDatas[0].Description,
                        IsSyteme = collection.IsSystem,
                        AccountingEventId = collection.EntryTempDatas[0].AccountingEventId,
                        BranchId = collection.BranchId
                    };
                    var resultPosting = await _mediator.Send(modelPosting);
                    if (!resultPosting.StatusCode.Equals(200))
                    {
                        message = "An error occured while positing " + resultPosting.Message;
                        await BaseUtilities.LogAndAuditAsync(message, collection, HttpStatusCodeEnum.OK, LogAction.AddTempDataEntriesCommand, LogLevelInfo.Information);
                        return ServiceResponse<bool>.ReturnResultWith200(true);
                    }
                }
                else
                {
                    foreach (var item in collection.EntryTempDatas)
                    {
                        var modelCmd = new AddEntryTempDataCommand
                        {
                            Id = item.Id,
                            AccountId = item.AccountId,
                            AccountBalance = item.AccountBalance,
                            AccountName = item.AccountName,
                            AccountNumber = item.AccountNumber,
                            Amount = item.Amount,
                            BookingDirection = item.BookingDirection,
                            Description = item.Description,
                            Reference = item.Reference,
                            AccountingEventId = collection.EntryTempDatas[0].AccountingEventId,
                            PositingSource = collection.IsSystem ? TypeOfEntry.TRUST_SYSTEM.ToString() : TypeOfEntry.USER.ToString(),
                            ExternalBranchId = item.ExternalBranchId,
                            BranchId = collection.BranchId

                        };
                        var result = await _mediator.Send(modelCmd);
                        if (!result.StatusCode.Equals(200))
                        {
                            var messageXX = ("An error occured while preparing for positing " + result.Message);
                            await BaseUtilities.LogAndAuditAsync(messageXX, collection, HttpStatusCodeEnum.InternalServerError, LogAction.AddEntryTempDataCommand, LogLevelInfo.Error);
                        }
                    }
                    var modelPosting = new AddTempDataEntriesCommand
                    {
                        Reference = collection.EntryTempDatas[0].Reference,
                        Description = collection.EntryTempDatas[0].Description,
                        IsSyteme = collection.IsSystem,
                        AccountingEventId = collection.EntryTempDatas[0].AccountingEventId,
                        BranchId = collection.BranchId
                    };
                    var resultPosting = await _mediator.Send(modelPosting);
                    if (!resultPosting.StatusCode.Equals(200))
                    {
                        message = "An error occured while positing " + resultPosting.Message;
                        await BaseUtilities.LogAndAuditAsync(message, collection, HttpStatusCodeEnum.Conflict, LogAction.AddTempDataEntriesCommand, LogLevelInfo.Warning);
                        return ServiceResponse<bool>.Return409();
                    }
                    var confirmPostedEntriesCommand = new ConfirmPostedEntriesCommand
                    {
                        Id = modelPosting.Reference,
                        HasApproved = true,
                        TransactionDate = dateTime,
                        ValidationIsNotRequired = true,
                        BranchId = collection.BranchId
                    };
                    var resultconfirmPostedEntries = await _mediator.Send(confirmPostedEntriesCommand);
                    if (!resultconfirmPostedEntries.StatusCode.Equals(200))
                    {
                        message = "An error occured while positing " + resultconfirmPostedEntries.Message;
                        await BaseUtilities.LogAndAuditAsync(message, collection, HttpStatusCodeEnum.Conflict, LogAction.ConfirmPostedEntriesCommand, LogLevelInfo.Warning);
                        return ServiceResponse<bool>.Return403(true);
                    }
                }


                message = $"Accounting entry  with reference {collection.EntryTempDatas[0].Reference}  was successfully added.";
                await BaseUtilities.LogAndAuditAsync(message, collection, HttpStatusCodeEnum.OK, LogAction.AddTempDataEntriesCommand, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AccountFeature: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(message, collection, HttpStatusCodeEnum.InternalServerError, LogAction.AddTempDataEntriesCommand, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(e);
            }
        }

   
        private async Task<DateTime> GetAccountingDate(List<APICaller.Helper.LoginModel.Authenthication.Branch> branches)
        {
            string branchId = _userInfoToken.BranchId;
            if (_userInfoToken.BranchId.Equals("1"))
            {
                branchId = branches.Where(x => x.branchCode.Equals("001")).FirstOrDefault().id;
            }
            return (await APICallHelper.GetAccountingDateOpen(_pathHelper, _userInfoToken, branchId)).Data;
        }
    }
}

