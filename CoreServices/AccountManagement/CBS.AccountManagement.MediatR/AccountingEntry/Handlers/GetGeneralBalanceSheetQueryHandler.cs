using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class GetGeneralBalanceSheetQueryHandler : IRequestHandler<GetGeneralBalanceSheetQuery, ServiceResponse<BalanceSheetDto>>
    {
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetGeneralBalanceSheetQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IDocumentReferenceCodeRepository _documentReferenceCodeRepository;
        private readonly IDocumentTypeRepository _documentTypeRepository;
        private readonly IBSAccountRepository _bSAccountRepository;
        private readonly ITrialBalanceRepository _trialBalanceRepository;
        private readonly ICorrespondingMappingRepository _correspondingMappingRepository;
        private readonly ICorrespondingMappingExceptionRepository _correspondingMappingExceptionRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IUnitOfWork<POSContext> _unitOfWork;

        public GetGeneralBalanceSheetQueryHandler(

            IConfiguration configuration,
            IMapper mapper,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken,
             ILogger<GetGeneralBalanceSheetQueryHandler> logger, IDocumentTypeRepository documentRepository, ITrialBalanceRepository trialBalanceRepository, IDocumentReferenceCodeRepository documentReferenceCodeRepository, ICorrespondingMappingRepository? correspondingMappingRepository, ICorrespondingMappingExceptionRepository? correspondingMappingExceptionRepository, IBSAccountRepository? bSAccountRepository)
        {
            _documentTypeRepository = documentRepository;
            _mapper = mapper;
            _unitOfWork = uow;
            _logger = logger;
            _documentReferenceCodeRepository = documentReferenceCodeRepository;
            _trialBalanceRepository = trialBalanceRepository;
            _bSAccountRepository = bSAccountRepository;
            _userInfoToken = userInfoToken;
            _correspondingMappingRepository = correspondingMappingRepository;
            _correspondingMappingExceptionRepository = correspondingMappingExceptionRepository;
            _pathHelper = new PathHelper(configuration);

        }


        public async Task<ServiceResponse<BalanceSheetDto>> Handle(GetGeneralBalanceSheetQuery request, CancellationToken cancellationToken)
        {

            BalanceSheetDto balanceSheetDto = new BalanceSheetDto();
          
            var Branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
            var BranchInfo = Branches.Find(x => x.id == request.BranchId);
            balanceSheetDto.BranchCode = BranchInfo.branchCode;
            balanceSheetDto.BranchName = BranchInfo.name;
            balanceSheetDto.BranchAddress = BranchInfo.address;
            balanceSheetDto.Name = BranchInfo.bank.name;
            balanceSheetDto.Location = BranchInfo.bank.address;
            balanceSheetDto.Address = BranchInfo.bank.address;
            balanceSheetDto.Capital = BranchInfo.bank.capital;
            balanceSheetDto.ImmatriculationNumber = BranchInfo.bank.immatriculationNumber;
            balanceSheetDto.WebSite = BranchInfo.webSite;
            balanceSheetDto.BranchTelephone = BranchInfo.telephone;
            balanceSheetDto.HeadOfficeTelePhone = BranchInfo.bank.telephone;
            string errorMessage = "";
            try
            {
                var TrialBalance = _trialBalanceRepository.FindBy(x => x.BranchId == request.BranchId);
                var models = _documentTypeRepository.FindBy(x => x.DocumentId.Equals(request.DocumentId));
                var bSLIST = _bSAccountRepository.FindBy(x => x.BranchId == request.BranchId);
                if (bSLIST.Any())
                {
                    balanceSheetDto.TotalAsset = bSLIST.Where(x => x.Cartegory.ToLower().Equals(FinancialStatement.Asset.ToLower())).Sum(x => x.Amount);
                    balanceSheetDto.TotalLiabilityEquity = bSLIST.Where(x => x.Cartegory.ToLower().Equals(FinancialStatement.Liability.ToLower())).Sum(x => x.Amount);

                    balanceSheetDto.Accounts = bSLIST.ToList();
                    _bSAccountRepository.RemoveRange(bSLIST);
                    return ServiceResponse<BalanceSheetDto>.ReturnResultWith200(balanceSheetDto);
                }
                foreach (var item in models)
                {
                    var listOfReference = _documentReferenceCodeRepository.FindBy(x => x.DocumentTypeId == item.Id&&x.DocumentId== request.DocumentId).OrderBy(x => x.ReferenceCode).ToList();
                    if (!listOfReference.Any())
                    {
                        errorMessage = $"The report with Id :{request.DocumentId} has not yet been configured.";
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                        request, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                        return ServiceResponse<BalanceSheetDto>.ReturnException(new Exception(errorMessage));
                    }
                    listOfReference.RemoveAll(c => c.ReferenceCode.Equals(string.Empty));
                    foreach (var referenceCode in listOfReference)
                    {
                        try
                        {
                            var listOfReferenceAccount = _correspondingMappingRepository.FindBy(k => k.DocumentReferenceCodeId == referenceCode.Id);
                            if (!listOfReference.Any())
                            {
                                errorMessage = $"The reference code {referenceCode.ReferenceCode} has no corresponding mapping account.";
                                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                                request, errorMessage, LogLevelInfo.Information.ToString(), 422, _userInfoToken.Token);
                                return ServiceResponse<BalanceSheetDto>.ReturnException(new Exception(errorMessage));
                            }
                            var listOfGross = listOfReferenceAccount.Where(x => x.Cartegory.Equals(BalanceSheetCartegory.GROSS));
                            var listOfProvision = listOfReferenceAccount.Where(x => x.Cartegory.Equals(BalanceSheetCartegory.PROVISION));
                            var listOfGrossException = referenceCode.HasException ? _correspondingMappingExceptionRepository.FindBy(k => k.DocumentReferenceCodeId == referenceCode.Id && k.Category == BalanceSheetCartegory.GROSS) : null;
                            var listOfProvisionException = referenceCode.HasException ? _correspondingMappingExceptionRepository.FindBy(k => k.DocumentReferenceCodeId == referenceCode.Id && k.Category == BalanceSheetCartegory.PROVISION) : null;

                            BalanceSheetAccount BsAcc = GetBalanceSheetRecord(listOfGross, listOfGrossException, listOfProvision, listOfProvisionException, referenceCode, request.BranchId, item.Name);
                            balanceSheetDto.Accounts.Add(BsAcc);
                         
                        }
                        catch (Exception ex)
                        {

                            throw(ex);
                        }

                    }
                    _bSAccountRepository.AddRange(balanceSheetDto.Accounts);
                }
                balanceSheetDto.TotalAsset = balanceSheetDto.Accounts.Where(x => x.Cartegory.ToLower().Equals(FinancialStatement.Asset.ToLower())).Sum(x => x.Amount);
                balanceSheetDto.TotalLiabilityEquity = balanceSheetDto.Accounts.Where(x => x.Cartegory.ToLower().Equals(FinancialStatement.Liability.ToLower())).Sum(x => x.Amount);
               await _unitOfWork.SaveAsync();
                return ServiceResponse<BalanceSheetDto>.ReturnResultWith200(balanceSheetDto);
            }
            catch (Exception ex)
            {

                errorMessage = $"BalanceSheet data failed to retrieved successfully balance sheet .";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<BalanceSheetDto>.Return500(ex);
            }
        }

        private BalanceSheetAccount GetBalanceSheetRecord(IQueryable<CorrespondingMapping> listOfGross, IQueryable<CorrespondingMappingException>? listOfGrossException, IQueryable<CorrespondingMapping> listOfProvision, IQueryable<CorrespondingMappingException>? listOfProvisionException, DocumentReferenceCode referenceCode, string? branchId, string name)
        {

            double totalGross, totalGrossException, totalProvision, totalProvisionException, accountBalance = 0;
            totalGross = GetTrialBalanceValue(listOfGross, branchId);
            totalProvision = GetTrialBalanceValue(listOfProvision, branchId);
            totalGrossException = referenceCode.HasException ? GetTrialBalanceExceptionValue(listOfGrossException, branchId) : 0;
            totalProvisionException = referenceCode.HasException ? GetTrialBalanceExceptionValue(listOfProvisionException, branchId) : 0;

            accountBalance = (totalGross - totalGrossException) - (totalProvision - totalProvisionException);
            return new BalanceSheetAccount
            {
                Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "DTO"),
                Amount = accountBalance,
                Reference = referenceCode.ReferenceCode,
                Description = referenceCode.Description,
                Cartegory = name
            };
        }

        private double GetTrialBalanceExceptionValue(IQueryable<CorrespondingMappingException>? listOfGrossException, string? branchId)
        {
            try
            {
                double sumbalance = 0;
                var tblist = _trialBalanceRepository.FindBy(x => x.BranchId == branchId).ToList();
                foreach (var item in listOfGrossException)
                {

                    if (item.AccountNumber.Length == 2)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2));
                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }

                    }
                    else if (item.AccountNumber.Length == 3)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2) && x.Account3 == item.AccountNumber.Substring(0, 3));

                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }
                    }
                    else if (item.AccountNumber.Length == 4)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2) && x.Account3 == item.AccountNumber.Substring(0, 3) && x.Account4 == item.AccountNumber.Substring(0, 4));

                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }
                    }
                    else if (item.AccountNumber.Length == 5)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2) && x.Account3 == item.AccountNumber.Substring(0, 3) && x.Account4 == item.AccountNumber.Substring(0, 4) && x.Account5 == item.AccountNumber.Substring(0, 5));

                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }
                    }
                }
                return sumbalance;
            }
            catch (Exception ex)
            {

                throw(ex);
            }
        }

        private double GetTrialBalanceValue(IQueryable<CorrespondingMapping> listOfGross, string? branchId)
        {
            try
            {
                double sumbalance = 0;
                var tblist = _trialBalanceRepository.FindBy(x => x.BranchId == branchId).ToList();
                foreach (var item in listOfGross)
                {
                    if (item.AccountNumber.Length == 2)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2));
                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }

                    }
                    else if (item.AccountNumber.Length == 3)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2) && x.Account3 == item.AccountNumber.Substring(0, 3));

                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }
                    }
                    else if (item.AccountNumber.Length == 4)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2) && x.Account3 == item.AccountNumber.Substring(0, 3) && x.Account4 == item.AccountNumber.Substring(0, 4));

                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }
                    }
                    else if (item.AccountNumber.Length == 5)
                    {
                        var ans = tblist.Where(x => x.Account1 == item.AccountNumber.Substring(0, 1) && x.Account2 == item.AccountNumber.Substring(0, 2) && x.Account3 == item.AccountNumber.Substring(0, 3) && x.Account4 == item.AccountNumber.Substring(0, 4) && x.Account5 == item.AccountNumber.Substring(0, 5));

                        if (ans.Any())
                        {
                            sumbalance = sumbalance + (ans.Sum(x => x.EndingCreditBalance) - ans.Sum(x => x.EndingDebitBalance));

                        }
                    }
                }
                return sumbalance;
            }
            catch (Exception ex)
            {

                throw (ex);
            }


      
        }
    }
}
