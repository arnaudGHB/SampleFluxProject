using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CBS.NLoan.MediatR.LoanMediaR.Handlers
{
    public class GetLoansDataTableQueryHandler : IRequestHandler<GetLoansDataTableQuery, ServiceResponse<CustomDataTable>>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanDeliquencyConfigurationRepository _loanDeliquencyConfigRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetLoansDataTableQueryHandler> _logger;

        public GetLoansDataTableQueryHandler(ILoanRepository loanRepository, IMapper mapper, ILogger<GetLoansDataTableQueryHandler> logger, ILoanDeliquencyConfigurationRepository loanDeliquencyConfigurationRepository)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _logger = logger;
            _loanDeliquencyConfigRepository=loanDeliquencyConfigurationRepository;
        }


        public async Task<ServiceResponse<CustomDataTable>> Handle(GetLoansDataTableQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var options = request.DataTableOptions;
                int skip = options.start;
                int pageSize = options.pageSize;
                string searchValue = options.searchValue?.ToLower();
                string sortColumn = options.sortColumnName ?? "LoanDate";
                string sortDirection = options.sortColumnDirection ?? "desc";

                // Step 1: Base query on loans
                var loanQuery = _loanRepository.FindBy(x => !x.IsDeleted).AsNoTracking();

                // Apply filters based on user-provided criteria
                if (!string.IsNullOrEmpty(request.Status) && request.Status.ToLower() != "all")
                {
                    loanQuery = loanQuery.Where(x => x.LoanStatus == request.Status);
                }

                if (!string.IsNullOrEmpty(request.DeliquentStatus) && request.DeliquentStatus.ToLower() != "all")
                {
                    loanQuery = loanQuery.Where(x => x.DeliquentStatus == request.DeliquentStatus);
                }

                if (!string.IsNullOrEmpty(request.BranchId))
                {
                    loanQuery = loanQuery.Where(x => x.BranchId == request.BranchId);
                }

                if (!string.IsNullOrEmpty(request.MemberId) && request.MemberId != "n/a")
                {
                    loanQuery = loanQuery.Where(x => x.CustomerId == request.MemberId);
                }

                // Apply date filters only if valid dates are provided (not MinValue)
                if (request.StartDate.HasValue && request.StartDate.Value != DateTime.MinValue)
                {
                    loanQuery = loanQuery.Where(x => x.LoanDate >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue && request.EndDate.Value != DateTime.MinValue)
                {
                    loanQuery = loanQuery.Where(x => x.LoanDate <= request.EndDate.Value);
                }

                // If search option is "all", skip filtering by search value
                if (!string.IsNullOrEmpty(searchValue) && searchValue != "all")
                {
                    loanQuery = ApplySearchFilter(loanQuery, searchValue);
                }

                // Get total count before pagination and filtering
                int recordsTotal = await loanQuery.CountAsync(cancellationToken);

                // Get the filtered count after search filtering (if any)
                int recordsFiltered = recordsTotal;

                // Validate that the sortColumn is a valid property of the Loan class
                var validProperties = typeof(Loan).GetProperties().Select(p => p.Name).ToList();
                if (!validProperties.Contains(sortColumn))
                {
                    sortColumn = "LoanDate"; // Default to LoanDate if column is invalid
                }

                // Apply sorting
                loanQuery = ApplySorting(loanQuery, sortColumn, sortDirection);

                // Apply pagination
                var loans = await loanQuery.Skip(skip).Take(pageSize).ToListAsync(cancellationToken);

                // Step 2: Join with LoanDeliquencyConfiguration
                var loanIds = loans.Select(x => x.LoanDeliquencyConfigurationId).Where(id => !string.IsNullOrEmpty(id)).ToList();
                var deliquencyConfigs = _loanDeliquencyConfigRepository.All.ToList();

                // Step 3: Map loans to DTOs with deliquency configurations
                var loanDtos = MapToLoanDtos(loans.ToList(), deliquencyConfigs).ToList();

                // Prepare the DataTable result
                var customDataTable = new CustomDataTable
                {
                    draw = int.TryParse(options.draw, out int drawValue) ? drawValue : 0,
                    recordsTotal = recordsTotal,
                    recordsFiltered = recordsFiltered,
                    data = loanDtos,
                    DataTableOptions = options
                };

                return ServiceResponse<CustomDataTable>.ReturnResultWith200(customDataTable);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching loans: {ex.Message}");
                return ServiceResponse<CustomDataTable>.Return500(ex, "Failed to retrieve loans.");
            }
        }

        //public async Task<ServiceResponse<CustomDataTable>> Handle(GetLoansDataTableQuery request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        var options = request.DataTableOptions;
        //        int skip = options.start;
        //        int pageSize = options.length;
        //        string searchValue = options.searchValue?.ToLower();
        //        string sortColumn = options.sortColumnName ?? "LoanDate";
        //        string sortDirection = options.sortColumnDirection ?? "desc";

        //        // Base query with optional join to LoanDeliquencyConfiguration
        //        var query = (from loan in _loanRepository.FindBy(x => !x.IsDeleted).AsNoTracking()
        //                    join delConfig in _loanDeliquencyConfigRepository.All.AsNoTracking()
        //                    on loan.LoanDeliquencyConfigurationId equals delConfig.Id into delConfigGroup
        //                    from delConfig in delConfigGroup.DefaultIfEmpty() // Handle cases where LoanDeliquencyConfigurationId is null
        //                    select new { Loan = loan, DeliquencyConfig = delConfig }).AsQueryable();

        //        // Apply filters based on user-provided criteria
        //        if (!string.IsNullOrEmpty(request.Status) && request.Status.ToLower() != "all")
        //        {
        //            query = query.Where(x => x.Loan.LoanStatus == request.Status);
        //        }

        //        if (!string.IsNullOrEmpty(request.DeliquentStatus) && request.DeliquentStatus.ToLower() != "all")
        //        {
        //            query = query.Where(x => x.Loan.DeliquentStatus == request.DeliquentStatus);
        //        }

        //        if (!string.IsNullOrEmpty(request.BranchId))
        //        {
        //            query = query.Where(x => x.Loan.BranchId == request.BranchId);
        //        }

        //        if (!string.IsNullOrEmpty(request.MemberId) && request.MemberId != "n/a")
        //        {
        //            query = query.Where(x => x.Loan.CustomerId == request.MemberId);
        //        }

        //        // Apply date filters only if valid dates are provided (not MinValue)
        //        if (request.StartDate.HasValue && request.StartDate.Value != DateTime.MinValue)
        //        {
        //            query = query.Where(x => x.Loan.LoanDate >= request.StartDate.Value);
        //        }

        //        if (request.EndDate.HasValue && request.EndDate.Value != DateTime.MinValue)
        //        {
        //            query = query.Where(x => x.Loan.LoanDate <= request.EndDate.Value);
        //        }

        //        // If search option is "all", skip filtering by search value
        //        if (!string.IsNullOrEmpty(searchValue) && searchValue != "all")
        //        {
        //            query = query.Where(x =>
        //                x.Loan.CustomerName.ToLower().Contains(searchValue) ||
        //                x.Loan.BranchId.ToLower().Contains(searchValue) ||
        //                x.Loan.AccountNumber.ToLower().Contains(searchValue) ||
        //                x.Loan.LoanStatus.ToLower().Contains(searchValue) ||
        //                (x.DeliquencyConfig != null && x.DeliquencyConfig.Name.ToLower().Contains(searchValue))
        //            );
        //        }

        //        // Get total count before pagination and filtering
        //        int recordsTotal = await query.CountAsync(cancellationToken);

        //        // Get the filtered count after search filter (if applied)
        //        int recordsFiltered = await query.CountAsync(cancellationToken);

        //        // Validate that the sortColumn is a valid property of the Loan class
        //        var validProperties = typeof(Loan).GetProperties().Select(p => p.Name).ToList();
        //        if (!validProperties.Contains(sortColumn))
        //        {
        //            sortColumn = "LoanDate"; // Default to LoanDate if the column is invalid
        //        }

        //        // Apply sorting using System.Linq.Dynamic.Core
        //        query = ApplySorting(query, sortColumn, sortDirection);

        //        // Apply pagination and materialize query
        //        var loanDetails = await query
        //            .Skip(skip)
        //            .Take(pageSize)
        //            .ToListAsync(cancellationToken);

        //        // Map results to DTOs
        //        var loanDtos = loanDetails.Select(x => MapToLoanDto(x.Loan, x.DeliquencyConfig)).ToList();

        //        // Prepare the DataTable result
        //        var customDataTable = new CustomDataTable
        //        {
        //            draw = int.TryParse(options.draw, out int drawValue) ? drawValue : 0,
        //            recordsTotal = recordsTotal,
        //            recordsFiltered = recordsFiltered,
        //            data = loanDtos,
        //            DataTableOptions = options
        //        };

        //        return ServiceResponse<CustomDataTable>.ReturnResultWith200(customDataTable);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError($"Error fetching loans: {ex.Message}");
        //        return ServiceResponse<CustomDataTable>.Return500(ex, "Failed to retrieve loans.");
        //    }
        //}

        private List<LoanDto> MapToLoanDtos(List<Loan> loans, List<LoanDeliquencyConfiguration> delConfigs)
        {
            // Create a dictionary for faster lookups of deliquency configurations (ignoring null IDs)
            var delConfigDict = delConfigs
                .Where(config => config.Id != null)
                .ToDictionary(config => config.Id, config => config);

            return loans.Select(loan =>
            {
                // Safely find the corresponding deliquency configuration (null if not found)
                delConfigDict.TryGetValue(loan.LoanDeliquencyConfigurationId ?? string.Empty, out var delConfig);

                return new LoanDto
                {
                    Id = loan.Id,
                    LoanApplicationId = loan.LoanApplicationId,
                    Principal = loan.Principal,
                    LoanAmount = loan.LoanAmount,
                    InterestForcasted = loan.InterestForcasted,
                    InterestRate = loan.InterestRate,
                    LastPayment = loan.LastPayment,
                    Paid = loan.Paid,
                    Balance = loan.Balance,
                    DueAmount = loan.DueAmount,
                    RequestedAmount = loan.LoanAmount,
                    RestructuredBalance = loan.OldBalance,
                    AccrualInterest = loan.AccrualInterest,
                    LastCalculatedInterest = loan.LastCalculatedInterest,
                    AccrualInterestPaid = loan.AccrualInterestPaid,
                    TotalPrincipalPaid = loan.TotalPrincipalPaid,
                    Tax = loan.Tax,
                    TaxPaid = loan.TaxPaid,
                    Fee = loan.Fee,
                    FeePaid = loan.FeePaid,
                    LoanDuration=loan.LoanDuration,
                    LoanManager=loan.LoanManager, IsCompleted=loan.IsCompleted,
                    Penalty = loan.Penalty,
                    PenaltyPaid = loan.PenaltyPaid,
                    DisbursementDate = loan.DisbursementDate,
                    FirstInstallmentDate = loan.FirstInstallmentDate,
                    NextInstallmentDate = loan.NextInstallmentDate,
                    LoanDate = loan.LoanDate,
                    IsLoanDisbursted = loan.IsLoanDisbursted,
                    DisbursmentStatus = loan.DisbursmentStatus ?? "Unknown",
                    LastInterestCalculatedDate = loan.LastInterestCalculatedDate,
                    LastRefundDate = loan.LastRefundDate,
                    LastEventData = loan.LastEventData,
                    CustomerId = loan.CustomerId,
                    CustomerName = loan.CustomerName ?? "N/A",
                    BranchId = loan.BranchId,
                    LoanStatus = loan.LoanStatus ?? "Unknown",
                    IsRestructured = loan.LoanStructuringStatus?.ToLower() == "restructured",
                    NewLoanId = loan.NewLoanId ?? "N/A",
                    IsWriteOffLoan = loan.IsWriteOffLoan,
                    IsDeliquentLoan = loan.IsDeliquentLoan,
                    IsCurrentLoan = loan.IsCurrentLoan,
                    MaturityDate = loan.MaturityDate,
                    OrganizationId = loan.OrganizationId,
                    BankId = loan.BankId ?? "Unknown",
                    LoanType = loan.LoanType ?? "Standard",
                    BranchCode = loan.BranchCode ?? "N/A",
                    LoanId = loan.LoanId ?? "N/A",
                    LoanJourneyStatus = loan.LoanJourneyStatus ?? "Normal",
                    VatRate = loan.VatRate,
                    LoanTarget = loan.LoanTarget ?? "Individual",
                    LoanCategory = loan.LoanCategory ?? "Main_Loan",
                    AccountNumber = loan.AccountNumber ?? "N/A",
                    IsUpload = loan.IsUpload,
                    NumberOfInstallments = loan.LoanDuration,
                    RepaymentCycle = "Monthly",
                    DeliquentInterest = loan.DeliquentInterest,
                    AdvancedPaymentDays = loan.AdvancedPaymentDays,
                    DeliquentDays = loan.DeliquentDays,
                    AdvancedPaymentAmount = loan.AdvancedPaymentAmount,
                    DeliquentAmount = loan.DeliquentAmount,
                    LoanStructuringStatus = loan.LoanStructuringStatus ?? "N/A",
                    LoanStructuringDate = loan.LoanStructuringDate,
                    OldCapital = loan.OldCapital,
                    OldInterest = loan.OldInterest,
                    OldVAT = loan.OldVAT,
                    OldPenalty = loan.OldPenalty,
                    OldBalance = loan.OldBalance,
                    OldDueAmount = loan.OldDueAmount,
                    DeliquentStatus = loan.DeliquentStatus ?? "Unknown",
                    StopInterestCalculation = loan.StopInterestCalculation,
                    StoppedBy = loan.StoppedBy ?? "Normal",
                    DateInterestWastStoped = loan.DateInterestWastStoped ?? DateTime.MinValue,
                    LastDeliquecyProcessedDate = loan.LastDeliquecyProcessedDate ?? DateTime.MinValue,
                    MigrationDate = loan.MigrationDate ?? DateTime.MinValue,
                    InterestMustBePaidUpFront = loan.InterestMustBePaidUpFront,
                    InterestAmountUpfront = loan.InterestAmountUpfront,
                    LoanDeliquencyConfigurationId = loan.LoanDeliquencyConfigurationId ?? "N/A",
                    Savings = loan.Savings,
                    OShares = loan.OShares,
                    PShares = loan.PShares,
                    Deposit = loan.Deposit,
                    Salary = loan.Salary,
                    Shortee = loan.Shortee,
                    Co_Obligor = loan.Co_Obligor,
                    Co_OperationGurantor = loan.Co_OperationGurantor,
                    OtherGuaranteeFund = loan.OtherGuaranteeFund,
                    TotalFundGuranteed = loan.TotalFundGuranteed,
                    PercentageOfLiquidityCoverage = loan.PercentageOfLiquidityCoverage,
                    PercentageOfCollateralCoverage = loan.PercentageOfCollateralCoverage,
                    PercentageOfOverAllCoverage = loan.PercentageOfOverAllCoverage,
                    LoanDeliquencyConfiguration = delConfig,
                    LoanDeliquencyConfigurationName = delConfig?.Name ?? "N/A"
                };
            }).ToList();
        }


        private LoanDto MapToLoanDto(Loan loan, LoanDeliquencyConfiguration delConfig)
        {
            return new LoanDto
            {
                Id = loan.Id,
                LoanApplicationId = loan.LoanApplicationId,
                Principal = loan.Principal,
                LoanAmount = loan.LoanAmount,
                InterestForcasted = loan.InterestForcasted,
                InterestRate = loan.InterestRate,
                LastPayment = loan.LastPayment,
                Paid = loan.Paid,
                Balance = loan.Balance,
                DueAmount = loan.DueAmount,
                RequestedAmount = loan.LoanAmount,  // Assuming RequestedAmount refers to the initial loan amount
                RestructuredBalance = loan.OldBalance,
                AccrualInterest = loan.AccrualInterest,
                LastCalculatedInterest = loan.LastCalculatedInterest,
                AccrualInterestPaid = loan.AccrualInterestPaid,
                TotalPrincipalPaid = loan.TotalPrincipalPaid,
                Tax = loan.Tax,
                TaxPaid = loan.TaxPaid,
                Fee = loan.Fee,
                FeePaid = loan.FeePaid,
                Penalty = loan.Penalty,
                PenaltyPaid = loan.PenaltyPaid,
                DisbursementDate = loan.DisbursementDate,
                FirstInstallmentDate = loan.FirstInstallmentDate,
                NextInstallmentDate = loan.NextInstallmentDate,
                LoanDate = loan.LoanDate,
                IsLoanDisbursted = loan.IsLoanDisbursted,
                DisbursmentStatus = loan.DisbursmentStatus ?? "Unknown",
                LastInterestCalculatedDate = loan.LastInterestCalculatedDate,
                LastRefundDate = loan.LastRefundDate,
                LastEventData = loan.LastEventData,
                CustomerId = loan.CustomerId,
                CustomerName = loan.CustomerName ?? "N/A",
                BranchId = loan.BranchId,
                LoanStatus = loan.LoanStatus ?? "Unknown",
                IsRestructured = loan.LoanStructuringStatus?.ToLower() == "restructured",
                NewLoanId = loan.NewLoanId ?? "N/A",
                IsWriteOffLoan = loan.IsWriteOffLoan,
                IsDeliquentLoan = loan.IsDeliquentLoan,
                IsCurrentLoan = loan.IsCurrentLoan,
                MaturityDate = loan.MaturityDate,
                OrganizationId = loan.OrganizationId,
                BankId = loan.BankId ?? "Unknown",
                LoanType = loan.LoanType ?? "Standard",
                BranchCode = loan.BranchCode ?? "N/A",
                LoanId = loan.LoanId ?? "N/A",
                LoanJourneyStatus = loan.LoanJourneyStatus ?? "Normal",
                VatRate = loan.VatRate,
                LoanTarget = loan.LoanTarget ?? "Individual",
                LoanCategory = loan.LoanCategory ?? "Main_Loan",
                AccountNumber = loan.AccountNumber ?? "N/A",
                IsUpload = loan.IsUpload,
                NumberOfInstallments = loan.LoanDuration,
                RepaymentCycle = "Monthly",  // Assuming default cycle
                DeliquentInterest = loan.DeliquentInterest,
                AdvancedPaymentDays = loan.AdvancedPaymentDays,
                DeliquentDays = loan.DeliquentDays,
                AdvancedPaymentAmount = loan.AdvancedPaymentAmount,
                DeliquentAmount = loan.DeliquentAmount,
                LoanStructuringStatus = loan.LoanStructuringStatus ?? "N/A",
                LoanStructuringDate = loan.LoanStructuringDate,
                OldCapital = loan.OldCapital,
                OldInterest = loan.OldInterest,
                OldVAT = loan.OldVAT,
                OldPenalty = loan.OldPenalty,
                OldBalance = loan.OldBalance,
                OldDueAmount = loan.OldDueAmount,
                DeliquentStatus = loan.DeliquentStatus ?? "Unknown",
                StopInterestCalculation = loan.StopInterestCalculation,
                StoppedBy = loan.StoppedBy ?? "Normal",
                DateInterestWastStoped = loan.DateInterestWastStoped ?? DateTime.MinValue,
                LastDeliquecyProcessedDate = loan.LastDeliquecyProcessedDate ?? DateTime.MinValue,
                MigrationDate = loan.MigrationDate ?? DateTime.MinValue,
                InterestMustBePaidUpFront = loan.InterestMustBePaidUpFront,
                InterestAmountUpfront = loan.InterestAmountUpfront,
                LoanDeliquencyConfigurationId = loan.LoanDeliquencyConfigurationId ?? "N/A",
                Savings = loan.Savings,
                OShares = loan.OShares,
                PShares = loan.PShares,
                Deposit = loan.Deposit,
                Salary = loan.Salary,
                Shortee = loan.Shortee,
                Co_Obligor = loan.Co_Obligor,
                Co_OperationGurantor = loan.Co_OperationGurantor,
                OtherGuaranteeFund = loan.OtherGuaranteeFund,
                TotalFundGuranteed = loan.TotalFundGuranteed,
                PercentageOfLiquidityCoverage = loan.PercentageOfLiquidityCoverage,
                PercentageOfCollateralCoverage = loan.PercentageOfCollateralCoverage,
                PercentageOfOverAllCoverage = loan.PercentageOfOverAllCoverage,
                LoanDeliquencyConfiguration = delConfig,
                LoanDeliquencyConfigurationName = delConfig?.Name ?? "N/A"
            };
        }

        private IQueryable<Loan> ApplySearchFilter(IQueryable<Loan> query, string searchValue)
        {
            return query.Where(x =>
                x.CustomerName.ToLower().Contains(searchValue) ||
                x.BranchId.ToLower().Contains(searchValue) ||
                x.AccountNumber.ToLower().Contains(searchValue) ||
                x.CustomerId.ToLower().Contains(searchValue) ||
                x.DeliquentStatus.ToLower().Contains(searchValue) ||
                x.LoanStatus.ToLower().Contains(searchValue));
        }
        private IQueryable<dynamic> ApplySorting(IQueryable<dynamic> query, string sortColumn, string sortDirection)
        {
            var parameter = Expression.Parameter(typeof(Loan), "loan");
            var property = typeof(Loan).GetProperty(sortColumn);

            if (property == null)
            {
                property = typeof(Loan).GetProperty("LoanDate"); // Default to LoanDate if column invalid
            }

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            string methodName = sortDirection.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { query.ElementType, property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression)
            );

            return query.Provider.CreateQuery<dynamic>(resultExpression);
        }

        private IQueryable<Loan> ApplySorting(IQueryable<Loan> query, string sortColumn, string sortDirection)
        {
            var parameter = Expression.Parameter(typeof(Loan), "x");
            var property = typeof(Loan).GetProperty(sortColumn);

            if (property == null)
            {
                property = typeof(Loan).GetProperty("LoanDate"); // Default to LoanDate
            }

            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            string methodName = sortDirection.ToLower() == "asc" ? "OrderBy" : "OrderByDescending";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { query.ElementType, property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression)
            );

            return query.Provider.CreateQuery<Loan>(resultExpression);
        }

    }
}
