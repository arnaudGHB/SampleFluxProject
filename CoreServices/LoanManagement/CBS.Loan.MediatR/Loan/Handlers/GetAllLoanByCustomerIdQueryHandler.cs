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
    public class GetAllLoanByCustomerIdQueryHandler : IRequestHandler<GetAllLoanByCustomerIdQuery, ServiceResponse<List<LoanDto>>>
    {
        private readonly ILoanRepository _LoanRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanByCustomerIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanQueryHandler.
        /// </summary>
        /// <param name="LoanRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanByCustomerIdQueryHandler(
            ILoanRepository LoanRepository,
            IMapper mapper, ILogger<GetAllLoanByCustomerIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanRepository = LoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanQuery to retrieve all Loans.
        /// </summary>
        /// <param name="request">The GetAllLoanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanDto>>> Handlex(GetAllLoanByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<Loan> query = _LoanRepository.AllIncluding(x => x.LoanApplication)
                    .AsNoTracking()
                    .Where(x => x.CustomerId == request.CustomerId);

                switch (request.QueryParameter.ToLower())
                {
                    case "open":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Open.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "closed":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Closed.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "disbursed":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Disbursed.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "restructured":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Restructured.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "rescheduled":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Rescheduled.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "current":
                        query = query.Where(x => x.IsCurrentLoan && x.IsLoanDisbursted);
                        break;
                    case "refinanced":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Refinancing.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "approved":
                        query = query.Where(x => !x.IsLoanDisbursted);
                        break;
                    case "all":
                        // No additional filter for 'all' since we want all statuses
                        break;
                    default:
                        throw new ArgumentException("Invalid LoanStatus");
                }
                //query = query
                //    .Include(x => x.LoanApplication.LoanCommiteeValidations)
                //    .Include(x => x.LoanApplication.LoanProduct)
                //    .Include(x => x.LoanApplication.Collateras)
                //    .Include(x => x.LoanApplication.Guarantors)
                //    .Include(x => x.LoanApplication.LoanPurpose)
                //    .Include(x => x.LoanApplication.DocumentAttachedToLoans)
                //    .Include(x => x.LoanApplication.LoanApplicationFees)
                //    .Include(x => x.LoanAmortizations)
                //    .Include(x => x.Refunds);

                var entities = await query.ToListAsync();

                return ServiceResponse<List<LoanDto>>.ReturnResultWith200(_mapper.Map<List<LoanDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Loans: {e.Message}");
                return ServiceResponse<List<LoanDto>>.Return500(e, "Failed to get all Loans");
            }
        }
        public async Task<ServiceResponse<List<LoanDto>>> Handle(GetAllLoanByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<Loan> query = _LoanRepository.FindBy(x => x.CustomerId == request.CustomerId)
                    .AsNoTracking();
                switch (request.QueryParameter.ToLower())
                {
                    case "open":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Open.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "closed":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Closed.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "disbursed":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Disbursed.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "restructured":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Restructured.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "rescheduled":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Rescheduled.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "current":
                        query = query.Where(x => x.IsCurrentLoan && x.IsLoanDisbursted);
                        break;
                    case "refinanced":
                        query = query.Where(x => x.LoanStatus.ToLower() == LoanStatus.Refinancing.ToString().ToLower()&& x.IsLoanDisbursted);
                        break;
                    case "approved":
                        query = query.Where(x => !x.IsLoanDisbursted);
                        break;
                    case "all":
                        // No additional filter for 'all' since we want all statuses
                        break;
                    default:
                        throw new ArgumentException("Invalid LoanStatus");
                }
                //query = query
                //    .Include(x => x.LoanApplication.LoanCommiteeValidations)
                //    .Include(x => x.LoanApplication.LoanProduct)
                //    .Include(x => x.LoanApplication.Collateras)
                //    .Include(x => x.LoanApplication.Guarantors)
                //    .Include(x => x.LoanApplication.LoanPurpose)
                //    .Include(x => x.LoanApplication.DocumentAttachedToLoans)
                //    .Include(x => x.LoanApplication.LoanApplicationFees)
                //    .Include(x => x.LoanAmortizations)
                //    .Include(x => x.Refunds);

                var entities = await query.ToListAsync();

                return ServiceResponse<List<LoanDto>>.ReturnResultWith200(_mapper.Map<List<LoanDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Loans: {e.Message}");
                return ServiceResponse<List<LoanDto>>.Return500(e, "Failed to get all Loans");
            }
        }


    }
}
