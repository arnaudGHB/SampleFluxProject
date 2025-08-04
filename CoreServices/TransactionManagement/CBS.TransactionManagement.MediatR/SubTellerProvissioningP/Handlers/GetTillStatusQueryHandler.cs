using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Queries;
using System.Linq;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.MediatR.SubTellerProvissioningP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific PrimaryTellerProvisioningHistory based on its unique identifier.
    /// </summary>
    public class GetTillStatusQueryHandler : IRequestHandler<GetTillStatusQuery, ServiceResponse<List<TillOpenAndCloseOfDayDto>>>
    {
        // Repositories and dependencies
        private readonly IPrimaryTellerProvisioningHistoryRepository _primaryTellerProvisioningHistoryRepository;
        private readonly ISubTellerProvisioningHistoryRepository _subTellerProvisioningHistoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetTillStatusQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public GetTillStatusQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository primaryTellerProvisioningHistoryRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetTillStatusQueryHandler> logger,
            ISubTellerProvisioningHistoryRepository subTellerProvisioningHistoryRepository,
            IAccountRepository accountRepository)
        {
            // Constructor injection of dependencies
            _primaryTellerProvisioningHistoryRepository = primaryTellerProvisioningHistoryRepository;
            _subTellerProvisioningHistoryRepository = subTellerProvisioningHistoryRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _accountRepository = accountRepository;
        }

        public async Task<ServiceResponse<List<TillOpenAndCloseOfDayDto>>> Handle(GetTillStatusQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;

            try
            {
                // Convert date strings to DateTime objects
                var dateFrom = request.DateFrom;
                var dateTo = request.DateTo;

                // Validate the date range
                BaseUtilities.ValidateDates(dateFrom, dateTo);

                // Fetch primary and sub teller data based on date range and optional branch filtering
                var primaryTellerData = await FetchTellerData(_primaryTellerProvisioningHistoryRepository, dateFrom, dateTo, request.ByBranch, request.QueryParameter, request.ByTeller);

                var subTellerData = await FetchTellerData(_subTellerProvisioningHistoryRepository, dateFrom, dateTo, request.ByBranch, request.QueryParameter, request.ByTeller);

                // Get distinct teller IDs for fetching account balances
                var tellerIds = GetDistinctTellerIds(primaryTellerData, subTellerData);

                // Fetch all required account balances in one call
                var accountBalances = await FetchAccountBalances(tellerIds);

                // Map DTOs for primary and sub tellers
                var primaryTellerDtos = MapTellerData(primaryTellerData, accountBalances, isPrimary: true);
                var subTellerDtos = MapTellerData(subTellerData, accountBalances, isPrimary: false);

                // Combine the lists
                var jointData = primaryTellerDtos.Concat(subTellerDtos).ToList();

                if (jointData.Any())
                {
                    // Log successful data retrieval
                    errorMessage = "Open and closing of day data was retrieved successfully.";
                    await AuditLog(request, LogAction.Read, errorMessage, 200);
                    return ServiceResponse<List<TillOpenAndCloseOfDayDto>>.ReturnResultWith200(jointData, errorMessage);
                }
                else
                {
                    // Log and return 404 if no data found
                    errorMessage = "No data found for the provided criteria.";
                    _logger.LogError(errorMessage);
                    await AuditLog(request, LogAction.Read, errorMessage, 404);
                    return ServiceResponse<List<TillOpenAndCloseOfDayDto>>.Return404(errorMessage);
                }
            }
            catch (Exception ex)
            {
                // Log and return 500 in case of error
                errorMessage = $"Error occurred while retrieving data: {ex.Message}";
                _logger.LogError(errorMessage);
                await AuditLog(request, LogAction.Read, errorMessage, 500);
                return ServiceResponse<List<TillOpenAndCloseOfDayDto>>.Return500(errorMessage);
            }
        }

        // Method to fetch teller data with optional branch filtering
        private async Task<List<PrimaryTellerProvisioningHistory>> FetchTellerData(IPrimaryTellerProvisioningHistoryRepository repository, DateTime dateFrom, DateTime dateTo, bool byBranch, string queryParameter, bool byTeller)
        {
            var query = repository.FindBy(x => x.OpenedDate.Value.Date >= dateFrom.Date && x.OpenedDate.Value.Date <= dateTo.Date);

            if (byBranch)
            {
                query = query.Where(x => x.BranchId == queryParameter);
            }
            else if (byTeller)
            {
                query = query.Where(x => x.TellerId == queryParameter);

            }

            return await query.Include(x => x.Teller).ToListAsync();
        }

        // Method to fetch teller data with optional branch filtering
        private async Task<List<SubTellerProvioningHistory>> FetchTellerData(ISubTellerProvisioningHistoryRepository repository, DateTime dateFrom, DateTime dateTo, bool byBranch, string queryParameter, bool byTeller)
        {
            var query = repository.FindBy(x => x.OpenedDate.Value.Date >= dateFrom.Date && x.OpenedDate.Value.Date <= dateTo.Date);

            if (byBranch)
            {
                query = query.Where(x => x.BranchId == queryParameter);
            }
            else if (byTeller)
            {
                query = query.Where(x => x.TellerId == queryParameter);

            }
            return await query.Include(x => x.Teller).ToListAsync();
        }

        // Method to extract distinct teller IDs from primary and sub teller data
        private List<string> GetDistinctTellerIds(List<PrimaryTellerProvisioningHistory> primaryTellerData, List<SubTellerProvioningHistory> subTellerData)
        {
            var tellerIds = primaryTellerData.Select(p => p.TellerId).Distinct().ToList();
            tellerIds.AddRange(subTellerData.Select(s => s.TellerId).Distinct());
            return tellerIds.Distinct().ToList();
        }

        // Method to fetch account balances for tellers
        private async Task<Dictionary<string, decimal>> FetchAccountBalances(List<string> tellerIds)
        {
            return await _accountRepository.FindBy(x => tellerIds.Contains(x.TellerId)).ToDictionaryAsync(x => x.TellerId, x => x.Balance);
        }

        // Method to map teller data to DTOs
        private List<TillOpenAndCloseOfDayDto> MapTellerData(List<PrimaryTellerProvisioningHistory> tellerData, Dictionary<string, decimal> accountBalances, bool isPrimary)
        {
            return tellerData.Select(t => new TillOpenAndCloseOfDayDto
            {
                UserIdInChargeOfThisTeller = t.UserIdInChargeOfThisTeller,
                ProvisionedBy = t.ProvisionedBy,
                IsCashReplenished = isPrimary ? false : t.IsCashReplenishment,
                ReplenishedAmount = isPrimary ? t.CashReplenishmentAmount : t.CashReplenishmentAmount,
                OpenedDate = t.OpenedDate ?? DateTime.MinValue,
                ClossedDate = t.ClossedDate ?? new DateTime(1900, 1, 1),
                OpenOfDayAmount = t.OpenOfDayAmount,
                IsRequestedForCashReplenishment = isPrimary ? false : t.IsCashReplenishment,
                CashAtHand = t.CashAtHand,
                EndOfDayAmount = t.EndOfDayAmount,
                AccountBalance = accountBalances.TryGetValue(t.TellerId, out var balance) ? balance : 0,
                LastOPerationAmount = t.LastUserID != null ? t.PreviouseBalance : 0,
                LastOperationType = t.Teller?.OperationType,
                PreviouseBalance = t.PreviouseBalance,
                TellerComment = isPrimary ? t.PrimaryTellerComment : t.PrimaryTellerComment,
                TellerType = t.Teller.TellerType,
                IsPrimary = isPrimary ? "Primary" : "Sub-Teller",
                ReferenceId = t.ReferenceId,
                CloseOfReferenceId = t.CloseOfDayReferenceId,
                BranchCode = t.BranchId,
                ClossedStatus = t.ClossedStatus, BranchId=t.BranchId,
                PrimaryTellerComment = t.PrimaryTellerComment,
                PrimaryTellerConfirmationStatus = isPrimary ? null : t.ClossedStatus,
                TellerName = t.Teller?.Name ?? "Unknown", Teller=t.Teller, BankId=t.BankId, DailyTellerId=t.DailyTellerId, TellerId=t.TellerId, SubTellerComment=t.PrimaryTellerComment, Id=t.Id, InitialOpening="N/A", Note=t.PrimaryTellerComment, LastUserID=t.LastUserID,

                // Map opening and closing notes and coins counts
                OpeningNote10000 = t.OpeningNote10000,
                OpeningNote5000 = t.OpeningNote5000,
                OpeningNote2000 = t.OpeningNote2000,
                OpeningNote1000 = t.OpeningNote1000,
                OpeningNote500 = t.OpeningNote500,
                OpeningCoin500 = t.OpeningCoin500,
                OpeningCoin100 = t.OpeningCoin100,
                OpeningCoin50 = t.OpeningCoin50,
                OpeningCoin25 = t.OpeningCoin25,
                OpeningCoin10 = t.OpeningCoin10,
                OpeningCoin5 = t.OpeningCoin5,
                OpeningCoin1 = t.OpeningCoin1,

                ClosingNote10000 = t.ClosingNote10000,
                ClosingNote5000 = t.ClosingNote5000,
                ClosingNote2000 = t.ClosingNote2000,
                ClosingNote1000 = t.ClosingNote1000,
                ClosingNote500 = t.ClosingNote500,
                ClosingCoin500 = t.ClosingCoin500,
                ClosingCoin100 = t.ClosingCoin100,
                ClosingCoin50 = t.ClosingCoin50,
                ClosingCoin25 = t.ClosingCoin25,
                ClosingCoin10 = t.ClosingCoin10,
                ClosingCoin5 = t.ClosingCoin5,
                ClosingCoin1 = t.ClosingCoin1
            }).ToList();
        }

        private List<TillOpenAndCloseOfDayDto> MapTellerData(List<SubTellerProvioningHistory> tellerData, Dictionary<string, decimal> accountBalances, bool isPrimary)
        {
            return tellerData.Select(t => new TillOpenAndCloseOfDayDto
            {
                UserIdInChargeOfThisTeller = t.UserIdInChargeOfThisTeller,
                ProvisionedBy = t.ProvisionedBy,
                IsCashReplenished = isPrimary ? false : t.IsCashReplenished,
                ReplenishedAmount = isPrimary ? t.ReplenishedAmount : t.ReplenishedAmount,
                OpenedDate = t.OpenedDate ?? DateTime.MinValue,
                ClossedDate = t.ClossedDate ?? new DateTime(1900, 1, 1),
                OpenOfDayAmount = t.OpenOfDayAmount,
                IsRequestedForCashReplenishment = isPrimary ? false : t.IsRequestedForCashReplenishment,
                CashAtHand = t.CashAtHand,
                EndOfDayAmount = t.EndOfDayAmount,
                AccountBalance = accountBalances.TryGetValue(t.TellerId, out var balance) ? balance : 0,
                LastOPerationAmount = t.LastUserID != null ? t.PreviouseBalance : 0,
                LastOperationType = t.Teller?.OperationType,
                PreviouseBalance = t.PreviouseBalance,
                TellerComment = isPrimary ? t.PrimaryTellerComment : t.SubTellerComment,
                TellerType = t.Teller.TellerType,
                IsPrimary = isPrimary ? "Primary" : "Sub-Teller",
                ReferenceId = t.ReferenceId,
                CloseOfReferenceId = t.CloseOfReferenceId,
                BranchCode = t.BranchId,
                ClossedStatus = t.ClossedStatus, BranchId=t.BranchId,
                PrimaryTellerComment = t.PrimaryTellerComment,
                PrimaryTellerConfirmationStatus = isPrimary ? null : t.PrimaryTellerConfirmationStatus,
                TellerName = t.Teller?.Name ?? "Unknown",
                Teller = t.Teller,
                BankId = t.BankId,
                DailyTellerId = t.DailyTellerId,
                TellerId = t.TellerId,
                SubTellerComment = t.SubTellerComment,
                Id = t.Id,
                InitialOpening = "N/A",
                Note = t.PrimaryTellerComment,
                LastUserID = t.LastUserID,

                // Map opening and closing notes and coins counts
                OpeningNote10000 = t.OpeningNote10000,
                OpeningNote5000 = t.OpeningNote5000,
                OpeningNote2000 = t.OpeningNote2000,
                OpeningNote1000 = t.OpeningNote1000,
                OpeningNote500 = t.OpeningNote500,
                OpeningCoin500 = t.OpeningCoin500,
                OpeningCoin100 = t.OpeningCoin100,
                OpeningCoin50 = t.OpeningCoin50,
                OpeningCoin25 = t.OpeningCoin25,
                OpeningCoin10 = t.OpeningCoin10,
                OpeningCoin5 = t.OpeningCoin5,
                OpeningCoin1 = t.OpeningCoin1,

                ClosingNote10000 = t.ClosingNote10000,
                ClosingNote5000 = t.ClosingNote5000,
                ClosingNote2000 = t.ClosingNote2000,
                ClosingNote1000 = t.ClosingNote1000,
                ClosingNote500 = t.ClosingNote500,
                ClosingCoin500 = t.ClosingCoin500,
                ClosingCoin100 = t.ClosingCoin100,
                ClosingCoin50 = t.ClosingCoin50,
                ClosingCoin25 = t.ClosingCoin25,
                ClosingCoin10 = t.ClosingCoin10,
                ClosingCoin5 = t.ClosingCoin5,
                ClosingCoin1 = t.ClosingCoin1
            }).ToList();
        }

        // Method to log actions and audit results
        private async Task AuditLog(GetTillStatusQuery request, LogAction action, string message, int statusCode)
        {
            await APICallHelper.AuditLogger(
                _userInfoToken.FullName,
                action.ToString(),
                request,
                message,
                LogLevelInfo.Information.ToString(),
                statusCode,
                _userInfoToken.Token
            );
        }
    }

}
