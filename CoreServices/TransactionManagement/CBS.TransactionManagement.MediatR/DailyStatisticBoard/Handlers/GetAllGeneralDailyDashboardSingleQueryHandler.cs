using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.Repository.DailyStatisticBoard;
using CBS.TransactionManagement.DailyStatisticBoard.Queries;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using CBS.TransactionManagement.Data.Entity.DailyStatisticBoard;

namespace CBS.TransactionManagement.DailyStatisticBoard.Handlers
{
    /// <summary>
    /// Handles the retrieval of all GeneralDailyDashboard data based on the GetAllGeneralDailyDashboardSingleQuery.
    /// This handler fetches the data from the repository within the specified date range, calculates summarized values, 
    /// and returns the accumulated results as a single GeneralDailyDashboardDto object.
    /// </summary>
    public class GetAllGeneralDailyDashboardSingleQueryHandler : IRequestHandler<GetAllGeneralDailyDashboardSingleQuery, ServiceResponse<GeneralDailyDashboardDto>>
    {
        private readonly IGeneralDailyDashboardRepository _GeneralDailyDashboardRepository; // Repository for accessing GeneralDailyDashboard data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllGeneralDailyDashboardSingleQueryHandler> _logger; // Logger for logging actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllGeneralDailyDashboardSingleQueryHandler.
        /// </summary>
        /// <param name="GeneralDailyDashboardRepository">Repository for GeneralDailyDashboard data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        public GetAllGeneralDailyDashboardSingleQueryHandler(
            IGeneralDailyDashboardRepository GeneralDailyDashboardRepository,
            IMapper mapper,
            ILogger<GetAllGeneralDailyDashboardSingleQueryHandler> logger)
        {
            _GeneralDailyDashboardRepository = GeneralDailyDashboardRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllGeneralDailyDashboardSingleQuery to retrieve all GeneralDailyDashboard entries within the specified date range.
        /// </summary>
        /// <param name="request">The GetAllGeneralDailyDashboardSingleQuery containing date range parameters.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        /// <returns>Returns a ServiceResponse with the accumulated GeneralDailyDashboardDto object.</returns>
        public async Task<ServiceResponse<GeneralDailyDashboardDto>> Handle(GetAllGeneralDailyDashboardSingleQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Attempt to retrieve all GeneralDailyDashboard entries within the specified date range.
                var entities = await _GeneralDailyDashboardRepository.All
                    .Where(x => x.Date.Date >= request.DateFrom.Date && x.Date.Date <= request.DateTo.Date)
                    .ToListAsync();

                // Check if any data was found for the given date range.
                if (entities == null || !entities.Any())
                {
                    // Log and handle the scenario where no data is found.
                    var notFoundMessage = $"No GeneralDailyDashboard data was found for all branches within the specified date range. [{request.DateFrom.Date} - {request.DateTo.Date}]";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GeneralDailyDashboard, LogLevelInfo.Warning);

                    // Return a 404 Not Found response with the warning message.
                    return ServiceResponse<GeneralDailyDashboardDto>.Return404(notFoundMessage);
                }

                // Calculate and map the summarized data to a GeneralDailyDashboardDto object.
                var dailyDashboardDto = CalculateSummary(entities);

                // Return a successful response with the summarized DTO.
                return ServiceResponse<GeneralDailyDashboardDto>.ReturnResultWith200(dailyDashboardDto);
            }
            catch (Exception e)
            {
                // Handle exceptions, log the error, and return a 500 Internal Server Error response.
                var errorMessage = $"An error occurred while retrieving daily dashboard data for the specified date range. [{request.DateFrom.Date} - {request.DateTo.Date}]";
                _logger.LogError(errorMessage, e);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GeneralDailyDashboard, LogLevelInfo.Error);

