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
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Domain;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace CBS.AccountManagement.MediatR 
{

    public class UploadChartOfAccountQueryHandler : IRequestHandler<UploadChartOfAccountQuery, ServiceResponse<List<ChartOfAccountDto>>>
    {
        // Dependencies
        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IAccountCategoryRepository _AccountCategoryRepository;
        private readonly IAccountClassRepository _AccountClassRepository;
        private readonly ILogger<UploadChartOfAccountQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<POSContext> _uowOfWork;
        // Constructor to inject dependencies
        public UploadChartOfAccountQueryHandler(IChartOfAccountRepository ChartOfAccountRepository,
            IAccountCategoryRepository AccountCategoryRepository, IAccountClassRepository AccountClassRepository,
            ILogger<UploadChartOfAccountQueryHandler> logger,
           UserInfoToken userInfoToken,
            IMapper mapper, IUnitOfWork<POSContext> uowOfWork)
        {
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _AccountCategoryRepository = AccountCategoryRepository;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _mapper = mapper;
            _AccountClassRepository = AccountClassRepository;
            _uowOfWork = uowOfWork;
        }

     
        public async Task<ServiceResponse<List<ChartOfAccountDto>>> Handle(UploadChartOfAccountQuery request, CancellationToken cancellationToken)
        {
            List<Data.ChartOfAccount> listChartOfAccount = new  List<Data. ChartOfAccount>();
             
            try
            {
                // Check if there are existing AccountCategorys in the repository
                var entityExists = _ChartOfAccountRepository.All.ToList();

                if (entityExists.Count() == 0)
                {
                    // Map DTOs to entity models
                    var listAccount  =_mapper.Map<List<Data.ChartOfAccount>>(request.ChartOfAccounts);

                    foreach (var item in listAccount)
                    {
                        string root = item.AccountNumber.Substring(0,1);

                        var AccountClasses = _AccountClassRepository.FindBy(c => c.AccountNumber.Equals(root));
                        if (AccountClasses.Any()) 
                        {
                            var AccountClass = AccountClasses.FirstOrDefault();
                            item.AccountCartegoryId = root.Equals("0") ? "xxxxx" : AccountClass.AccountCategoryId;
                            if (item.AccountCartegoryId==null)
                            {

                            }
                            item.MigrationDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                        }
                       
                        listChartOfAccount.Add(Data.ChartOfAccount.UpdateAccountEntity(item));
                    }
                    _ChartOfAccountRepository.AddRange(listChartOfAccount);
                   await  _uowOfWork.SaveAsync();
                    // Map the added AccountCategories back to DTOs
                    var ChartOfAccount = _mapper.Map<List<ChartOfAccountDto>>(request.ChartOfAccounts);

                    // Return a successful response with the added AccountCategories
                    return ServiceResponse<List<ChartOfAccountDto>>.ReturnResultWith200(request.ChartOfAccounts);
                }
                else
                {
                    if (request.LanguageConvert)
                    {
                        UpdateExistingChartSetEnglishVersion(entityExists, request.ChartOfAccounts);
                        await _uowOfWork.SaveAsync();
                        // Return a successful response with the added AccountCategories
                        return ServiceResponse<List<ChartOfAccountDto>>.ReturnResultWith200(request.ChartOfAccounts);

                    }
                    else
                    {
                        //if (true) 
                        //{
                            var listAccount = _mapper.Map<List<Data.ChartOfAccount>>(request.ChartOfAccounts);

                            foreach (var item in listAccount)
                            {
                                string root = item.AccountNumber.Substring(0, 1);

                                var AccountClasses = _AccountClassRepository.FindBy(c => c.AccountNumber.Equals(root));
                                if (AccountClasses.Any())
                                {
                                    var AccountClass = AccountClasses.FirstOrDefault();
                                    item.AccountCartegoryId = root.Equals("0") ? "xxxxx" : AccountClass.AccountCategoryId;
                                    item.MigrationDate = BaseUtilities.UtcToDoualaTime(DateTime.Now);
                                }
                    
                            var modelAcc = Data.ChartOfAccount.UpdateAccountEntity(item);
                            modelAcc.ParentAccountId = item.ParentAccountId==null?_ChartOfAccountRepository.FindBy(x=>x.AccountNumber.Equals(item.AccountNumber)).FirstOrDefault().Id:"xxxxxxx";
                                listChartOfAccount.Add(modelAcc);
                            }
                            _ChartOfAccountRepository.AddRange(listChartOfAccount);
                            await _uowOfWork.SaveAsync();
                            // Map the added AccountCategories back to DTOs
                            var ChartOfAccount = _mapper.Map<List<ChartOfAccountDto>>(request.ChartOfAccounts);

                            // Return a successful response with the added AccountCategories
                            return ServiceResponse<List<ChartOfAccountDto>>.ReturnResultWith200(request.ChartOfAccounts);
                        //}
                        //var errorMessage = $"Error occurred while reading the ChartOfAccount already createds";
                        //_logger.LogError(errorMessage);

                        //// Return a server error response
                        //return ServiceResponse<List<ChartOfAccountDto>>.Return500(errorMessage);
                    }

                }
            }
            catch (Exception e)
            {
                // Log an error if an exception occurs during processing
                var errorMessage = $"Error occurred while reading the ChartOfAccountDto configurations: {e.Message}";
                _logger.LogError(errorMessage);

                // Return a server error response
                return ServiceResponse<List<ChartOfAccountDto>>.Return500(errorMessage);
            }
        }

        private void UpdateExistingChartSetEnglishVersion(List<Data.ChartOfAccount> entityExists, List<ChartOfAccountDto> chartOfAccounts)
        {
            foreach (var item in  chartOfAccounts)
            {
                var model = entityExists.Where(x => x.AccountNumber == item.AccountNumber.Trim()).FirstOrDefault();
                if (model != null)
                {
                   model.LabelEn= item.LabelEn.Trim();
                    _ChartOfAccountRepository.Update(model);
                }

            }
        }


     
    }
}
