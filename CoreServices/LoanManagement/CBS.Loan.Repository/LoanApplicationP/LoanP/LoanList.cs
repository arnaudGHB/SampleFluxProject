
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using Microsoft.EntityFrameworkCore;

namespace CBS.NLoan.Repository.LoanApplicationP.LoanP
{


    public class LoanList : List<LoanDto>
    {
        public LoanList()
        {
        }

        public int Skip { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public PaginationMetadata PaginationMetadata;

        public LoanList(List<LoanDto> items, int count, int skip, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            Skip = skip;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public async Task<LoanList> Create(IQueryable<Loan> source, int skip, int pageSize)
        {
            var count = await GetCount(source);
            PaginationMetadata = new PaginationMetadata
            {
                PageSize = pageSize,
                Skip = skip,
                TotalCount = count,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };
            var dtoList = await GetDtos(source, skip, pageSize);
            var dtoPageList = new LoanList(dtoList, count, skip, pageSize);
            return dtoPageList;
        }

        public async Task<int> GetCount(IQueryable<Loan> source)
        {
            return await source.AsNoTracking().CountAsync();
        }

        public async Task<List<LoanDto>> GetDtos(IQueryable<Loan> source, int skip, int pageSize)
        {
            var entities = await source
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(c => new LoanDto
                {
                    Id = c.Id,
                    NumberOfInstallments = c.LoanAmortizations.Count, // Assuming each amortization represents an installment
                    RepaymentCycle = c.LoanApplication.RepaymentCircle ?? "DefaultCycle", // Handle null with a default value
                    LoanApplicationId = c.LoanApplicationId,
                    Principal = c.Principal,
                    LoanAmount = c.LoanAmount,
                    InterestForcasted = c.InterestForcasted,
                    InterestRate = c.InterestRate,
                    LastPayment = c.LastPayment,
                    Paid = c.Paid,
                    Balance = c.Balance,
                    DueAmount = c.DueAmount,
                    AccrualInterest = c.AccrualInterest,
                    AccrualInterestPaid = c.AccrualInterestPaid,
                    TotalPrincipalPaid = c.TotalPrincipalPaid,
                    Tax = c.Tax,
                    TaxPaid = c.TaxPaid,
                    FeePaid = c.FeePaid,
                    Fee = c.Fee,
                    Penalty = c.Penalty,
                    PenaltyPaid = c.PenaltyPaid,
                    DisbursementDate = c.DisbursementDate,
                    FirstInstallmentDate = c.FirstInstallmentDate,
                    NextInstallmentDate = c.NextInstallmentDate,
                    LoanDate = c.LoanDate,
                    IsLoanDisbursted = c.IsLoanDisbursted,
                    LastInterestCalculatedDate = c.LastInterestCalculatedDate,
                    LastRefundDate = c.LastRefundDate,
                    LastEventData = c.LastEventData,
                    CustomerId = c.CustomerId,
                    LoanManager = c.LoanManager,
                    LoanStatus = c.LoanStatus,
                    NewLoanId = c.NewLoanId,
                    IsWriteOffLoan = c.IsWriteOffLoan,
                    IsDeliquentLoan = c.IsDeliquentLoan,
                    IsCurrentLoan = c.IsCurrentLoan,
                    MaturityDate = c.MaturityDate,
                    OrganizationId = c.OrganizationId,
                    BranchId = c.BranchId,
                    BankId = c.BankId,
                    PaginationMetadata = PaginationMetadata // Assuming it is a global property
                })
                .ToListAsync();
            return entities;
        }

    }


}
