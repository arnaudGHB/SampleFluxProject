using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetLoansByMultipleCustomersQueryHandler : IRequestHandler<GetLoansByMultipleCustomersQuery, ServiceResponse<List<LoanDto>>>
    {
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoansByMultipleCustomersQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoansByMultipleCustomersQueryHandler(
            ILoanRepository LoanRepository,
            IMapper mapper, ILogger<GetLoansByMultipleCustomersQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }


        /// <summary>
        /// Handles the query to retrieve loans for multiple customers based on their CustomerIds
        /// and an optional QueryParameter for filtering by loan status or other attributes.
        /// If multiple open loans exist, it takes the oldest loan with DueAmount > 0.
        /// </summary>
        public async Task<ServiceResponse<List<LoanDto>>> Handle(GetLoansByMultipleCustomersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Validate input
                if (request.CustomerIds == null || !request.CustomerIds.Any())
                {
                    return ServiceResponse<List<LoanDto>>.Return422("CustomerIds list cannot be null or empty.");
                }

                // ✅ Chunk CustomerIds if too large (SQL Server supports max 2100 parameters)
                List<Loan> allLoans = new List<Loan>();
                int chunkSize = 500; // Adjust based on SQL parameter limits

                foreach (var chunk in request.CustomerIds.Chunk(chunkSize))
                {
                    var chunkLoans = await _LoanRepository
                        .FindBy(x => chunk.Contains(x.CustomerId) && x.DueAmount > 0).Include(x=>x.LoanApplication).ThenInclude(x=>x.LoanProduct)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    allLoans.AddRange(chunkLoans);
                }

                // ✅ Apply additional filters in-memory (EF Core struggles with `ToLower()`)
                if (!string.IsNullOrWhiteSpace(request.QueryParameter))
                {
                    string param = request.QueryParameter.ToLower(); // Convert once for efficiency

                    allLoans = param switch
                    {
                        "open" => allLoans.Where(x => x.LoanStatus == LoanStatus.Open.ToString()).ToList(),
                        "closed" => allLoans.Where(x => x.LoanStatus == LoanStatus.Closed.ToString()).ToList(),
                        "disbursed" => allLoans.Where(x => x.LoanStatus == LoanStatus.Disbursed.ToString()).ToList(),
                        "restructured" => allLoans.Where(x => x.LoanStatus == LoanStatus.Restructured.ToString()).ToList(),
                        "rescheduled" => allLoans.Where(x => x.LoanStatus == LoanStatus.Rescheduled.ToString()).ToList(),
                        "current" => allLoans.Where(x => x.IsCurrentLoan).ToList(),
                        "refinanced" => allLoans.Where(x => x.LoanStatus == LoanStatus.Refinancing.ToString()).ToList(),
                        "approved" => allLoans.Where(x => !x.IsLoanDisbursted).ToList(),
                        "all" => allLoans, // No additional filtering for 'all'
                        _ => throw new ArgumentException("Invalid LoanStatus provided in QueryParameter.")
                    };
                }

                // ✅ Group in-memory to get the oldest loan per customer
                var oldestLoans = allLoans
                    .GroupBy(x => x.CustomerId)
                    .Select(g => g.OrderBy(l => l.LoanDate).FirstOrDefault())
                    .ToList();

                // ✅ Map entities to DTOs and return the result
                var loanDtos = _mapper.Map<List<LoanDto>>(oldestLoans);
                return ServiceResponse<List<LoanDto>>.ReturnResultWith200(loanDtos);
            }
            catch (Exception e)
            {
                // ✅ Improved logging with full stack trace
                _logger.LogError(e, "Failed to get loans: {ErrorMessage}", e.Message);
                return ServiceResponse<List<LoanDto>>.Return500(e, "Failed to get loans");
            }
        }


    }
}