                return ServiceResponse<GeneralDailyDashboardDto>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Summarizes the provided list of GeneralDailyDashboard entries by accumulating all numeric values.
        /// </summary>
        /// <param name="dashboardData">Collection of GeneralDailyDashboard entries to summarize.</param>
        /// <returns>Returns a single GeneralDailyDashboardDto containing the summarized data.</returns>
        public GeneralDailyDashboardDto CalculateSummary(IEnumerable<GeneralDailyDashboard> dashboardData)
        {
            return new GeneralDailyDashboardDto
            {
                NumberOfCashIn = dashboardData.Sum(d => d.NumberOfCashIn),
                NumberOfCashOut = dashboardData.Sum(d => d.NumberOfCashOut),
                TotalCashInAmount = dashboardData.Sum(d => d.TotalCashInAmount),
                TotalCashOutAmount = dashboardData.Sum(d => d.TotalCashOutAmount),
                NewMembers = dashboardData.Sum(d => d.NewMembers),
                ClosedAccounts = dashboardData.Sum(d => d.ClosedAccounts),
                ActiveAccounts = dashboardData.Sum(d => d.ActiveAccounts),
                DormantAccounts = dashboardData.Sum(d => d.DormantAccounts),
                NumberOfInterBranchCashIn = dashboardData.Sum(d => d.NumberOfInterBranchCashIn),
                NumberOfInterBranchCashOut = dashboardData.Sum(d => d.NumberOfInterBranchCashOut),
                VolumeOfInterBranchCashIn = dashboardData.Sum(d => d.VolumeOfInterBranchCashIn),
                VolumeOfInterBranchCashOut = dashboardData.Sum(d => d.VolumeOfInterBranchCashOut),
                LoanDisbursements = dashboardData.Sum(d => d.LoanDisbursements),
                LoanRepayments = dashboardData.Sum(d => d.LoanRepayments),
                ServiceFeesCollected = dashboardData.Sum(d => d.ServiceFeesCollected),
                InterestPaid = dashboardData.Sum(d => d.InterestPaid),
                Vat = dashboardData.Sum(d => d.Vat),
                Penalties = dashboardData.Sum(d => d.Penalties),
                DailyExpenses = dashboardData.Sum(d => d.DailyExpenses),
                OrdinaryShares = dashboardData.Sum(d => d.OrdinaryShares),
                PreferenceShares = dashboardData.Sum(d => d.PreferenceShares),
                Savings = dashboardData.Sum(d => d.Savings),
                Deposits = dashboardData.Sum(d => d.Deposits),
                CashInHand57 = dashboardData.Sum(d => d.CashInHand57),
                CashInHand56 = dashboardData.Sum(d => d.CashInHand56),
                MTNMobileMoney = dashboardData.Sum(d => d.MTNMobileMoney),
                MobileMoneyCashOut = dashboardData.Sum(d => d.MobileMoneyCashOut),
                NumberOfCashOutMTN = dashboardData.Sum(d => d.NumberOfCashOutMTN),
                NumberOfCashOutOrange = dashboardData.Sum(d => d.NumberOfCashOutOrange),
                NumberOfCashInMTN = dashboardData.Sum(d => d.NumberOfCashInMTN),
                NumberOfLoanFee = dashboardData.Sum(d => d.NumberOfLoanFee),
                NumberOfLoanDisbursementFee = dashboardData.Sum(d => d.NumberOfLoanDisbursementFee),
                NumberOfCashInOrange = dashboardData.Sum(d => d.NumberOfCashInOrange),
                OrangeMoneyCashOut = dashboardData.Sum(d => d.OrangeMoneyCashOut),
                OrangeMoney = dashboardData.Sum(d => d.OrangeMoney),
                DailyCollectionCashOut = dashboardData.Sum(d => d.DailyCollectionCashOut),
                DailyCollectionCashIn = dashboardData.Sum(d => d.DailyCollectionCashIn),
                NumberOfDailyCollectionCashOut = dashboardData.Sum(d => d.NumberOfDailyCollectionCashOut),
                NumberOfDailyCollectionCashIn = dashboardData.Sum(d => d.NumberOfDailyCollectionCashIn),
                MomocashCollection = dashboardData.Sum(d => d.MomocashCollection),
                Transfer = dashboardData.Sum(d => d.Transfer),
                NumberOfTransfer = dashboardData.Sum(d => d.NumberOfTransfer),
                PrimaryTillOpenOfDayBalance = dashboardData.Sum(d => d.PrimaryTillOpenOfDayBalance),
                SubTillTillOpenOfDayBalance = dashboardData.Sum(d => d.SubTillTillOpenOfDayBalance),
                SubTillBalance = dashboardData.Sum(d => d.SubTillBalance),
                PrimaryTillBalance = dashboardData.Sum(d => d.PrimaryTillBalance),
                CashReplenishmentSubTill = dashboardData.Sum(d => d.CashReplenishmentSubTill),
                CashReplenishmentPrimaryTill = dashboardData.Sum(d => d.CashReplenishmentPrimaryTill),
                NumberOfBranches = dashboardData.Select(d => d.BranchId).Distinct().Count(),
                Date = dashboardData.Max(d => d.Date),
                AccountingDate = dashboardData.Max(d => d.AccountingDate)
            };
        }
    }
}
