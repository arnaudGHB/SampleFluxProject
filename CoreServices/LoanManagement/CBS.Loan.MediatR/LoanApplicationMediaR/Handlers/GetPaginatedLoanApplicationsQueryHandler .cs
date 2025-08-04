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
    public class GetPaginatedLoanApplicationsQueryHandler : IRequestHandler<GetPaginatedLoanApplicationsQuery, ServiceResponse<PaginatedResult<LoanApplicationDto>>>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaginatedLoanApplicationsQueryHandler> _logger;

        public GetPaginatedLoanApplicationsQueryHandler(
            ILoanApplicationRepository loanApplicationRepository,
            IMapper mapper,
            ILogger<GetPaginatedLoanApplicationsQueryHandler> logger)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the retrieval of paginated loan applications based on various filter criteria.
        /// Filters include loan status, branch, member, and date range, and the response includes pagination details.
        /// </summary>
        /// <param name="request">The query containing filter and pagination parameters.</param>
        /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
        /// <returns>A service response containing the paginated result of loan applications.</returns>
        public async Task<ServiceResponse<PaginatedResult<LoanApplicationDto>>> Handle(GetPaginatedLoanApplicationsQuery request, CancellationToken cancellationToken)
        {
            string auditMessage = string.Empty;
            try
            {
                _logger.LogInformation("Starting retrieval of paginated loan applications | Page: {PageNumber}, Size: {PageSize}, BranchId: {BranchId}, MemberId: {MemberId}, Status: {ParamMeter}",
                    request.PageNumber, request.PageSize, request.BranchId ?? "All", request.MemberId ?? "All", request.ParamMeter ?? "All");

                // Step 1: Build base query
                IQueryable<LoanApplication> query = _loanApplicationRepository
                    .FindBy(x => !x.IsDeleted)
                    .AsNoTracking()
                    .Include(x => x.LoanApplicationFees)
                    .Include(x => x.Guarantors)
                    .Include(x => x.DocumentAttachedToLoans)
                    .Include(x => x.LoanCommiteeValidations)
                    .Include(x => x.LoanProduct.LoanProductRepaymentCycles);

                // Step 2: Apply filters
                query = ApplyFilters(query, request);

                // Step 3: Get total items count before pagination
                int totalItems = await query.CountAsync(cancellationToken);

                if (totalItems == 0)
                {
                    _logger.LogInformation("No loan applications found for the given filter criteria.");
                    auditMessage = "No loan applications found based on the provided filters.";
                    await BaseUtilities.LogAndAuditAsync(auditMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanApplicationSearch, LogLevelInfo.Information);
                    var emptyResult = new PaginatedResult<LoanApplicationDto>(new List<LoanApplicationDto>(), request.PageNumber, request.PageSize, totalItems);
                    return ServiceResponse<PaginatedResult<LoanApplicationDto>>.ReturnResultWith200(emptyResult, auditMessage);
                }

                // Step 4: Apply pagination and ordering
                var paginatedLoanApplications = await query
                    .OrderByDescending(x => x.ApplicationDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                // Step 5: Map entities to DTOs
                var loanApplicationDtos = _mapper.Map<List<LoanApplicationDto>>(paginatedLoanApplications);

                // Step 6: Prepare and return the response
                var result = new PaginatedResult<LoanApplicationDto>(loanApplicationDtos, request.PageNumber, request.PageSize, totalItems);
                auditMessage = $"Successfully fetched {loanApplicationDtos.Count} loan applications for Page: {request.PageNumber}.";
                _logger.LogInformation(auditMessage);
                await BaseUtilities.LogAndAuditAsync(auditMessage, request, HttpStatusCodeEnum.OK, LogAction.LoanApplicationSearch, LogLevelInfo.Information);

                return ServiceResponse<PaginatedResult<LoanApplicationDto>>.ReturnResultWith200(result);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Failed to fetch paginated loan applications: {ex.Message}";
                _logger.LogError(ex, errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.LoanApplicationSearch, LogLevelInfo.Error);
                return ServiceResponse<PaginatedResult<LoanApplicationDto>>.Return500(errorMessage);
            }
        }

        /// <summary>
        /// Applies the necessary filters to the loan applications query based on the request parameters.
        /// </summary>
        /// <param name="query">The initial query to filter.</param>
        /// <param name="request">The query containing filter criteria.</param>
        /// <returns>The filtered query.</returns>
        private IQueryable<LoanApplication> ApplyFilters(IQueryable<LoanApplication> query, GetPaginatedLoanApplicationsQuery request)
        {
            // Apply status filter
            if (!string.IsNullOrEmpty(request.ParamMeter))
            {
                query = request.ParamMeter.ToLower() switch
                {
                    "pending" => query.Where(x => x.ApprovalStatus == LoanApplicationStatus.Pending.ToString()),
                    "approved" => query.Where(x => x.ApprovalStatus == LoanApplicationStatus.Approved.ToString()),
                    "rejected" => query.Where(x => x.ApprovalStatus == LoanApplicationStatus.Rejected.ToString()),
                    _ => query // No filtering by status for "all" or invalid status values.
                };
            }

            // Apply branch filter
            if (!string.IsNullOrEmpty(request.BranchId))
            {
                query = query.Where(x => x.BranchId == request.BranchId);
            }

            // Apply member filter
            if (!string.IsNullOrEmpty(request.MemberId))
            {
                query = query.Where(x => x.CustomerId == request.MemberId);
            }

            // Apply date filters
            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.ApplicationDate >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.ApplicationDate <= request.EndDate.Value);
            }

            return query;
        }
    }
}
