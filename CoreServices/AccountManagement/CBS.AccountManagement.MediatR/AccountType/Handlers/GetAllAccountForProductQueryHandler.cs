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
 
    public class GetAllAccountForProductQueryHandler : IRequestHandler<GetAllAccountForProductQuery, ServiceResponse<List<AccountProduct>>>
    {
        private readonly IProductAccountingBookRepository _ProductAccountingBookRepository; // Repository for accessing AccountType data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<GetAllAccountForProductQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;
 
        public GetAllAccountForProductQueryHandler(
            IProductAccountingBookRepository AccountTypeRepository,
            IMapper mapper,
            ILogger<GetAllAccountForProductQueryHandler> logger,UserInfoToken userInfoToken, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _ProductAccountingBookRepository = AccountTypeRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

  
        public async Task<ServiceResponse<List<AccountProduct>>> Handle(GetAllAccountForProductQuery request, CancellationToken cancellationToken)
        {
            List<AccountProduct> accounts = new List<AccountProduct>();
            string errorMessage = null;
            try
            {

                var entities = GetExistingAccountType(request);
                if (entities.Count()>0)
                {
                

                    foreach (var item in entities)
                    {
                        var product = await _chartOfAccountManagementPositionRepository.FindAsync(item.ChartOfAccountId);
                        AccountProduct accountProduct = new AccountProduct
                        {
                            OperationEvent = item.Id.Split('@')[1],
                            Id = item.Id,
                            AccountNumber = product.AccountNumber.PadRight(6,'0') + product.PositionNumber.PadRight(3, '0') + "[BCD]",
                            Description = product.Description,
                            Status = product.Description.Equals("ROOT ACCOUNT") ? "Not Applicable" : "In Used"

                        };

                            accounts.Add(accountProduct);
                     }
                  
                        // Map the AccountType entity to AccountTypeDto and return it with a success response
                    
                        string successMessage = $"Gotten all accounts in used for product  {request.ProductName} successfully.";
                        LogAuditSuccess( request, successMessage);
                        
                        return ServiceResponse < List <AccountProduct>> .ReturnResultWith200(accounts, successMessage);
                        
                     
                }
                else
                {
                    // If the AccountType entity was not found, log the error and return a 404 Not Found response
                    errorMessage= "ProductAccountingBookDto not found.";
                    _logger.LogError(errorMessage);
                    LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  404);
                    return ServiceResponse<List<AccountProduct>>.Return404(errorMessage);
                    
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting AccountType: {e.Message}";
                LogAndAuditError(request, errorMessage, LogLevelInfo.Error,  500);
                _logger.LogError(errorMessage);
                return ServiceResponse<List<AccountProduct>>.Return404(errorMessage);
 
            }
        }
        private  List<ProductAccountingBook> GetExistingAccountType(GetAllAccountForProductQuery request)
        {
            return  (_ProductAccountingBookRepository.FindBy(x => x.Name == request.ProductName)).ToList  ()?? new List<ProductAccountingBook>();
               
        }

        private void LogAndAuditError(GetAllAccountForProductQuery request, string errorMessage, LogLevelInfo logLevel, int statusCode)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                JsonConvert.SerializeObject(request), errorMessage, logLevel.ToString(), statusCode,_userInfoToken.Token).Wait();
        }

        private void LogAuditSuccess(GetAllAccountForProductQuery request, string successMessage)
        {
          
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(),
                JsonConvert.SerializeObject(request), successMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token).Wait();
        }
    }
}