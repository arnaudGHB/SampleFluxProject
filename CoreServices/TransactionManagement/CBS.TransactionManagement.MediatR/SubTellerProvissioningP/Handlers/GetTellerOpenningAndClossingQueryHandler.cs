using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific PrimaryTellerProvisioningHistory based on its unique identifier.
    /// </summary>
    public class GetTellerOpenningAndClossingQueryHandler : IRequestHandler<GetTellerOpenningAndClossingQuery, ServiceResponse<List<OpenningAnclClossingTillDto>>>
    {
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvioningHistoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTellerOpenningAndClossingQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public GetTellerOpenningAndClossingQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetTellerOpenningAndClossingQueryHandler> logger,
            ISubTellerProvisioningHistoryRepository subTellerProvioningHistoryRepository,
            IAccountRepository accountRepository)
        {
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _subTellerProvioningHistoryRepository = subTellerProvioningHistoryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _accountRepository = accountRepository;
        }

        public async Task<ServiceResponse<List<OpenningAnclClossingTillDto>>> Handle(GetTellerOpenningAndClossingQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                // Convert date strings to DateTime objects
                var dateFrom = BaseUtilities.ConvertStringToDate(request.DateFrom);
                var dateTo = BaseUtilities.ConvertStringToDate(request.DateTo);

                // Validate date range
                BaseUtilities.ValidateDates(dateFrom, dateTo);

                // Query for primary teller data, with or without branch filtering
                var primaryTellerQuery = _primaryTellerProvisioningHistoryRepository
                    .FindBy(x => x.OpenedDate >= dateFrom && x.OpenedDate <= dateTo);

                if (request.ByBracnch)
                {
                    primaryTellerQuery = primaryTellerQuery.Where(x => x.BranchId == request.BranchId);
                }

                var primaryTellerData = await primaryTellerQuery.Include(x => x.Teller).ToListAsync();

                // Query for sub teller data, with or without branch filtering
                var subTellerQuery = _subTellerProvioningHistoryRepository
                    .FindBy(x => x.OpenedDate >= dateFrom && x.OpenedDate <= dateTo);

                if (request.ByBracnch)
                {
                    subTellerQuery = subTellerQuery.Where(x => x.BranchId == request.BranchId);
                }

                var subTellerData = await subTellerQuery.Include(x => x.Teller).ToListAsync();

                // Fetch all required account balances in one call
                var tellerIds = primaryTellerData.Select(p => p.TellerId).Distinct().ToList();
                tellerIds.AddRange(subTellerData.Select(s => s.TellerId).Distinct());
                tellerIds = tellerIds.Distinct().ToList();

                var accountBalances = await _accountRepository
                    .FindBy(x => tellerIds.Contains(x.TellerId))
                    .ToDictionaryAsync(x => x.TellerId, x => x.Balance);

                // Create primary teller DTO list
                var primaryTellerDtos = primaryTellerData.Select(p => new OpenningAnclClossingTillDto
                {
                    UserIdInChargeOfThisTeller = p.UserIdInChargeOfThisTeller,
                    ProvisionedBy = p.ProvisionedBy,
                    IsCashReplenished = false, // Default for primary teller
                    ReplenishedAmount = p.CashReplenishmentAmount, // Default for primary teller
                    OpenedDate = p.OpenedDate ?? DateTime.MinValue,
                    ClossedDate = p.ClossedDate ?? new DateTime(1900, 1, 1),
                    OpenOfDayAmount = p.OpenOfDayAmount,
                    AmountReplenished = p.CashReplenishmentAmount, // Default for primary teller
                    IsRequestedForCashReplenishment = false, // Default for primary teller
                    CashAtHand = p.CashAtHand,
                    EndOfDayAmount = p.EndOfDayAmount,
                    AccountBalance = accountBalances.TryGetValue(p.TellerId, out var balance) ? balance : 0, // Fetch account balance
                    LastOPerationAmount = p.LastUserID != null ? p.PreviouseBalance : 0,
                    LastOperationType = p.Teller?.OperationType,
                    PreviouseBalance = p.PreviouseBalance,
                    TellerComment = p.PrimaryTellerComment,
                    TellerType = p.Teller.TellerType,
                    IsPrimary = p.Teller.IsPrimary ? "Primary" : "Sub-Teller",
                    OpeningReference = p.ReferenceId,
                    ClossingReference = p.CloseOfDayReferenceId,
                    BranchCode = p.BranchId,
                    ClossedStatus = p.ClossedStatus,
                    PrimaryTellerComment = p.PrimaryTellerComment,
                    PrimaryTellerConfirmationStatus = null, // Default for primary teller
                    TellerName = p.Teller?.Name ?? "Unknown"
                }).ToList();

                // Create sub teller DTO list
                var subTellerDtos = subTellerData.Select(s => new OpenningAnclClossingTillDto
                {
                    UserIdInChargeOfThisTeller = s.UserIdInChargeOfThisTeller,
                    ProvisionedBy = s.ProvisionedBy,
                    IsCashReplenished = s.IsCashReplenished,
                    ReplenishedAmount = s.ReplenishedAmount,
                    OpenedDate = s.OpenedDate ?? DateTime.MinValue,
                    ClossedDate = s.ClossedDate ?? new DateTime(1900, 1, 1),
                    OpenOfDayAmount = s.OpenOfDayAmount, // Default for sub teller
                    AmountReplenished = s.ReplenishedAmount,
                    IsRequestedForCashReplenishment = s.IsRequestedForCashReplenishment,
                    CashAtHand = s.CashAtHand, // Default for sub teller
                    EndOfDayAmount = s.EndOfDayAmount,
                    AccountBalance = accountBalances.TryGetValue(s.TellerId, out var balance) ? balance : 0, // Fetch account balance
                    LastOPerationAmount = s.LastOPerationAmount, // Default for sub teller
                    LastOperationType = s.Teller?.OperationType,
                    PreviouseBalance = s.PreviouseBalance, // Default for sub teller
                    TellerComment = s.SubTellerComment,
                    TellerType = s.Teller.TellerType,
                    IsPrimary = s.Teller.IsPrimary ? "Primary" : "Sub-Teller",
                    OpeningReference = s.ReferenceId,
                    ClossingReference = s.CloseOfReferenceId,
                    BranchCode = s.BranchId,
                    ClossedStatus = s.ClossedStatus,
                    PrimaryTellerComment = s.PrimaryTellerComment,
                    PrimaryTellerConfirmationStatus = s.PrimaryTellerConfirmationStatus,
                    TellerName = s.Teller?.Name ?? "Unknown"
                }).ToList();

                // Combine the lists
                var jointData = primaryTellerDtos.Concat(subTellerDtos).ToList();

                if (jointData.Any())
                {
                    // Log successful data retrieval
                    errorMessage = "Open and closing of day data was retrieved successfully.";
                    await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return ServiceResponse<List<OpenningAnclClossingTillDto>>.ReturnResultWith200(jointData);
                }
                else
                {
                    // Log and return 404 if no data found
                    _logger.LogError("No data found for the provided criteria.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "No data found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<List<OpenningAnclClossingTillDto>>.Return404();
                }
            }
            catch (Exception e)
            {
                // Log and return 500 in case of error
                errorMessage = $"Error occurred while retrieving data: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<OpenningAnclClossingTillDto>>.Return500(e);
            }
        }
    }

}
