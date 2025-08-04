using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.Handlers
{
    public class GetProductConfigurationStatusQueryHandler : IRequestHandler<GetProductConfigurationStatusQuery, ServiceResponse<List<ProductAccountConfiguration>>>
    {
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository; // Repository for accessing AccountTypes data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetProductConfigurationStatusQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingRuleEntryRepository _accountingEntryRepository;
        private readonly IProductAccountingBookRepository _productAccountingBookRepository;
        /// <summary>
        /// Constructor for initializing the GetAllAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountTypes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetProductConfigurationStatusQueryHandler(
            IChartOfAccountManagementPositionRepository AccountTypeRepository,
            IMapper mapper, ILogger<GetProductConfigurationStatusQueryHandler> logger, UserInfoToken userInfoToken, IAccountingRuleEntryRepository? accountingEntryRepository, IProductAccountingBookRepository? productAccountingBookRepository)
        {
            // Assign provided dependencies to local variables.
            _chartOfAccountManagementPositionRepository = AccountTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _accountingEntryRepository = accountingEntryRepository;
            _productAccountingBookRepository = productAccountingBookRepository;
        }

        /// <summary>
        /// Handles the GetAllAccountTypeQuery to retrieve all AccountTypes.
        /// </summary>
        /// <param name="request">The GetAllAccountTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ProductAccountConfiguration>>> Handle(GetProductConfigurationStatusQuery request, CancellationToken cancellationToken)
        {
            List<string> ListOfEvents = new List<string>();
            List<string> ListOfOperationEvents = new List<string>();
            List<ProductAccountConfiguration> ListOfProductAccountConfiguration = new List<ProductAccountConfiguration>();
            try
            {
                if (request.IdType.Equals("loan"))
                {
                    ListOfEvents = GetLoanOperationEvent();
                }
                else
                {
                      ListOfEvents = GetSavingOperationEvent();

                }
                foreach (var item in ListOfEvents)
                {
                    ListOfOperationEvents.Add(request.Id+"@"+item);
                }
                foreach (var item in ListOfOperationEvents)
                {
                    var model = _accountingEntryRepository.FindBy(xx => xx.EventCode.Equals(item)).FirstOrDefault();
                    var modelChart = await _chartOfAccountManagementPositionRepository.FindAsync(model.DeterminationAccountId);
                    ProductAccountConfiguration configuration = new ProductAccountConfiguration
                    {
                        Id = model.Id,
                        OperationEvent = item.Split('@')[1],
                        AccountName = modelChart.Description== "ROOT ACCOUNT"?"Default Value": modelChart.Description,
                        AccountNumber = modelChart.Description == "ROOT ACCOUNT"?"000000" : modelChart.AccountNumber.PadRight(6, '0') +modelChart.PositionNumber.PadRight(3,'0') + "[BranchCodeX]",
                        Key = item
                        
                    };
                    ListOfProductAccountConfiguration.Add(configuration);
                }
               
                // Retrieve the accounting entries matching the request ID
                

       
                return ServiceResponse<List<ProductAccountConfiguration>>.ReturnResultWith200(ListOfProductAccountConfiguration);
            }
            catch (Exception e)
            {

                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Failed to get all AccountTypes: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountTypeQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<ProductAccountConfiguration>>.Return500(e, errorMessage);
            }
        }

        public List<string> GetSavingOperationEvent()
        {
            return new List<string>
                                    {
                                        "Commission_Account",
                                        "Principal_Saving_Account",
                                        "Transfer_Fee_Account",
                                        "Saving_Interest_Expense_Account",
                                        "Saving_Interest_Account",
                                        "Liasson_Account",
                                        "Withdrawal_Fee_Account"
                                    };
        }

        public List<string> GetLoanOperationEvent()
        {
            return new List<string>
                                    {
                                        "Loan_Principal_Account",
                                        "Loan_Provisioning_Account",
                                        "Loan_VAT_Account",
                                        "Loan_Penalty_Account",
                                        "Loan_Fee_Account",
                                        "Loan_Interest_Account",
                                        "Loan_Transit_Account",
                                        "Loan_WriteOff_Account"
                                    };
        }
    }


    public class GetAllProductConfigurationStatusQueryHandler : IRequestHandler<GetAllProductConfigurationStatusQuery, ServiceResponse<List<ProductAccountConfiguration>>>
    {
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository; // Repository for accessing AccountTypes data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllProductConfigurationStatusQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingRuleEntryRepository _accountingEntryRepository;
        private readonly IProductAccountingBookRepository _productAccountingBookRepository;
        /// <summary>
        /// Constructor for initializing the GetAllAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountTypes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllProductConfigurationStatusQueryHandler(
            IChartOfAccountManagementPositionRepository AccountTypeRepository,
            IMapper mapper, ILogger<GetAllProductConfigurationStatusQueryHandler> logger, UserInfoToken userInfoToken, IAccountingRuleEntryRepository? accountingEntryRepository, IProductAccountingBookRepository? productAccountingBookRepository)
        {
            // Assign provided dependencies to local variables.
            _chartOfAccountManagementPositionRepository = AccountTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _accountingEntryRepository = accountingEntryRepository;
            _productAccountingBookRepository = productAccountingBookRepository;
        }

        /// <summary>
        /// Handles the GetAllAccountTypeQuery to retrieve all AccountTypes.
        /// </summary>
        /// <param name="request">The GetAllAccountTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<ProductAccountConfiguration>>> Handle(GetAllProductConfigurationStatusQuery request, CancellationToken cancellationToken)
        {
            List<string> ListOfEvents = new List<string>();
            List<string> ListOfOperationEvents = new List<string>();
            List<ProductAccountConfiguration> ListOfProductAccountConfiguration = new List<ProductAccountConfiguration>();
            try
            {
                
                foreach (var item in ListOfOperationEvents)
                {
                    var model = _accountingEntryRepository.FindBy(xx => xx.EventCode.Equals(item)).FirstOrDefault();
                    var modelChart = await _chartOfAccountManagementPositionRepository.FindAsync(model.DeterminationAccountId);
                    ProductAccountConfiguration configuration = new ProductAccountConfiguration
                    {
                        Id = model.Id,
                        OperationEvent = item.Split('@')[1],
                        AccountName = modelChart.Description == "ROOT ACCOUNT" ? "Default Value" : modelChart.Description,
                        AccountNumber = modelChart.Description == "ROOT ACCOUNT" ? "000000" : modelChart.AccountNumber.PadRight(6, '0') + modelChart.PositionNumber.PadRight(3, '0') + "[BranchCodeX]",
                        Key = item

                    };
                    ListOfProductAccountConfiguration.Add(configuration);
                }

                // Retrieve the accounting entries matching the request ID



                return ServiceResponse<List<ProductAccountConfiguration>>.ReturnResultWith200(ListOfProductAccountConfiguration);
            }
            catch (Exception e)
            {

                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Failed to get all AccountTypes: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountTypeQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<ProductAccountConfiguration>>.Return500(e, errorMessage);
            }
        }

        public List<string> GetSavingOperationEvent()
        {
            return new List<string>
                                    {
                                        "Commission_Account",
                                        "Principal_Saving_Account",
                                        "Transfer_Fee_Account",
                                        "Saving_Interest_Expense_Account",
                                        "Saving_Interest_Account",
                                        "Liasson_Account",
                                        "Withdrawal_Fee_Account"
                                    };
        }

        public List<string> GetLoanOperationEvent()
        {
            return new List<string>
                                    {
                                        "Loan_Principal_Account",
                                        "Loan_Provisioning_Account",
                                        "Loan_VAT_Account",
                                        "Loan_Penalty_Account",
                                        "Loan_Fee_Account",
                                        "Loan_Interest_Account",
                                        "Loan_Transit_Account",
                                        "Loan_WriteOff_Account"
                                    };
        }
    }
}
