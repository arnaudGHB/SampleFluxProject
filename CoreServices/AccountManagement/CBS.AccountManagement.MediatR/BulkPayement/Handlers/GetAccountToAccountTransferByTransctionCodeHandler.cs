using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.BulkTransaction;
using CBS.AccountManagement.Data.Dto;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;

using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Collections.Generic;
 
namespace CBS.BudgetItemDetailManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BudgetItemDetail based on its unique identifier.
    /// </summary>
    public class GetAccountToAccountTransferByTransctionCodeHandler : IRequestHandler<GetAccountToAccountTransferByTransctionCode, ServiceResponse<bool>>
    {
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountToAccountTransferByTransctionCodeHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingEntriesServices _accountingEntriesServices;
        private readonly IMediator _mediator;// AutoMapper for object mapping.
        private readonly IAccountingEntryRepository _accountingEntryRepository;
        private readonly IMongoGenericRepository<BulkTransferResultCommand> _bulkTransferResultCommandRepository;
        private readonly IUnitOfWork<POSContext> _uow;

 


        /// <summary>
        /// Constructor for initializing the GetAccountToAccountTransferByTransctionCodeHandler.
        /// </summary>
        /// <param name="BudgetItemDetailRepository">Repository for AccountToAccountTransfer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountToAccountTransferByTransctionCodeHandler(
            IMongoUnitOfWork mongoUnitOfWork,
            IMapper mapper, IAccountingEntryRepository accountingEntryRepository,
            ILogger<GetAccountToAccountTransferByTransctionCodeHandler> logger,UserInfoToken userInfoToken, IAccountingEntriesServices? accountingEntriesServices, IMediator? mediator, IUnitOfWork<POSContext>? uow)
        {
            _mongoUnitOfWork = mongoUnitOfWork;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _accountingEntriesServices = accountingEntriesServices;
            _accountingEntryRepository = accountingEntryRepository;
            _bulkTransferResultCommandRepository = _mongoUnitOfWork.GetRepository<BulkTransferResultCommand>();
        }

  
        public async Task<ServiceResponse<bool>> Handle(GetAccountToAccountTransferByTransctionCode request, CancellationToken cancellationToken)
        {
            List<AccountingEntry> allEntries = new();
            List<AccountManagement.Data.BulkTransaction.MakeAccountPostingCommand> bulkResults = new();
            string errorMessage = null;

            try
            {
                bool hasFailed = false;

                var recordRepository = _mongoUnitOfWork.GetRepository<AccountToAccountTransfer>();
 

                // Retrieve all records that are in "Error" or "Pending" status
                var entities = (await recordRepository.GetAllAsync())
                    .Where(x => x.Status.Equals(BulkExecutionStatus.Error.ToString(), StringComparison.OrdinalIgnoreCase)
                             || x.Status.Equals(BulkExecutionStatus.Pending.ToString(), StringComparison.OrdinalIgnoreCase));

                var selectedItems = entities.Where(x => x.BulkExecutionCode == request.BulkExecutionCode).ToList();

                if (!selectedItems.Any())
                {
                    errorMessage = $"No entries found with BulkExecutionCode: {request.BulkExecutionCode}";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Warning);

                    return ServiceResponse<bool>.Return404();
                }
                // Check for duplicate transactions
                if (await _accountingEntriesServices.TransactionExists(selectedItems[0].TransactionReferenceId))
                {
                    errorMessage = $"An entry has already been posted with this transaction Ref:{selectedItems[0].TransactionReferenceId}.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                foreach (var item in selectedItems)
                {
                    var model = CreateBulkTransferResultCommand(item);
                    var result = await _mediator.Send(model);

                    if (result.StatusCode == 200)
                    {
                        if (!result.Data.MakeAccountPostingCommands.Any())
                        {
                            model.Status = ProcessingStatus.Completed.ToString();
                            item.Status = model.Status;
                            item.EndTime = BaseUtilities.UtcNowToDoualaTime();
                            allEntries.AddRange(result.Data.AccountingEntries);
                            await BaseUtilities.LogAndAuditAsync(item.ErrorMessage, item.MakeAccountPostingCommands,
                              HttpStatusCodeEnum.OK, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Information);
                        }
                        else
                        {
                            hasFailed = true;
                            model.Status = ProcessingStatus.Error.ToString();
                            item.Status = model.Status;
                            item.ErrorMessage = $"Processing failed.  Error: {result.Message}";
                            model.ErrorMessage= $"Processing failed.  Error: {result.Message}";
                            item.EndTime = BaseUtilities.UtcNowToDoualaTime();
                            model.MakeAccountPostingCommands.AddRange(item.MakeAccountPostingCommands);

                            await BaseUtilities.LogAndAuditAsync(item.ErrorMessage, item.MakeAccountPostingCommands,
                                HttpStatusCodeEnum.InternalServerError, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Warning);
                        }
                    }
                    else
                    {
                        hasFailed = true;
                        model.Status = ProcessingStatus.Error.ToString();
                        item.EndTime = BaseUtilities.UtcNowToDoualaTime();
                        item.Status = model.Status;
                        item.ErrorMessage = $"Processing failed. Error: {result.Message}";
                        model.MakeAccountPostingCommands.AddRange(item.MakeAccountPostingCommands);
                        await BaseUtilities.LogAndAuditAsync(item.ErrorMessage, item.MakeAccountPostingCommands,
                              HttpStatusCodeEnum.InternalServerError, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Warning);
                    }

                    // Save results
                    await UpsertBulkTransferResultCommand( model);
                    await recordRepository.UpdateAsync(item.Id, item);
                }

                // Persist accounting entries if any
                if (_accountingEntriesServices.EvaluateDoubleEntryRule( allEntries))
                {
                    _accountingEntryRepository.AddRange(allEntries);
                    await _uow.SaveAsyncWithOutAffectingBranchId();
                    errorMessage = "All the entries have been executed successfully";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Information);

                }
                else
                {

                }


                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error processing AccountToAccountTransfer: {BaseUtilities.GetInnerExceptionMessages(ex)}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetAccountToAccountTransferByTransctionCode, LogLevelInfo.Error);

                return ServiceResponse<bool>.Return500(ex);
            }
        }
        private BulkTransferResultCommand CreateBulkTransferResultCommand(AccountToAccountTransfer item)
        {
            return new BulkTransferResultCommand
            {
                Id = item.Id,
                MakeAccountPostingCommands = item.MakeAccountPostingCommands ?? new(),
                MemberName = item.MemberName,
                MemberReference = item.MemberReference,
                TransactionType = item.TransactionType,
                BranchId = item.BranchId,
                BranchName = item.BranchName,
                Status = item.Status,
                ErrorMessage = string.Empty,
                StartTime = item.StartTime,
                EndTime = DateTime.UtcNow.AddMinutes(30),
                ExternalBranchName = item.ExternalBranchName,
                Narration = item.Narration,
                ExternalBranchcode = item.ExternalBranchcode,
                TransactionReferenceId = item.TransactionReferenceId,
                BulkExecutionCode = item.BulkExecutionCode,
                TotalAmount = item.TotalAmount,
                Charges = item.Charges,
                UserFullName = item.UserFullName,
                UserInfoToken = item.UserInfoToken,
                BranchCode = item.BranchCode
            };
        }
        private async Task UpsertBulkTransferResultCommand( BulkTransferResultCommand model)
        {
            var existingRecord = await _bulkTransferResultCommandRepository.GetByIdAsync(model.Id);
            if (existingRecord == null)
            {
                await _bulkTransferResultCommandRepository.InsertAsync(model);
            }
            else
            {
                await _bulkTransferResultCommandRepository.UpdateAsync(model.Id, model);
            }
        }



    }
}