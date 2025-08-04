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
    public class GetOpenLoansByBranchQueryHandler : IRequestHandler<GetOpenLoansByBranchQuery, ServiceResponse<List<LightLoanDto>>>
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOpenLoansByBranchQueryHandler> _logger;

        public GetOpenLoansByBranchQueryHandler(ILoanRepository loanRepository, IMapper mapper, ILogger<GetOpenLoansByBranchQueryHandler> logger)
        {
            _loanRepository = loanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<LightLoanDto>>> Handle(GetOpenLoansByBranchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.BranchId))
                {
                    return ServiceResponse<List<LightLoanDto>>.Return422("Invalid BranchId.");
                }

                var openLoans = await _loanRepository
                    .FindBy(x => x.BranchId == request.BranchId &&
                                 x.LoanStatus == LoanStatus.Open.ToString() &&
                                 x.DueAmount > 0)
                    .AsNoTracking()
                    .Select(x => new LightLoanDto
                    {
                        Id = x.Id,
                        LoanAmount = x.LoanAmount,
                        DueAmount = x.DueAmount,
                        Balance = x.Balance,
                        CustomerId = x.CustomerId,
                        LoanStatus = x.LoanStatus,
                        LoanType = x.LoanType,
                        IsUpload = x.IsUpload,
                        LoanCategory = x.LoanCategory,
                        AccrualInterest = x.AccrualInterest,
                        InterestRate = x.InterestRate,
                        Tax = x.Tax,
                        Principal = x.Principal,
                        TotalPrincipalPaid = x.TotalPrincipalPaid,
                        LoanDate = x.LoanDate,
                        MaturityDate = x.MaturityDate,
                        LoanApplication = x.LoanApplication != null
                            ? new LightLoanApplicationDto
                            {
                                Id = x.LoanApplication.Id,
                                LoanProduct = x.LoanApplication.LoanProduct != null
                                    ? new LightLoanProductDto
                                    {
                                        Id = x.LoanApplication.LoanProduct.Id,
                                        ProductCode = x.LoanApplication.LoanProduct.ProductCode,
                                        ProductName = x.LoanApplication.LoanProduct.ProductName,
                                        TargetType = x.LoanApplication.LoanProduct.TargetType
                                    }
                                    : null
                            }
                            : null
                    })
                    .ToListAsync(cancellationToken);

                return ServiceResponse<List<LightLoanDto>>.ReturnResultWith200(openLoans);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to retrieve open loans for BranchId {BranchId}: {ErrorMessage}", request.BranchId, e.Message);
                return ServiceResponse<List<LightLoanDto>>.Return500(e, "Failed to retrieve open loans");
            }
        }

    }
}
