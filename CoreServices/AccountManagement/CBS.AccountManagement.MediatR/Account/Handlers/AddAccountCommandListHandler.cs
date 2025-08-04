using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddAccountCommandListHandler : IRequestHandler<AddAccountCommandList, ServiceResponse<List<AccountDto>>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountCommandListHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;

        /// <summary>
        /// Constructor for initializing the AddAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountCommandListHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<AddAccountCommandListHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IChartOfAccountRepository? chartOfAccountRepository, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _AccountRepository = AccountRepository;
            _ChartOfAccountRepository = chartOfAccountRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountDto>>> Handle(AddAccountCommandList request, CancellationToken cancellationToken)
        {
            try
            {
                Data.ChartOfAccount model = new Data.ChartOfAccount();
                List<AccountDto> Modelist = new List<AccountDto>();
                foreach (var item in request.AccountCommands)
                {
                    var existingAccount = await _AccountRepository.FindBy(c => c.AccountNumber == item.AccountNumber).FirstOrDefaultAsync();
                    // If a Account with the same name already exists, return a conflict response
                    if (existingAccount != null)
                    {

                        var errorMessage = $"Account {item.AccountNumber} already exists.";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommandList",
                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                        return ServiceResponse<List<AccountDto>>.Return409(errorMessage);
                    }
                    // Map the AddAccountCommand to a Account entity
                    var modelItem = _ChartOfAccountRepository.FindBy(f => f.AccountNumber == item.AccountNumber);
                   
                    
                    if (modelItem.Any())
                    {
                        model= modelItem.FirstOrDefault();
                    }
                    else
                    {
                        var errorMessage = $"No ChartOfAccount with this number:{item.AccountNumber}In the system.";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommandList",
                            JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                        return ServiceResponse<List<AccountDto>>.Return409(errorMessage);

                    }
                    var AccountEntity = _mapper.Map<Data.Account>(item);
                    AccountEntity.Id = BaseUtilities.GenerateUniqueNumber();
               var chartPosition = await _chartOfAccountManagementPositionRepository.FindAsync(item.ChartOfAccountManagementPositionId);
                    AccountEntity.SetAccountEntity(AccountEntity, model.AccountCartegoryId, item.AccountNumber, item.AccountName, model.Id, item.AccountOwnerId, "012", _userInfoToken.BranchCode, chartPosition.PositionNumber);

                    // Add the new Account entity to the repository
                    _AccountRepository.Add(AccountEntity);
                    var AccountDto = _mapper.Map<AccountDto>(AccountEntity);
                    Modelist.Add(AccountDto);

                }
                await _uow.SaveAsync();
                // Map the Account entity to AccountDto and return it with a success response
                var errorMessag = $"new Account created Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommandList",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<AccountDto>>.ReturnResultWith200(Modelist);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountCommandList",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<AccountDto>>.Return500(e, errorMessage);
            }
        }

        public string GetAccountNumberManagementPosition(Data.ChartOfAccount chartOfAccount)
        {
            string numberString = string.Empty;
            if (chartOfAccount.HasManagementPostion.Value)
            {
                var models = _chartOfAccountManagementPositionRepository.FindBy(f => f.ChartOfAccountId == chartOfAccount.Id);

                if (models.Any())
                {
                    numberString = models.FirstOrDefault().PositionNumber.PadRight(3, '0');
                }
            }
            return numberString;
        }

    }
}