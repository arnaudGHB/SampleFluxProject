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
using CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Commands;
using CBS.TransactionManagement.Repository.AccountingDayOpening;

namespace CBS.TransactionManagement.MediatR.CashReplenishmentSubTellerP.Handlers
{
    /// <summary>
    /// Handles the command to add a new CashReplenishment.
    /// </summary>
    public class AddCashReplenishmentCommandHandler : IRequestHandler<AddCashReplenishmentSubTellerCommand, ServiceResponse<SubTellerCashReplenishmentDto>>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        //_primaryTellerProvisioningHistoryRepository
        private readonly ITellerRepository _tellerRepository; // Repository for accessing CashReplenishment data.
        private readonly ICashReplenishmentRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCashReplenishmentCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accessing CashReplenishment data.

        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddCashReplenishmentCommandHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCashReplenishmentCommandHandler(
            ICashReplenishmentRepository CashReplenishmentRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddCashReplenishmentCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null,
            ITellerRepository tellerRepository = null,
            IAccountRepository accountRepository = null,
            IDailyTellerRepository dailyTellerRepository = null,
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository = null,
            IAccountingDayRepository accountingDayRepository = null)
        {
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
            _tellerRepository = tellerRepository;
            _accountRepository = accountRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _accountingDayRepository = accountingDayRepository;
        }

        /// <summary>
        /// Handles the AddCashReplenishmentCommand to add a new CashReplenishment.
        /// </summary>
        /// <param name="request">The AddCashReplenishmentCommand containing CashReplenishment data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<SubTellerCashReplenishmentDto>> Handle(AddCashReplenishmentSubTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string Reference = BaseUtilities.GenerateInsuranceUniqueNumber(8, $"CSR-ST{_userInfoToken.BranchCode}-");

                // Get the active teller for the day
                var dailyTeller = await _dailyTellerRepository.GetAnActiveSubTellerForTheDate();

                // Validate teller provisioning account
                //var primaryTeller = await _primaryTellerProvisioningHistoryRepository.CheckIfPrimaryTellerIsOpened();

                // Retrieve the teller and their associated account
                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);
                var tellerAccount = await _accountRepository.RetrieveTellerAccount(teller);

                // Check if a CashReplenishment exists for the teller and branch
                var existingCashReplenishment = await _CashReplenishmentRepository.FindBy(c => c.TellerId == teller.Id &&
                                                                                                    c.ApprovedStatus != Status.Approved.ToString() &&
                                                                                                    !c.IsDeleted &&
                                                                                                    c.BranchId == teller.BranchId)
                                                                                     .FirstOrDefaultAsync();
                // If a CashReplenishment already exists, return a conflict response
                if (existingCashReplenishment != null)
                {
                    var errorMessage = $"You are having a pending request that needs to be validated.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashRequisitionSubteller, LogLevelInfo.Warning, Reference);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.Return403(errorMessage);
                }
                if (request.RequestedAmount==0)
                {
                    var errorMessage = $"Invalid amount entered.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashRequisitionSubteller, LogLevelInfo.Warning, Reference);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.Return403(errorMessage);
                }
                // Check if the requested amount exceeds the teller's maximum amount to manage
                if ((tellerAccount.Balance + request.RequestedAmount) > teller.MaximumAmountToManage)
                {
                    string errorMessage = $"The amount you requested has exceeded your teller's maximum amount.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.CashRequisitionSubteller, LogLevelInfo.Warning, Reference);
                    return ServiceResponse<SubTellerCashReplenishmentDto>.Return403(errorMessage);
                }
                // Map the command to a CashReplenishmentSubTeller entity
                var cashReplenishmentEntity = _mapper.Map<CashReplenishmentSubTeller>(request);
                cashReplenishmentEntity.Id = BaseUtilities.GenerateUniqueNumber();
                cashReplenishmentEntity.ApprovedStatus = Status.Pending.ToString();
                cashReplenishmentEntity.BranchId = _userInfoToken.BranchID;
                cashReplenishmentEntity.TellerId = teller.Id;
                cashReplenishmentEntity.ApprovedDate = DateTime.MaxValue;
                cashReplenishmentEntity.InitializeDate = BaseUtilities.UtcNowToDoualaTime();
                cashReplenishmentEntity.RequesterUserId = _userInfoToken.Id;
                cashReplenishmentEntity.RequesterName = _userInfoToken.FullName;
                cashReplenishmentEntity.TransactionReference = Reference;
                // Add the new CashReplenishment entity to the repository
                _CashReplenishmentRepository.Add(cashReplenishmentEntity);

                // Save changes to the database
                await _uow.SaveAsync();

                // Log and audit the successful creation of CashReplenishment
                string msg = $"CashReplenishment for sub till {teller.Name} was created successfully";
                await BaseUtilities.LogAndAuditAsync(msg, request, HttpStatusCodeEnum.OK, LogAction.CashRequisitionSubteller, LogLevelInfo.Information, Reference);

                // Map the CashReplenishment entity to CashReplenishmentDto and return it with a success response
                var cashReplenishmentDto = _mapper.Map<SubTellerCashReplenishmentDto>(cashReplenishmentEntity);
                return ServiceResponse<SubTellerCashReplenishmentDto>.ReturnResultWith200(cashReplenishmentDto, msg);
            }
            catch (InvalidOperationException ex)
            {
                string messageError = $"Validation error occurred while processing cash requisition: {ex.Message}";
                _logger.LogError(messageError);
                await BaseUtilities.LogAndAuditAsync(messageError, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashRequisitionSubteller, LogLevelInfo.Error, null);
                return ServiceResponse<SubTellerCashReplenishmentDto>.Return400(ex.Message);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CashRequisitionSubteller: {e.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CashRequisitionSubteller, LogLevelInfo.Error,null);
                return ServiceResponse<SubTellerCashReplenishmentDto>.Return500(e, errorMessage);
            }
        }



    }

}
