using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.DailyStatisticBoard;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.DailyStatisticBoard
{

    public class GeneralDailyDashboardRepository : GenericRepository<GeneralDailyDashboard, TransactionContext>, IGeneralDailyDashboardRepository
    {
        private readonly ILogger<TellerRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountingDayRepository _accountingDayRepository;

        public GeneralDailyDashboardRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<TellerRepository> logger, UserInfoToken userInfoToken, IAccountingDayRepository accountingDayRepository) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
            _accountingDayRepository = accountingDayRepository;
        }
        public async Task UpdateOrCreateDashboardAsync(CashOperation cashOperation, string reference)
        {
            try
            {
                // Step 1: Get today's date based on Douala's timezone.
                var today = BaseUtilities.UtcNowToDoualaTime();

                // Step 2: Try to find an existing dashboard entry for the branch and today's date.
                var existingDashboard = await FindBy(d => d.BranchId == cashOperation.BranchId && d.Date.Date == today.Date).AsNoTracking().FirstOrDefaultAsync();

                // Step 3: Get the current accounting day for the branch.
                var accountingDay = _accountingDayRepository.GetCurrentAccountingDay(cashOperation.BranchId);

                // Step 4: If no dashboard entry exists for the current day, create a new dashboard entry.
                if (existingDashboard == null)
                {
                    existingDashboard = new GeneralDailyDashboard
                    {
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        BranchId = cashOperation.BranchId,
                        Date = today,
                        NumberOfCashIn = 0,
                        NumberOfCashOut = 0,
                        TotalCashInAmount = 0,
                        TotalCashOutAmount = 0,
                        NewMembers = 0,
                        ClosedAccounts = 0,
                        ActiveAccounts = 0,
                        DormantAccounts = 0,
                        LoanDisbursements = 0,
                        LoanRepayments = 0,
                        ServiceFeesCollected = 0,
                        DailyExpenses = 0,
                        OrdinaryShares = 0,
                        PreferenceShares = 0,
                        Savings = 0,
                        Deposits = 0,
                        CashInHand57 = 0,
                        CashInHand56 = 0,
                        MTNMobileMoney = 0,
                        OrangeMoney = 0,
                        DailyCollectionCashOut = 0,
                        DailyCollectionCashIn = 0,
                        MomocashCollection = 0,
                        Transfer = 0,
                        PrimaryTillBalance = 0,
                        SubTillTillOpenOfDayBalance = 0,
                        PrimaryTillOpenOfDayBalance = 0,
                        AccountingDate = accountingDay,
                        BranchCode = cashOperation.BranchCode,
                        BranchName = cashOperation.BranchName,
                        SubTillBalance = 0
                    };

                    // Add the new dashboard entry based on the operation type
                    UpdateDashboardEntry(existingDashboard, cashOperation,reference);
                    Add(existingDashboard);

                    string successMessage = $"Successfully created dashboard entry for branch: {cashOperation.BranchName}. " +
                                            $"Operation type: {cashOperation.OperationType}, Amount: {cashOperation.Amount}";
                    _logger.LogInformation(successMessage);
                    cashOperation.SubTellerProvioningHistory.Teller = null;
                    await BaseUtilities.LogAndAuditAsync(successMessage, cashOperation, HttpStatusCodeEnum.OK, cashOperation.LogAction, LogLevelInfo.Information, reference);
                }
                else
                {
                    // Step 5: Update the existing dashboard entry based on the operation type.
                    UpdateDashboardEntry(existingDashboard, cashOperation,reference);
                    Update(existingDashboard);

                    string successMessage = $"Successfully updated dashboard entry for branch: {cashOperation.BranchName}. " +
                                            $"Operation type: {cashOperation.OperationType}, Amount: {cashOperation.Amount}";
                    _logger.LogInformation(successMessage);
                    cashOperation.SubTellerProvioningHistory.Teller = null;
                    await BaseUtilities.LogAndAuditAsync(successMessage, cashOperation, HttpStatusCodeEnum.OK, cashOperation.LogAction, LogLevelInfo.Information, reference);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Error updating/creating dashboard: {ex.Message}";
                _logger.LogError(errorMessage);
                cashOperation.SubTellerProvioningHistory.Teller = null;
                await BaseUtilities.LogAndAuditAsync(errorMessage, cashOperation, HttpStatusCodeEnum.InternalServerError, cashOperation.LogAction, LogLevelInfo.Error, reference);
                throw;
            }
        }

        private void UpdateDashboardEntry(GeneralDailyDashboard dashboard, CashOperation cashOperation,string reference)
        {
            cashOperation.Amount = Math.Abs(cashOperation.Amount);
            switch (cashOperation.OperationType)
            {
                case CashOperationType.OpenOfDayPrimaryTill:
                    dashboard.PrimaryTillBalance -= cashOperation.Amount;
                    dashboard.PrimaryTillOpenOfDayBalance += cashOperation.Amount;
                    break;

                case CashOperationType.OpenOfDaySubTill:
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.SubTillTillOpenOfDayBalance += cashOperation.Amount;
                    break;
                case CashOperationType.NewMember:
                    dashboard.NewMembers++;
                    break;

                case CashOperationType.CashIn:
                    if (cashOperation.IsInterBranch)
                    {
                        dashboard.NumberOfInterBranchCashIn++;
                        dashboard.VolumeOfInterBranchCashIn += cashOperation.Amount;
                    }
                    dashboard.NumberOfCashIn++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;
                case CashOperationType.RemittanceIn:
                   
                    dashboard.NumberOfCashIn++;
                    dashboard.NumberOfRemiitanceInitiated++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.VolumeOfRemiitanceInitiated += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;
                case CashOperationType.OtherCashIn:
                    dashboard.NumberOfCashIn++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;
                case CashOperationType.OtherCashOut:
                    decimal amount = (cashOperation.Amount - cashOperation.Fee);
                    dashboard.NumberOfCashOut++;
                    dashboard.TotalCashOutAmount += amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;


                case CashOperationType.CashOut:
                    decimal amount2 = (cashOperation.Amount - cashOperation.Fee);
                    if (cashOperation.IsInterBranch)
                    {
                        dashboard.NumberOfInterBranchCashOut++;
                        dashboard.VolumeOfInterBranchCashOut += amount2;
                    }
                    dashboard.NumberOfCashOut++;
                    dashboard.TotalCashOutAmount += amount2;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;

                case CashOperationType.RemittanceOut:
                    decimal amount3 = (cashOperation.Amount - cashOperation.Fee);
                    if (cashOperation.IsInterBranch)
                    {
                        dashboard.NumberOfInterBranchCashOut++;
                        dashboard.VolumeOfInterBranchCashOut += amount3;
                    }
                    dashboard.NumberOfRemiitanceReception++;
                    dashboard.VolumeOfRemiitanceReception += amount3;
                    dashboard.NumberOfCashOut++;
                    dashboard.TotalCashOutAmount += amount3;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;

                case CashOperationType.LoanRepayment:
                    dashboard.LoanRepayments += cashOperation.Amount;
                    if (cashOperation.IsInterBranch)
                    {
                        dashboard.NumberOfInterBranchCashIn++;
                        dashboard.VolumeOfInterBranchCashIn += cashOperation.Amount;
                    }
                    dashboard.NumberOfCashIn++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;

                case CashOperationType.MobileMoneyCashIn:
                    dashboard.MTNMobileMoney -= cashOperation.Amount;
                    dashboard.NumberOfCashIn++;
                    dashboard.NumberOfCashInMTN++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;

                case CashOperationType.MobileMoneyCashOut:
                    dashboard.MobileMoneyCashOut += cashOperation.Amount;
                    dashboard.NumberOfCashOut++;
                    dashboard.NumberOfCashOutMTN++;
                    dashboard.TotalCashOutAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    break;

                case CashOperationType.OrangeMoneyCashIn:
                    dashboard.OrangeMoney -= cashOperation.Amount;
                    dashboard.NumberOfCashIn++;
                    dashboard.NumberOfCashInOrange++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    break;

                case CashOperationType.OrangeMoneyCashOut:
                    dashboard.OrangeMoneyCashOut += cashOperation.Amount;
                    dashboard.NumberOfCashOut++;
                    dashboard.NumberOfCashOutOrange++;
                    dashboard.TotalCashOutAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    break;

                case CashOperationType.LoanDisbursementFee:
                    dashboard.NumberOfCashIn++;
                    dashboard.NumberOfLoanDisbursementFee++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.ServiceFeesCollected += cashOperation.Fee;
                    dashboard.LoanDisbursements += cashOperation.Fee;
                    break;
                case CashOperationType.LoanFee:
                    dashboard.ServiceFeesCollected += cashOperation.Amount;
                    dashboard.NumberOfCashIn++;
                    dashboard.NumberOfLoanFee++;
                    dashboard.TotalCashInAmount += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    break;
                case CashOperationType.CashReplenishmentSubTill:
                    dashboard.CashReplenishmentSubTill += cashOperation.Amount;
                    dashboard.SubTillBalance = cashOperation.SubTellerProvioningHistory.CashAtHand;
                    dashboard.PrimaryTillBalance -= cashOperation.Amount;
                    break;
                case CashOperationType.CashReplenishmentPrimaryTill:
                    dashboard.CashReplenishmentPrimaryTill += cashOperation.Amount;
                    dashboard.PrimaryTillBalance += cashOperation.Amount;
                    break;

                case CashOperationType.CloseOfDayPrimaryTill:
                    dashboard.CashInHand57 += dashboard.SubTillBalance;
                    dashboard.SubTillBalance = 0;
                    break;

                default:
                    string errorMessage = $"Unknown cash operation type encountered: {cashOperation.OperationType}. Please check the cash operation type provided.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task<GeneralDailyDashboard> GetDashboardDataForBranchByDateAsync(string branchId, DateTime date)
        {
            var param = new Param { BranchId = branchId, Date = date };
            try
            {
                // Query to fetch the dashboard entry for a specific branch and date.
                return await FindBy(d => d.BranchId == branchId && d.Date.Date == date.Date).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // Log generic errors with branch and date information
                string errorMessage = $"An unexpected error occurred while retrieving dashboard data for branch: {branchId} on date: {date}. Exception: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the error
                await BaseUtilities.LogAndAuditAsync(errorMessage, param, HttpStatusCodeEnum.InternalServerError, LogAction.GeneralDailyDashboard, LogLevelInfo.Error, null);

                throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task<List<GeneralDailyDashboard>> GetDashboardDataForAllBranchesByDateAsync(DateTime date)
        {
            var param = new Param { BranchId = "N/A", Date = date };

            try
            {
                // Query to fetch all dashboard entries for the specified date.
                return await FindBy(d => d.Date.Date == date.Date).ToListAsync();
            }
            catch (Exception ex)
            {
                // Log generic errors with date information
                string errorMessage = $"An unexpected error occurred while retrieving dashboard data for all branches on date: {date}. Exception: {ex.Message}";
                _logger.LogError(errorMessage);

                // Log and audit the error
                await BaseUtilities.LogAndAuditAsync(errorMessage, param, HttpStatusCodeEnum.InternalServerError, LogAction.GeneralDailyDashboard, LogLevelInfo.Error,null);

                throw new InvalidOperationException(errorMessage);
            }
        }

    }
}
public class Param
{
    public DateTime Date { get; set; }
    public string BranchId { get; set; }
}