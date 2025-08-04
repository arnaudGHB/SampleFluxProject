using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
 
    public class GetAllAccountForASpecificProductTypeQueryHandler : IRequestHandler<GetAllAccountForASpecificProductTypeQuery,ServiceResponse<List<ProductAccountingChart>>>
    {
        private readonly IProductAccountingBookRepository _ProductAccountingBookRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetAllAccountForASpecificProductTypeQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
        private readonly IMediator _mediator;
        public GetAllAccountForASpecificProductTypeQueryHandler(
            IProductAccountingBookRepository AccountTypeRepository,
            IMapper mapper,
            ILogger<GetAllAccountForASpecificProductTypeQueryHandler> logger,UserInfoToken userInfoToken, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository, IMediator? mediator)
        {
            _ProductAccountingBookRepository = AccountTypeRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _mapper = mapper;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

  
        public async Task< ServiceResponse<List<ProductAccountingChart>>> Handle(GetAllAccountForASpecificProductTypeQuery request, CancellationToken cancellationToken)
        {
            ProductAccountingChart account = new ProductAccountingChart();
            List < ProductAccountingChart > productList = new List<ProductAccountingChart> ();
            string errorMessage = null;
            List<ProductAccountingChart> productListFinal = new List<ProductAccountingChart>();
            try
            {

                var entities = GetExistingAccountType(request);
                if (entities.Count()>0)
                {
                

                    foreach (var item in entities)
                    {
                         account = new ProductAccountingChart();
                        var model = productList.Find(Id => Id.ProductId == item.ProductAccountingBookId);
                        if (model==null)
                        {
                            account.ProductId = item.ProductAccountingBookId;
                            account.ProductName = item.Name;
                            account.AccountingChart = new List<AccountProduct>();
                        }
                        else
                        {
                            continue;
                        }
                     
                        productList.Add(account);   

                     }

                    foreach (var item in productList)
                    {
                        var result = await _mediator.Send(new GetAllAccountForProductQuery { ProductName = item.ProductName });
                        item.AccountingChart.AddRange(result.Data);
                        productListFinal.Add(item);
                    }

                    // Map the AccountType entity to AccountTypeDto and return it with a success response

                    string successMessage = $"Gotten all accounts in used for product  {request.ProductType} successfully.";
                        LogAuditSuccess( request, successMessage);
                        
                        return ServiceResponse < List <ProductAccountingChart>> .ReturnResultWith200(productListFinal, successMessage);
                        
                     
                }
                else
                {
                    // If the AccountType entity was not found, log the error and return a 404 Not Found response
                    errorMessage= "ProductAccountingBookDto not found.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  404);
                    return ServiceResponse<List<ProductAccountingChart>>.Return404(errorMessage);
                    
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  500);
                _logger.LogError(errorMessage);
                return ServiceResponse<List<ProductAccountingChart>>.Return404(errorMessage);
 
            }
        }
        private  List<ProductAccountingBook> GetExistingAccountType(GetAllAccountForASpecificProductTypeQuery request)
        {
            return  (_ProductAccountingBookRepository.FindBy(x => x.ProductType == request.ProductType)).ToList  ()?? new List<ProductAccountingBook>();
               
        }

        private List<ProductAccountingBook> GetExistingProductAccountingBookName(string name)
        {
            return (_ProductAccountingBookRepository.FindBy(x => x.ProductAccountingBookName == name)).ToList() ?? new List<ProductAccountingBook>();

        }
        private void LogAndAuditError(GetAllAccountForASpecificProductTypeQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(GetAllAccountForASpecificProductTypeQuery request, string successMessage)
        {
          
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}