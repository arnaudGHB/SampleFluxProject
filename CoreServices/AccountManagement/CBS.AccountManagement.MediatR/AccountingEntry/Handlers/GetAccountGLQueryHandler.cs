using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
 
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
 
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System.Net;

namespace CBS.AccountingEntryManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific AccountingEntry based on its unique identifier.
    /// </summary>
    public class GetAccountGLQueryHandler : IRequestHandler<GetAccountGLQuery, ServiceResponse<AccountingGeneralLedger>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing AccountingEntry data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountGLQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly PathHelper _pathHelper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _accountRepository;
        /// <summary>
        /// Constructor for initializing the GetAccountingEntryQueryHandler
        /// </summary>
        /// <param name="AccountingEntryRepository">Repository for AccountingEntry data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountGLQueryHandler(
            IAccountingEntryRepository AccountingEntryRepository,
            IMapper mapper,
            ILogger<GetAccountGLQueryHandler> logger,
            PathHelper? pathHelper,
            UserInfoToken? userInfoToken,
            IAccountRepository? accountRepository)
        {
            _accountRepository = accountRepository;
            _accountingEntryRepository = AccountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
            _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountingEntryQuery to retrieve a specific AccountingEntry.
        /// </summary>
        /// <param name="request">The GetAccountingEntryQuery containing AccountingEntry ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        //public async Task<ServiceResponse<AccountingGeneralLedger>> Handle(GetAccountGLQuery request, CancellationToken cancellationToken)
        //{
        //    AccountingGeneralLedger accountingGeneralLedger= new AccountingGeneralLedger();
        //    string errorMessage = null;
        //    try
        //    {

        //        // Retrieve the AccountingEntry entity with the specified ID from the repository
        //        var entity = await  _accountingEntryRepository.All.Where(x=>(x.DrAccountId==request.AccountId||x.CrAccountId==request.AccountId)&& x.IsDeleted == false&&x.ValueDate.Date >= request.FromDate.Date && x.ValueDate.Date <= request.ToDate.Date).ToListAsync();
        //        if (entity.Count() != 0)
        //        {
        //            var account = await _accountRepository.FindAsync(request.AccountId);

        //                // Map the AccountingEntry entity to AccountingEntryDto and return it with a success response
        //                var AccountingEntryDto = _mapper.Map<List<AccountingEntryDto>>(entity);
        //            accountingGeneralLedger.AccountingEntries = AccountingEntryDto.ToList();

        //            var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
        //            var BranchInfo = Branches.Find(x => x.id == request.BranchId);
        //            accountingGeneralLedger.EntityId = BranchInfo.id;
        //            accountingGeneralLedger.EntityType = "BRANCH";
        //            accountingGeneralLedger.FromDate = request.FromDate;
        //            accountingGeneralLedger.ToDate = request.ToDate;
        //            accountingGeneralLedger.BranchName = BranchInfo.name;
        //            accountingGeneralLedger.BranchAddress = BranchInfo.address;
        //            accountingGeneralLedger.Name = BranchInfo.bank.name;
        //            accountingGeneralLedger.Location = BranchInfo.bank.address;
        //            accountingGeneralLedger.Address = BranchInfo.bank.address;
        //            accountingGeneralLedger.Capital = BranchInfo.bank.capital;
        //            accountingGeneralLedger.ImmatriculationNumber = BranchInfo.bank.immatriculationNumber;
        //            accountingGeneralLedger.WebSite = BranchInfo.webSite;
        //            accountingGeneralLedger.BranchTelephone = BranchInfo.telephone;
        //            accountingGeneralLedger.HeadOfficeTelePhone = BranchInfo.bank.telephone;
        //            accountingGeneralLedger.BranchCode = BranchInfo.branchCode;
        //            accountingGeneralLedger.MainAccountNumber = account.TempData;
        //            return ServiceResponse<AccountingGeneralLedger>.ReturnResultWith200(accountingGeneralLedger);


        //        }
        //        else
        //        {
        //            // If the AccountingEntry entity was not found, log the error and return a 404 Not Found response
        //            _logger.LogError("AccountingEntry not found.");
        //            return ServiceResponse<AccountingGeneralLedger>.Return404();
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        errorMessage = $"Error occurred while getting AccountingEntry: {e.Message}";

        //        // Log the error and return a 500 Internal Server Error response with the error message
        //        _logger.LogError(errorMessage);
        //        return ServiceResponse<AccountingGeneralLedger>.Return500(e);
        //    }
        //}

        public async Task<ServiceResponse<AccountingGeneralLedger>> Handle(GetAccountGLQuery request, CancellationToken cancellationToken)
        {
            AccountingGeneralLedger accountingGeneralLedger = new();
            string errorMessage = null;

            try
            {
                // Fetch accounting entries matching the given criteria
                var accountingEntries = await _accountingEntryRepository.All
                    .Where(x => (x.DrAccountId == request.AccountId || x.CrAccountId == request.AccountId)
                                && !x.IsDeleted
                                && x.ValueDate.Date >= request.FromDate.Date
                                && x.ValueDate.Date <= request.ToDate.Date)
                    .ToListAsync(cancellationToken);

                if (!accountingEntries.Any())
                {
                    errorMessage = $"No accounting entries found for AccountId: {request.AccountId}.";
                    _logger.LogWarning(errorMessage);

                    // Log and Audit
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.GetAccountGLQuery, LogLevelInfo.Warning);

                    return ServiceResponse<AccountingGeneralLedger>.Return404();
                }

                // Fetch account details
                var account = await _accountRepository.FindAsync(request.AccountId);
                if (account == null)
                {
                    errorMessage = $"Account not found for ID: {request.AccountId}.";
                    _logger.LogWarning(errorMessage);

                    // Log and Audit
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.GetAccountGLQuery, LogLevelInfo.Warning);

                    return ServiceResponse<AccountingGeneralLedger>.Return404();
                }

                // Fetch branch details
                var branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                var branchInfo = branches.FirstOrDefault(x => x.id == request.BranchId);

                if (branchInfo == null)
                {
                    errorMessage = $"Branch not found for ID: {request.BranchId}.";
                    _logger.LogWarning(errorMessage);

                    // Log and Audit
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.GetAccountGLQuery, LogLevelInfo.Warning);

                    return ServiceResponse<AccountingGeneralLedger>.Return404();
                }

                // Map accounting entries
                accountingGeneralLedger.AccountingEntries = _mapper.Map<List<AccountingEntryDto>>(accountingEntries);
                accountingGeneralLedger.EntityId = branchInfo.id;
                accountingGeneralLedger.EntityType = "BRANCH";
                accountingGeneralLedger.FromDate = request.FromDate;
                accountingGeneralLedger.ToDate = request.ToDate;
                accountingGeneralLedger.BranchName = branchInfo.name;
                accountingGeneralLedger.BranchAddress = branchInfo.address;
                accountingGeneralLedger.Name = branchInfo.bank.name;
                accountingGeneralLedger.Location = branchInfo.bank.address;
                accountingGeneralLedger.Address = branchInfo.bank.address;
                accountingGeneralLedger.Capital = branchInfo.bank.capital;
                accountingGeneralLedger.ImmatriculationNumber = branchInfo.bank.immatriculationNumber;
                accountingGeneralLedger.WebSite = branchInfo.webSite;
                accountingGeneralLedger.BranchTelephone = branchInfo.telephone;
                accountingGeneralLedger.HeadOfficeTelePhone = branchInfo.bank.telephone;
                accountingGeneralLedger.BranchCode = branchInfo.branchCode;
                accountingGeneralLedger.MainAccountNumber = account.TempData;

                errorMessage = $"Successfully retrieved general ledger for AccountId: {request.AccountId}.";
                _logger.LogInformation(errorMessage);

                // Log and Audit success event
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetAccountGLQuery, LogLevelInfo.Information);

                return ServiceResponse<AccountingGeneralLedger>.ReturnResultWith200(accountingGeneralLedger);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error occurred while retrieving general ledger for AccountId: {request.AccountId}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage);

                // Log and Audit error event
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetAccountGLQuery, LogLevelInfo.Error);

                return ServiceResponse<AccountingGeneralLedger>.Return500(ex);
            }
        }


    }
}