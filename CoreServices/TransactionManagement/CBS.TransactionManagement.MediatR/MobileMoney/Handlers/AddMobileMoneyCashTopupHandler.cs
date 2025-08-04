using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using System.Globalization;
using AutoMapper.Internal;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Repository.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MobileMoney;

namespace CBS.TransactionManagement.MediatR.MobileMoney.Handlers
{
    /// <summary>
    /// Handles the command to add a new CashReplenishment.
    /// </summary>
    public class AddMobileMoneyCashTopupHandler : IRequestHandler<AddMobileMoneyCashTopupCommand, ServiceResponse<MobileMoneyCashTopupDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddMobileMoneyCashTopupHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IMobileMoneyCashTopupRepository _mobileMoneyCashTopupRepository;
        private readonly ITellerRepository _tellerRepository;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddCashReplenishmentCommandHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddMobileMoneyCashTopupHandler(
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddMobileMoneyCashTopupHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            IAccountRepository accountRepository = null,
            IAccountingDayRepository accountingDayRepository = null,
            IMobileMoneyCashTopupRepository mobileMoneyCashTopupRepository = null,
            ITellerRepository tellerRepository = null)
        {
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _accountRepository = accountRepository;
            _accountingDayRepository = accountingDayRepository;
            _mobileMoneyCashTopupRepository = mobileMoneyCashTopupRepository;
            _tellerRepository = tellerRepository;
        }

        /// <summary>
        /// Handles the AddCashReplenishmentCommand to add a new CashReplenishment.
        /// </summary>
        /// <param name="request">The AddCashReplenishmentCommand containing CashReplenishment data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<MobileMoneyCashTopupDto>> Handle(AddMobileMoneyCashTopupCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Get the active teller for the day
                var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(_userInfoToken.BranchID);

                var mobileMoneyAccount = await _accountRepository.RetrieveMobileMoneyTellerAccount(request.BranchId,request.OperatorType);
                // Check if a MobileMoneyCashTopup exists for the given MobileMoneyTransactionId
                var mobileMoneyCashTopup = await _mobileMoneyCashTopupRepository
                    .FindBy(c => c.MobileMoneyTransactionId == request.MobileMoneyTransactionId &&c.IsDeleted==false)
                    .FirstOrDefaultAsync();

                if (mobileMoneyCashTopup != null)
                {
                    var errorMessage = $"{mobileMoneyAccount.AccountType} Cash Topup with Transaction ID '{request.MobileMoneyTransactionId}' already exists.";
                    LogAndAuditError(errorMessage, request, 403);
                    return ServiceResponse<MobileMoneyCashTopupDto>.Return403(errorMessage);
                }

                var memberAccount = await _accountRepository.GetAccountByAccountNumber(request.AccountNumber);

                if (memberAccount.AccountType != AccountType.MobileMoneyMTN.ToString() &&
                    memberAccount.AccountType != AccountType.MobileMoneyORANGE.ToString())
                {
                    var errorMessage = $"The provided account number '{request.AccountNumber}' is invalid. Only Mobile Money accounts of type 'MobileMoneyMTN' or 'MobileMoneyORANGE' are allowed for this operation. Please verify the account type and try again.";
                    LogAndAuditError(errorMessage, request, 403);
                    return ServiceResponse<MobileMoneyCashTopupDto>.Return403(errorMessage);
                }

                if (memberAccount.BranchId !=_userInfoToken.BranchID)
                {
                    var errorMessage = $"The account number '{request.AccountNumber}' does not belong to your branch. Please verify the account details or contact support for assistance.";
                    LogAndAuditError(errorMessage, request, 403);
                    return ServiceResponse<MobileMoneyCashTopupDto>.Return403(errorMessage);

                }
                var teller = await _tellerRepository.FindAsync(mobileMoneyAccount.TellerId);

                if (teller == null)
                {
                    var errorMessage = $"{mobileMoneyAccount.AccountType} teller {request.SourceType} does not exist.";
                    LogAndAuditError(errorMessage, mobileMoneyAccount, 404);
                    throw new InvalidOperationException(errorMessage);
                }
    
                // Check if the requested amount exceeds the teller's maximum amount to manage
                if ((mobileMoneyAccount.Balance + request.Amount) > teller.MaximumAmountToManage)
                {
                    string errorMessage = $"The requested amount of {BaseUtilities.FormatCurrency(request.Amount)} exceeds teller {mobileMoneyAccount.AccountType} maximum allowable balance of {BaseUtilities.FormatCurrency(teller.MaximumAmountToManage)}." +
                                          $" Current account balance: {BaseUtilities.FormatCurrency(mobileMoneyAccount.Balance)}.";

                    _logger.LogError(errorMessage);
                    LogAndAuditError(errorMessage, request, 403);

                    return ServiceResponse<MobileMoneyCashTopupDto>.Return403(errorMessage);
                }
                string Code = request.OperatorType == AccountType.MobileMoneyMTN.ToString() ? "MMC-T" : "ORM-T";
                // Map the command to a MobileMoneyCashTopup entity
                var cashReplenishmentEntity = _mapper.Map<MobileMoneyCashTopup>(request);
                string Reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"{Code}{_userInfoToken.BranchCode}-");
                cashReplenishmentEntity.Id = BaseUtilities.GenerateUniqueNumber();
                cashReplenishmentEntity.RequestApprovalStatus = Status.Pending.ToString();
                cashReplenishmentEntity.BranchId = _userInfoToken.BranchID;
                cashReplenishmentEntity.BankId = mobileMoneyAccount.BankId;
                cashReplenishmentEntity.RequestApprovalDate = DateTime.MaxValue;
                cashReplenishmentEntity.RequestDate = BaseUtilities.UtcNowToDoualaTime();
                cashReplenishmentEntity.RequestInitiatedBy = _userInfoToken.FullName;
                cashReplenishmentEntity.RequestNote = request.RequestNote;
                cashReplenishmentEntity.RequestApprovedBy = "N/A";
                cashReplenishmentEntity.RequestApprovalNote = "N/A";
                cashReplenishmentEntity.MobileMoneyMemberReference = memberAccount.CustomerId;
                cashReplenishmentEntity.RequestReference = Reference;
                cashReplenishmentEntity.TellerId = teller.Id;
                cashReplenishmentEntity.PhoneNumber = teller.MobileMoneyFloatNumber;
                _mobileMoneyCashTopupRepository.Add(cashReplenishmentEntity);
                // Save changes to the database
                await _uow.SaveAsync();
                // Log and audit the successful creation of CashReplenishment
                string msg = $"{mobileMoneyAccount.AccountType} Cash transfer of {BaseUtilities.FormatCurrency(request.Amount)} for till {teller.Name} was created initiated by {_userInfoToken.FullName}";
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Create.ToString(), request, msg, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the CashReplenishment entity to CashReplenishmentDto and return it with a success response
                var cashReplenishmentDto = _mapper.Map<MobileMoneyCashTopupDto>(cashReplenishmentEntity);
                return ServiceResponse<MobileMoneyCashTopupDto>.ReturnResultWith200(cashReplenishmentDto, msg);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CashReplenishment: {e.Message}";
                LogAndAuditError(errorMessage, request, 500, e);
                return ServiceResponse<MobileMoneyCashTopupDto>.Return500(e, errorMessage);
            }
        }

        // Utility method to log and audit errors
        private void LogAndAuditError(string errorMessage, object request, int statusCode, Exception exception = null)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }



    }

}
