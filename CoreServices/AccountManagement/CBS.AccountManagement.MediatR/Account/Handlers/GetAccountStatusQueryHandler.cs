using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{

    public class GetAccountStatusQueryHandler : IRequestHandler<GetAccountStatusQuery, ServiceResponse<AccountDetails>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountStatusQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IAccountCategoryRepository _accountCategoryRepository;
        public GetAccountStatusQueryHandler(UserInfoToken userInfoToken,
            IAccountRepository AccountRepository,
            IMapper mapper, ILogger<GetAccountStatusQueryHandler> logger, PathHelper? pathHelper, IAccountCategoryRepository? accountCategoryRepository)
        {
            // Assign provided dependencies to local variables.
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _pathHelper = pathHelper;
            _accountCategoryRepository = accountCategoryRepository;
        }



        public async Task<ServiceResponse<AccountDetails>> Handle(GetAccountStatusQuery request, CancellationToken cancellationToken)
        {
            AccountDetails listAccountData = new AccountDetails();
            try
            {
                var accountCartegorie = _accountCategoryRepository.All.ToList();    // Retrieve all AccountCategories entities from the repository&&
                var entities = new List<Data.Account>();        // Retrieve all Accounts entities from the repository&&

                if (_userInfoToken.IsHeadOffice)
                {
                    if (request.BranchId == null)
                    {
                        var entitiesG = (_AccountRepository.All.Where(x => x.IsDeleted.Equals(false) && x.AccountOwnerId == _userInfoToken.BranchId).GroupBy(x => x.AccountNumberReference)
                                        .ToList()).ToDictionary(   key => key.Key,      value => value.First()  );// or value.SingleOrDefault() if you expect only one account per reference 
                        var AccountDetails = (from account in entitiesG.Values
                                              select new AccountData
                                              {
                                                  Cartegory = account.AccountCategoryId,
                                                  AccountName = account.AccountName,
                                                  AccountNumber = account.AccountNumber,
                                                  CurrentBalance = account.CurrentBalance.ToString() // (BaseUtilities.ConvertToLong(account.CurrentBalance)).ToString(),
                                              }).ToList();
                        listAccountData.LedgerAccountDetails = (from acc in AccountDetails
                                                                join cat in accountCartegorie on acc.Cartegory equals cat.Id
                                                                select new AccountData
                                                                {
                                                                    Cartegory = cat.Name,
                                                                    AccountName = acc.AccountName,
                                                                    AccountNumber = acc.AccountNumber,

                                                                    CurrentBalance = acc.CurrentBalance,//(BaseUtilities.ConvertToLong(acc.CurrentBalance)).ToString()
                                                                }).OrderBy(x => x.AccountNumber).ToList();

                    }
                    else
                    {
                        entities = _AccountRepository.All.Where(x => x.IsDeleted.Equals(false) && x.AccountOwnerId == request.BranchId).ToList();
                        var AccountDetails = (from account in entities
                                              select new AccountData
                                              {
                                                  Cartegory = account.AccountCategoryId,
                                                  AccountName = account.AccountName,
                                                  AccountNumber = account.AccountNumberCU,//account.AccountNumber.PadRight(6, '0') + account.AccountNumberManagementPosition.PadRight(3, '0'),
                                                  CurrentBalance = (BaseUtilities.ConvertToLong(account.CurrentBalance)).ToString(),
                                              }).ToList();
                        listAccountData.LedgerAccountDetails = (from acc in AccountDetails
                                                                join cat in accountCartegorie on acc.Cartegory equals cat.Id
                                                                select new AccountData
                                                                {
                                                                    Cartegory = cat.Name,
                                                                    AccountName = acc.AccountName,
                                                                    AccountNumber = acc.AccountNumber,
                                                                    CurrentBalance = (BaseUtilities.ConvertToLong(acc.CurrentBalance)).ToString()
                                                                }).OrderBy(x => x.AccountNumber).ToList();

                    }

                }
                else
                {
                    entities = _AccountRepository.All.Where(x => x.IsDeleted.Equals(false) && x.AccountOwnerId == request.BranchId).ToList();
                    var AccountDetails = (from account in entities
                                          select new AccountData
                                          {
                                              Cartegory = account.AccountCategoryId,
                                              AccountName = account.AccountName,
                                              AccountNumber = account.AccountNumberCU,
                                              CurrentBalance = (BaseUtilities.ConvertToLong(account.CurrentBalance)).ToString(), // (BaseUtilities.ConvertToLong(account.CurrentBalance)).ToString(),
                                                                                                                                 //CurrentBalance = (BaseUtilities.ConvertToLong(account.CurrentBalance)).ToString(),
                                          }).ToList();
                    listAccountData.LedgerAccountDetails = (from acc in AccountDetails
                                                            join cat in accountCartegorie on acc.Cartegory equals cat.Id
                                                            select new AccountData
                                                            {
                                                                Cartegory = cat.Name,
                                                                AccountName = acc.AccountName,
                                                                AccountNumber = acc.AccountNumber,
                                                                CurrentBalance = (BaseUtilities.ConvertToLong(acc.CurrentBalance)).ToString(),
                                                            }).OrderBy(x => x.AccountNumber).ToList();

                }
                var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                var BranchInfo = Branches.Find(x => x.id == request.BranchId);
                listAccountData.EntityId = BranchInfo.id;
                listAccountData.EntityType = "BRANCH";
                listAccountData.FromDate = request.FromDate;
                listAccountData.ToDate = request.ToDate;
                listAccountData.BranchName = BranchInfo.name;
                listAccountData.BranchAddress = BranchInfo.address;
                listAccountData.Name = BranchInfo.bank.name;
                listAccountData.Location = BranchInfo.bank.address;
                listAccountData.Address = BranchInfo.address;
                listAccountData.Capital = BranchInfo.bank.capital;
                listAccountData.ImmatriculationNumber = BranchInfo.bank.immatriculationNumber;
                listAccountData.WebSite = BranchInfo.webSite;
                listAccountData.BranchTelephone = BranchInfo.telephone;
                listAccountData.HeadOfficeTelePhone = BranchInfo.bank.telephone;
                listAccountData.BranchCode = BranchInfo.branchCode;
                string errorMessage = $"Return AccountDto with a success response";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetAccountStatusQuery, LogLevelInfo.Information);
                //request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                //listAccountData = _mapper.Map(entities, listAccountData);
                return ServiceResponse<AccountDetails>.ReturnResultWith200(listAccountData);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Accounts: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
           
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetAccountStatusQuery, LogLevelInfo.Error);

                return ServiceResponse<AccountDetails>.Return500(e, "Failed to get all Accounts");
            }
        }

    }
}
