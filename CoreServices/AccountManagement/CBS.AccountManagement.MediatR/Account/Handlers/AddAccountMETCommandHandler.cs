using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Commands;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Account.
    /// </summary>
    public class AddAccountMETCommandHandler : IRequestHandler<AddAccountMETCommand, ServiceResponse<AccountResponseDto>>
    {
        private readonly IChartOfAccountRepository _ChartOfAccountRepository;
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAccountMETCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IChartOfAccountManagementPositionRepository _chartOfAccountManagementPositionRepository;

        /// <summary>
        /// Constructor for initializing the AddAccountMETCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAccountMETCommandHandler(
            IAccountRepository AccountRepository,
            IChartOfAccountRepository ChartOfAccountRepository,
            IMapper mapper,
            ILogger<AddAccountMETCommandHandler> logger,PathHelper pathHelper,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken, IChartOfAccountManagementPositionRepository? chartOfAccountManagementPositionRepository)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _ChartOfAccountRepository = ChartOfAccountRepository;
            _chartOfAccountManagementPositionRepository = chartOfAccountManagementPositionRepository;
            _logger = logger;
            _uow = uow;
        _pathHelper = pathHelper;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddAccountMETCommand to add a new Account.
        /// </summary>
        /// <param name="request">The AddAccountMETCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AccountResponseDto>> Handle(AddAccountMETCommand request, CancellationToken cancellationToken)
        {
            Data.ChartOfAccount model = new Data.ChartOfAccount();
            try
            {

                var chart = await _chartOfAccountManagementPositionRepository.FindAsync(request.ChartOfAccountManagementPositionId);
                chart.ChartOfAccount = await _ChartOfAccountRepository.FindAsync(chart.ChartOfAccountId);
                if (chart.ChartOfAccount == null)
                {

                    var errorMessage = $"ChartOfAccountId :{request.AccountNumber} does not exist in the accounting Chart.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountMETCommand",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<AccountResponseDto>.Return409(errorMessage);
                }

                // Check if a Account with the same name already exists (case-insensitive)

                // Map the AddAccountMETCommand to a Account entity
                var AccountEntity = new Data.Account(); // _mapper.Map<Data.Account>(request);

                AccountEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "AC");
                AccountEntity.AccountCategoryId = chart.ChartOfAccount.AccountCartegoryId;
                if (request.AccountNumber.Contains("451"))
                {
                    List<Branch> branches = await APICallHelper.GetAllBranchInfos(_pathHelper, _userInfoToken);
                    var tempRemittance = request.AccountTypeId.Split("@");
                  
                        request.AccountName = GetLiaisonName( request.LiaisonBranchCode,branches);
                        AccountEntity.SetAccount451Entity(request.OwnerBranchCode, AccountEntity, chart.ChartOfAccount.AccountCartegoryId, request.AccountNumber, request.AccountNumberCU, request.AccountName, request.ChartOfAccountManagementPositionId, request.AccountOwnerId, _pathHelper.BankManagement_BankCode, request.LiaisonBranchCode, chart.PositionNumber, request.AccountTypeId);


                   
                }
                else
                {
                    AccountEntity.SetAccountEntity(AccountEntity, chart.ChartOfAccount.AccountCartegoryId, request.AccountNumber, request.AccountName, request.ChartOfAccountManagementPositionId, request.AccountOwnerId, request.AccountNumberCU, _userInfoToken.BranchCode, chart.PositionNumber);

                }
                // Add the new Account entity to the repository
                _AccountRepository.Add(AccountEntity);
                await _uow.SaveAsync();



                // Map the Account entity to AccountDto and return it with a success response
                var AccountDto = _mapper.Map<AccountDto>(AccountEntity);
                var errorMessag = $"new Account created Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountMETCommand",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<AccountResponseDto>.ReturnResultWith200(new AccountResponseDto { AccountName= AccountEntity .AccountName, AccountNumber= AccountEntity .AccountNumber,Id= AccountEntity .Id});
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {BaseUtilities.GetInnerExceptionMessages(e)}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddAccountMETCommand",
                    request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<AccountResponseDto>.Return500(e);
            }
        }

        private string GetLiaisonName(string branchCode, List<Branch> _branches)
        {
            var branch = _branches.FirstOrDefault(x => x.branchCode == branchCode);
            return _userInfoToken.BranchName + "-" + branch?.name + " Liaison Account";
        }
    }
}