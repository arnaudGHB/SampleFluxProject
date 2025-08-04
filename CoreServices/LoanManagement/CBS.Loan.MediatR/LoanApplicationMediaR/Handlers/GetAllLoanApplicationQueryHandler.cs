using AutoMapper;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Queries;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using CBS.NLoan.Repository.LoanApplicationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanApplicationQueryHandler : IRequestHandler<GetAllLoanApplicationQuery, ServiceResponse<List<LoanApplicationDto>>>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository; // Repository for accessing Loans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanApplicationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly ILoanCommiteeGroupRepository _loanCommiteeGroupRepository; // Repository for accessing Loans data.
        private readonly ILoanCommeteeMemberRepository _loanCommeteeMemberRepository; // Repository for accessing Loans data.
        private readonly UserInfoToken _userInfoToken; // Repository for accessing Loans data.

        /// <summary>
        /// Constructor for initializing the GetAllLoanApplicationQueryHandler.
        /// </summary>
        /// <param name="loanApplicationRepository">Repository for Loans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanApplicationQueryHandler(
            ILoanApplicationRepository loanApplicationRepository,
            IMapper mapper,
            ILogger<GetAllLoanApplicationQueryHandler> logger,
            ILoanCommiteeGroupRepository loanCommiteeGroupRepository = null,
            ILoanCommeteeMemberRepository loanCommeteeMemberRepository = null,
            UserInfoToken userInfoToken = null)
        {
            // Assign provided dependencies to local variables.
            _loanApplicationRepository = loanApplicationRepository;
            _mapper = mapper;
            _logger = logger;
            _loanCommiteeGroupRepository = loanCommiteeGroupRepository;
            _loanCommeteeMemberRepository = loanCommeteeMemberRepository;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllLoanApplicationQuery to retrieve all Loans.
        /// </summary>
        /// <param name="request">The GetAllLoanApplicationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanApplicationDto>>> Handlex(GetAllLoanApplicationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Start building the base query as IQueryable<Loan>
                IQueryable<LoanApplication> query = _loanApplicationRepository.FindBy(x => x.IsDeleted == false)
                                                     .AsNoTracking() // Improves performance by disabling change tracking
                                                     .Include(x => x.LoanApplicationFees)
                                                     .Include(x => x.Guarantors)
                                                     .Include(x => x.DocumentAttachedToLoans)
                                                     .Include(x => x.LoanCommiteeValidations)
                                                              .Include(x => x.LoanApplicationFees)
                                                     .Include(x => x.LoanProduct.LoanProductRepaymentCycles);

                // Apply conditional filtering
                if (request.ParamMeter == "Pending")
                {
                    query = query.Where(x => x.ApprovalStatus != LoanApplicationStatus.Approved.ToString() && x.ApprovalStatus != LoanApplicationStatusX.Validated.ToString());
                }
                else
                {
                    query = query.Where(x => x.ApprovalStatus == LoanApplicationStatusX.Validated.ToString());
                }

                // Execute query and project to DTOs
                var loanApplications = await query.Select(x => _mapper.Map<LoanApplicationDto>(x)).ToListAsync(cancellationToken);

                // Return the result with a 200 status code
                return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(loanApplications);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Loans: {e.Message}");
                return ServiceResponse<List<LoanApplicationDto>>.Return500(e, "Failed to get all Loans");
            }
        }

        public async Task<ServiceResponse<List<LoanApplicationDto>>> Handle(GetAllLoanApplicationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the requested parameter is for "Pending" loan applications
                if (request.ParamMeter == "Pending")
                {
                    // Retrieve the current user's ID from the token
                    var userId = _userInfoToken.Id;
                    _logger.LogInformation($"Fetching pending loans for validation by committee member: {_userInfoToken.FullName}");

                    // Get all loan committee memberships where the user is a member
                    var loanCommiteeMemberships = await _loanCommeteeMemberRepository
                        .FindBy(x => x.UserId == userId)
                        .Include(x => x.LoanCommiteeGroup)
                        .ToListAsync(cancellationToken);

                    // If the user is not part of any loan committee group, log and return an empty result
                    if (loanCommiteeMemberships == null || !loanCommiteeMemberships.Any())
                    {
                        _logger.LogWarning($"{_userInfoToken.FullName} is not a member of any loan validation committee.");
                        return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(new List<LoanApplicationDto>());
                    }

                    // Extract loan amount ranges from all loan committee groups the user is part of
                    var loanRanges = loanCommiteeMemberships.Select(x => new
                    {
                        MinLoanAmount = x.LoanCommiteeGroup.MinimumLoanAmount,
                        MaxLoanAmount = x.LoanCommiteeGroup.MaximumLoanAmount
                    }).ToList();

                    // Start building the base query for loan applications
                    IQueryable<LoanApplication> loanApplicationsQuery = _loanApplicationRepository
                        .FindBy(x => x.IsDeleted == false)
                        .AsNoTracking() // Optimize by preventing tracking since we don't need to modify the entities
                        .Include(x => x.LoanApplicationFees)
                        .Include(x => x.Guarantors)
                        .Include(x => x.DocumentAttachedToLoans)
                        .Include(x => x.LoanCommiteeValidations)
                        .Include(x => x.LoanProduct.LoanProductRepaymentCycles);

                    // Filter for loans that are not yet approved or validated
                    loanApplicationsQuery = loanApplicationsQuery.Where(x =>
                        x.ApprovalStatus != LoanApplicationStatus.Approved.ToString() &&
                        x.ApprovalStatus != LoanApplicationStatusX.Validated.ToString());

                    // Fetch the query results to apply client-side filtering
                    var loanApplicationsList = await loanApplicationsQuery.ToListAsync(cancellationToken);

                    // Apply client-side filtering based on the user's validation range
                    var filteredLoanApplications = loanApplicationsList
                        .Where(loan => loanRanges.Any(range => loan.Amount >= range.MinLoanAmount && loan.Amount <= range.MaxLoanAmount))
                        .ToList();

                    // If no loans are found, log and return an empty result
                    if (!filteredLoanApplications.Any())
                    {
                        _logger.LogWarning($"{_userInfoToken.FullName}, no loans are within the validation range.");
                        return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(new List<LoanApplicationDto>());
                    }

                    // Map to DTOs
                    var loanApplications = filteredLoanApplications
                        .Select(x => _mapper.Map<LoanApplicationDto>(x))
                        .ToList();

                    // Log successful retrieval and return loan applications with 200 status
                    _logger.LogInformation($"{loanApplications.Count} pending loan applications found for {_userInfoToken.FullName}.");
                    return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(loanApplications);
                }
                else
                {
                    // Log fetching all validated loans
                    _logger.LogInformation($"Fetching all validated loans for {_userInfoToken.FullName}.");

                    // Build base query for loan applications that are validated
                    IQueryable<LoanApplication> loanApplicationsQuery = _loanApplicationRepository
                        .FindBy(x => x.IsDeleted == false)
                        .AsNoTracking()
                        .Include(x => x.LoanApplicationFees)
                        .Include(x => x.Guarantors)
                        .Include(x => x.DocumentAttachedToLoans)
                        .Include(x => x.LoanCommiteeValidations)
                        .Include(x => x.LoanProduct.LoanProductRepaymentCycles);

                    // Filter only validated loan applications
                    loanApplicationsQuery = loanApplicationsQuery.Where(x => x.ApprovalStatus == LoanApplicationStatusX.Validated.ToString());

                    // Fetch and map the query results to DTOs
                    var loanApplications = await loanApplicationsQuery
                        .Select(x => _mapper.Map<LoanApplicationDto>(x))
                        .ToListAsync(cancellationToken);

                    // If no loans are found, log and return an empty result
                    if (!loanApplications.Any())
                    {
                        _logger.LogWarning($"{_userInfoToken.FullName}, no validated loans found.");
                        return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(new List<LoanApplicationDto>());
                    }

                    // Log successful retrieval and return loan applications with 200 status
                    _logger.LogInformation($"{loanApplications.Count} validated loan applications found for {_userInfoToken.FullName}.");
                    return ServiceResponse<List<LoanApplicationDto>>.ReturnResultWith200(loanApplications);
                }
            }
            catch (Exception e)
            {
                // Log error with details and return a 500 error response
                _logger.LogError($"Failed to retrieve loan applications for {_userInfoToken.FullName}: {e.Message}");
                return ServiceResponse<List<LoanApplicationDto>>.Return500(e, "Failed to retrieve loan applications.");
            }
        }

    }
}
