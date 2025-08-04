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
using System.Linq.Expressions;
using System.Reflection;

namespace CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers
{
    public class GetLoanApplicationsDataTableQueryHandler : IRequestHandler<GetLoanApplicationsDataTableQuery, ServiceResponse<CustomDataTable>>
    {
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLoanApplicationsDataTableQueryHandler> _logger;

        public GetLoanApplicationsDataTableQueryHandler(
            ILoanApplicationRepository loanApplicationRepository,
            IMapper mapper,
            ILogger<GetLoanApplicationsDataTableQueryHandler> logger)
        {
            _loanApplicationRepository = loanApplicationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<CustomDataTable>> Handle(GetLoanApplicationsDataTableQuery request, CancellationToken cancellationToken)
        {
            ///
            try
            {
                var options = request.DataTableOptions;
                int skip = options.start;
                int pageSize = options.length;
                string searchValue = options.searchValue?.Trim();
                string sortColumn = options.sortColumnName ?? "ApplicationDate";
                string sortDirection = options.sortColumnDirection ?? "desc";

                // Base Query
                IQueryable<LoanApplication> query = _loanApplicationRepository
                    .FindBy(x => !x.IsDeleted)
                    .AsNoTracking();

                // Apply Filters
                if (!string.IsNullOrEmpty(request.Status) && request.Status.ToLower() != "all")
                {
                    query = query.Where(x => x.ApprovalStatus == request.Status);
                }

                if (!string.IsNullOrEmpty(request.BranchId) && request.BranchId.ToLower() != "all")
                {
                    query = query.Where(x => x.BranchId == request.BranchId);
                }

                if (!string.IsNullOrEmpty(request.MemberId) && request.MemberId != "n/a")
                {
                    query = query.Where(x => x.CustomerId == request.MemberId);
                }

                if (request.StartDate.HasValue)
                {
                    query = query.Where(x => x.ApplicationDate >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(x => x.ApplicationDate <= request.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(request.LoanCategory) && request.LoanCategory.ToLower() != "all")
                {
                    query = query.Where(x => x.LoanCategory == request.LoanCategory);
                }

                if (!string.IsNullOrEmpty(request.LoanTarget) && request.LoanTarget.ToLower() != "all")
                {
                    query = query.Where(x => x.LoanTarget == request.LoanTarget);
                }

                if (!string.IsNullOrEmpty(request.ApprovalStatus) && request.ApprovalStatus.ToLower() != "all")
                {
                    query = query.Where(x => x.Status == request.ApprovalStatus);
                }

                // Search Functionality
                if (!string.IsNullOrEmpty(searchValue))
                {
                    query = query.Where(x =>
                        x.CustomerId.Contains(searchValue) ||
                        x.BranchId.Contains(searchValue) ||
                        x.LoanProductId.Contains(searchValue) ||
                        x.ApprovalStatus.Contains(searchValue) ||
                        x.CustomerName.Contains(searchValue) ||
                        x.LoanCategory.Contains(searchValue) ||
                        x.LoanTarget.Contains(searchValue)
                    );
                }

                // Get Total Records Before Pagination
                int recordsTotal = await query.CountAsync(cancellationToken);

                // Apply Sorting
                query = ApplySorting(query, sortColumn, sortDirection);

                // Apply Pagination
                var loanApplications = await query
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                // Map to DTOs
                var loanApplicationDtos = _mapper.Map<List<LoanApplicationDto>>(loanApplications);

                // Prepare the DataTable Result
                var customDataTable = new CustomDataTable
                {
                    draw = int.TryParse(options.draw, out int drawValue) ? drawValue : 0,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = loanApplicationDtos,
                    DataTableOptions = options
                };

                return ServiceResponse<CustomDataTable>.ReturnResultWith200(customDataTable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching loan applications: {ex.Message}");
                return ServiceResponse<CustomDataTable>.Return500(ex, "Failed to retrieve loan applications.");
            }
        }

        /// <summary>
        /// Dynamically applies sorting based on the column name.
        /// </summary>
        private IQueryable<LoanApplication> ApplySorting(IQueryable<LoanApplication> query, string sortColumn, string sortDirection)
        {
            try
            {
                var parameter = Expression.Parameter(typeof(LoanApplication), "x");
                var property = typeof(LoanApplication).GetProperty(sortColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)
                {
                    property = typeof(LoanApplication).GetProperty("ApplicationDate"); // Default to ApplicationDate
                }

                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExpression = Expression.Lambda(propertyAccess, parameter);

                var methodName = sortDirection.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";
                var resultExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { query.ElementType, property.PropertyType },
                    query.Expression,
                    Expression.Quote(orderByExpression)
                );

                return query.Provider.CreateQuery<LoanApplication>(resultExpression);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ApplySorting: {ex.Message}");
                return query.OrderByDescending(x => x.ApplicationDate); // Default Fallback Sorting
            }
        }
    }
}
