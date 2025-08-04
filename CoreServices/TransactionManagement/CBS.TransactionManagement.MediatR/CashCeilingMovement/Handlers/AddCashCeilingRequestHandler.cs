using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Helper.Helper;
using Microsoft.IdentityModel.Tokens;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.MediatR;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.CashCeilingMovement.Commands;
using CBS.TransactionManagement.Data.Entity.CashCeilingMovement;
using CBS.TransactionManagement.Repository.CashCeilingMovement;
using System.Net;

namespace CBS.TransactionManagement.CashCeilingMovement.Handlers
{
    /// <summary>
    /// Handles the creation of cash ceiling requests for primary to vault or subteller to primary till transactions.
    /// </summary>
    public class AddCashCeilingRequestHandler : IRequestHandler<AddCashCeilingRequestCommand, ServiceResponse<bool>>
    {
        private readonly ICashCeilingRequestRepository _cashCeilingRequestRepository;
        private readonly ITellerRepository _tellerRepository;
        private readonly IDailyTellerRepository _dailyTellerRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AddCashCeilingRequestHandler> _logger;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMapper _mapper;

        public AddCashCeilingRequestHandler(
            ICashCeilingRequestRepository cashCeilingRequestRepository,
            ITellerRepository tellerRepository,
            IDailyTellerRepository dailyTellerRepository,
            IAccountRepository accountRepository,
            ILogger<AddCashCeilingRequestHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            UserInfoToken userInfoToken,
            IMapper mapper)
        {
            _cashCeilingRequestRepository = cashCeilingRequestRepository;
            _tellerRepository = tellerRepository;
            _dailyTellerRepository = dailyTellerRepository;
            _accountRepository = accountRepository;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the creation of cash ceiling requests.
        /// </summary>
        /// <param name="request">Command containing request details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(AddCashCeilingRequestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Determine the teller type and retrieve active teller details
                var dailyTeller = request.RequestType == EventCode.Subteller_Cash_To_PrimaryTeller.ToString()
                    ? await _dailyTellerRepository.GetAnActiveSubTellerForTheDate()
                    : await _dailyTellerRepository.GetAnActivePrimaryTellerForTheDate();

                if (dailyTeller == null)
                {
                    string errorMessage = "No active teller found for the requested type.";
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.NotFound,
                        LogAction.CashCeillingCashOutRequest,
                        LogLevelInfo.Warning
                    );
                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                var teller = await _tellerRepository.GetTeller(dailyTeller.TellerId);

                // Validate if there is already a pending request for this teller and type
                var existingRequest = await _cashCeilingRequestRepository
                    .FindBy(c => c.TellerId == teller.Id &&
                                 c.RequestType == request.RequestType &&
                                 c.ApprovedStatus == Status.Pending.ToString() &&
                                 !c.IsDeleted)
                    .FirstOrDefaultAsync();

                if (existingRequest != null)
                {
                    string errorMessage = $"A pending cash ceiling request already exists for teller {teller.Name}.";
                    await BaseUtilities.LogAndAuditAsync(
                        errorMessage,
                        request,
                        HttpStatusCodeEnum.Conflict,
                        LogAction.CashCeillingCashOutRequest,
                        LogLevelInfo.Warning
                    );
                    return ServiceResponse<bool>.Return409(errorMessage);
                }

                // Map the request to the cash ceiling entity
                var cashCeilingRequest = MapCashCeilingRequest(request, teller);

                // Save the request to the repository
                _cashCeilingRequestRepository.Add(cashCeilingRequest);
                await _uow.SaveAsync();

                // Log and audit success
                string successMessage =
                    $"Cash ceiling request of type {request.RequestType} for cashout created successfully for teller {teller.Name} " +
                    $"in {_userInfoToken.BranchName}, initiated by {_userInfoToken.FullName}.";
                await BaseUtilities.LogAndAuditAsync(
                    successMessage,
                    request,
                    HttpStatusCodeEnum.OK,
                    LogAction.CashCeillingCashOutRequest,
                    LogLevelInfo.Information,
                    cashCeilingRequest.TransactionReference
                );

                return ServiceResponse<bool>.ReturnResultWith200(true, successMessage);
            }
            catch (Exception ex)
            {
                // Log and handle exceptions
                string errorMessage =
                    $"Error occurred while creating cash ceiling request for {_userInfoToken.BranchName}, initiated by {_userInfoToken.FullName}. " +
                    $"Error: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.CashCeillingCashOutRequest,
                    LogLevelInfo.Error
                );
                return ServiceResponse<bool>.Return500(ex, errorMessage);
            }
        }

        /// <summary>
        /// Maps the request to a CashCeilingRequest entity and populates additional fields.
        /// </summary>
        /// <param name="request">The original command.</param>
        /// <param name="teller">The associated teller details.</param>
        /// <returns>A populated CashCeilingRequest entity.</returns>
        private CashCeilingRequest MapCashCeilingRequest(AddCashCeilingRequestCommand request, Teller teller)
        {
            var cashCeilingRequest = _mapper.Map<CashCeilingRequest>(request);
            cashCeilingRequest.Id = BaseUtilities.GenerateUniqueNumber();
            cashCeilingRequest.TellerId = teller.Id;
            cashCeilingRequest.BranchId = teller.BranchId;
            cashCeilingRequest.RequestedByUserId = _userInfoToken.Id;
            cashCeilingRequest.BranchCode = _userInfoToken.BranchCode;
            cashCeilingRequest.BranchName = _userInfoToken.BranchName;
            cashCeilingRequest.RequestedBy = _userInfoToken.FullName ?? _userInfoToken.Email;
            cashCeilingRequest.InitializeDate = BaseUtilities.UtcNowToDoualaTime();
            cashCeilingRequest.ApprovedStatus = Status.Pending.ToString();
            cashCeilingRequest.TransactionReference = request.RequestType == EventCode.Subteller_Cash_To_PrimaryTeller.ToString()
                ? BaseUtilities.GenerateInsuranceUniqueNumber(8, $"STV-{_userInfoToken.BranchCode}-")
                : BaseUtilities.GenerateInsuranceUniqueNumber(8, $"PTV-{_userInfoToken.BranchCode}-");
            return cashCeilingRequest;
        }
    }
}
