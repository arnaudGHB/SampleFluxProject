using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new ChartOfAccount.
    /// </summary>
    public class AddChartOfAccountCommandHandler : IRequestHandler<AddChartOfAccountCommand, ServiceResponse<ChartOfAccountDto>>
    {
        private readonly IChartOfAccountRepository _ChartOfAccountRepository; // Repository for accessing ChartOfAccount data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddChartOfAccountCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddChartOfAccountCommandHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for ChartOfAccount data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>

        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddChartOfAccountCommandHandler(
            IChartOfAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<AddChartOfAccountCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _ChartOfAccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddChartOfAccountCommand to add a new ChartOfAccount.
        /// </summary>
        /// <param name="request">The AddChartOfAccountCommand containing Account data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChartOfAccountDto>> Handle(AddChartOfAccountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                //var  Account = await _ChartOfAccountRepository.FindAsync(request.RootParentId);

                //if ( Account == null)
                //{
                //    var errorMessage = $"There is already an existing account with AccountNumber {request.AccountNumber}.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<ChartOfAccountDto>.Return409(errorMessage);
                //}

                if (request.AccountNumber.Length == 1 )
                {
                    string Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "COA");
                    var model = new Data.ChartOfAccount
                    {
                        Id = Id,
                        AccountNumber = request.AccountNumber,
                        LabelFr = request.LabelFr,
                        LabelEn = request.LabelEn,
                        ParentAccountNumber = "0",
                        IsDeleted = false,
                        ParentAccountId = Id,
                        AccountCartegoryId = request.AccountCartegoryId,
                        TempData = request.AccountNumber.PadRight(6, '0'),// + _userInfoToken.BankCode + _userInfoToken.BranchCodeX,
                        IsDebit = request.IsDebit
                    };
                    // Map the AddChartOfAccountCommand to a ChartOfAccount entity

                    model = Data.ChartOfAccount.UpdateAccountEntity(model);
                    // Add the new ChartOfAccount entity to the repository
                    _ChartOfAccountRepository.Add(model);
                    await _uow.SaveAsync();
                    // Map the Account entity to ChartOfAccount and return it with a success response
                    ChartOfAccountDto AccountDto = new ChartOfAccountDto();
                    AccountDto = _mapper.Map(model, AccountDto);
                    return ServiceResponse<ChartOfAccountDto>.ReturnResultWith200(AccountDto);
                }
                else
                {
                    // Check if a ChartOfAccount with the same name already exists (case-insensitive)
                    var parentAccount = await _ChartOfAccountRepository.FindAsync(request.RootParentId);

                    // If a Account with the same name already exists, return a conflict response
                    if (parentAccount == null)
                    {
                        var errorMessage = $"Root Account {request.AccountNumber} does not exists with AccountName {request.LabelEn}.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<ChartOfAccountDto>.Return409(errorMessage);
                    }
                    var existingAccount = _ChartOfAccountRepository.FindBy(x => x.AccountNumber.Equals(request.AccountNumber) && x.IsDeleted == false).FirstOrDefault();

                    // If a Account with the same name already exists, return a conflict response
                    if (existingAccount != null)
                    {
                        var errorMessage =
                            $"Account Number :{request.AccountNumber} already exists with AccountName {existingAccount.LabelFr}.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<ChartOfAccountDto>.Return409(errorMessage);
                    }

                    var model = new Data.ChartOfAccount
                    {
                        Id = BaseUtilities.GenerateInsuranceUniqueNumber(10, "COA"),
                        AccountNumber = request.AccountNumber,
                        LabelFr = request.LabelFr,
                        LabelEn = request.LabelEn,
                        ParentAccountNumber = parentAccount.AccountNumber,
                        IsDeleted = false,
                        ParentAccountId = parentAccount.ParentAccountId,
                        AccountCartegoryId = request.AccountCartegoryId,

                        IsDebit = request.IsDebit
                    };
                    // Map the AddChartOfAccountCommand to a ChartOfAccount entity

                    model = Data.ChartOfAccount.UpdateAccountEntity(model);
                    // Add the new ChartOfAccount entity to the repository
                    _ChartOfAccountRepository.Add(model);
                    await _uow.SaveAsync();

                    // Map the Account entity to ChartOfAccount and return it with a success response
                    ChartOfAccountDto AccountDto = new ChartOfAccountDto();
                    AccountDto = _mapper.Map(model, AccountDto);
                    return ServiceResponse<ChartOfAccountDto>.ReturnResultWith200(AccountDto);
                }

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Account: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<ChartOfAccountDto>.Return500(e);
            }
        }
    }
}