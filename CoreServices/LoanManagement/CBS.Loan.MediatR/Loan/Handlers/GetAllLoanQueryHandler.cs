using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanueryHandler : IRequestHandler<GetAllLoanQuery, ServiceResponse<List<LoanDto>>>
    {
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Loans data.
        private readonly ILoanApplicationRepository _LoanApplicationRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanueryHandler(
            ILoanRepository LoanRepository,
            IMapper mapper, ILogger<GetAllLoanueryHandler> logger, ILoanApplicationRepository loanApplicationRepository = null)
        {
            // Assign provided dependencies to local variables.
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
            _LoanApplicationRepository = loanApplicationRepository;
        }

        public async Task<ServiceResponse<List<LoanDto>>> Handle(GetAllLoanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Start constructing the query to get all non-deleted loans
                var query = _LoanRepository.All.Where(x => !x.IsDeleted).AsNoTracking().AsQueryable();

                // Filter by BranchId if provided in the request
                if (request.IsByBranch && !string.IsNullOrEmpty(request.BranchId))
                {
                    query = query.Where(loan => loan.BranchId == request.BranchId);
                }

                // Apply additional filtering based on the QueryParam if provided
                if (!string.IsNullOrEmpty(request.QueryParam))
                {
                    // Convert the QueryParam to lowercase for case-insensitive comparison
                    string param = request.QueryParam.ToLower();
                    query = param switch
                    {
                        "open" => query.Where(loan => loan.LoanStatus.ToLower() == "open"),
                        "closed" => query.Where(loan => loan.LoanStatus.ToLower() == "closed"),
                        "disbursed" => query.Where(loan => loan.LoanStatus.ToLower() == "disbursed"),
                        "restructured" => query.Where(loan => loan.LoanStatus.ToLower() == "restructured"),
                        "pending" => query.Where(loan => loan.LoanStatus.ToLower() == "pending"),
                        _ => query
                    };
                }

                // Execute the query and get the list of loan entities
                var entities = await query.ToListAsync(cancellationToken);

                // Map the updated loan entities to LoanDto objects
                var loanDtos = _mapper.Map<List<LoanDto>>(entities);

                // Return the list of LoanDto objects with a 200 OK status
                return ServiceResponse<List<LoanDto>>.ReturnResultWith200(loanDtos);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error status
                _logger.LogError($"Failed to get all Loans: {e.Message}", e);
                return ServiceResponse<List<LoanDto>>.Return500(e, "Failed to get all Loans");
            }
        }

    }
}
