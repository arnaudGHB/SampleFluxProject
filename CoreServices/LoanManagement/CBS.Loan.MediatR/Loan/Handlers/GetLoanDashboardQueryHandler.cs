using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve loan dashboard data based on specific branch and query parameters.
    /// </summary>
    public class GetLoanDashboardQueryHandler : IRequestHandler<GetLoanDashboardQuery, ServiceResponse<LoanMainDashboardDto>>
    {
        private readonly ILoanRepository _loanRepository; // Repository for accessing Loan data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanDashboardQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanDashboardQueryHandler.
        /// </summary>
        /// <param name="loanRepository">Repository for Loan data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanDashboardQueryHandler(
            ILoanRepository loanRepository,
            IMapper mapper,
            ILogger<GetLoanDashboardQueryHandler> logger)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanDashboardQuery to retrieve loan dashboard information.
        /// </summary>
        /// <param name="request">The GetLoanDashboardQuery containing branch and query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanMainDashboardDto>> Handle(GetLoanDashboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Determine if we need to filter by branch
                IQueryable<Loan> loanQuery = _loanRepository.All.Where(x => !x.IsDeleted);

                if (request.QueryParameter.Equals("bybranch", StringComparison.OrdinalIgnoreCase))
                {
                    loanQuery = loanQuery.Where(x => x.BranchId == request.BranchId);
                }

                // Calculate required loan metrics directly in the query
                var loanDashboardData = await loanQuery
                    .GroupBy(_ => 1) // Group by a constant to aggregate without fetching individual loan data
                    .Select(g => new
                    {
                        TotalNumberOfLoans = g.Count(),
                        TotalVolumeOfLoanGranted = g.Sum(l => l.LoanAmount),
                        TotalRemainingBalance = g.Sum(l => l.Principal)
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (loanDashboardData == null)
                {
                    _logger.LogError("No loans found for the specified criteria.");
                    return ServiceResponse<LoanMainDashboardDto>.Return404();
                }

                // Map the results to the dashboard model
                var loanDashboard = new LoanMainDashboardDto
                {
                    TotalNumberOfLoans = loanDashboardData.TotalNumberOfLoans,
                    TotalVolumeOfLoanGranted = loanDashboardData.TotalVolumeOfLoanGranted,
                    TotalRemainingBalance = loanDashboardData.TotalRemainingBalance
                };

                // Return the calculated dashboard data with a success response
                return ServiceResponse<LoanMainDashboardDto>.ReturnResultWith200(loanDashboard);
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while retrieving loan dashboard data: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanMainDashboardDto>.Return500(e);
            }
        }
    }

}
