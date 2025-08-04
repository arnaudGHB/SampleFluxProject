using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanProductQueryHandler : IRequestHandler<GetLoanProductQuery, ServiceResponse<LoanProductDto>>
    {
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing LoanProduct data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanProductQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProduct data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductQueryHandler(
            ILoanProductRepository LoanProductRepository,
            IMapper mapper,
            ILogger<GetLoanProductQueryHandler> logger)
        {
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductQuery to retrieve a specific LoanProduct.
        /// </summary>
        /// <param name="request">The GetLoanProductQuery containing LoanProduct ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductDto>> Handle(GetLoanProductQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProduct entity with the specified ID from the repository
                var entity =await _LoanProductRepository.AllIncluding(x => x.LoanProductCollaterals
                , c => c.Penalties.Where(x=>!x.IsDeleted), b => b.LoanProductMaturityPeriodExtensions, b => b.LoanProductRepaymentCycles,
                b=>b.LoanProductCategory, b => b.LoanTerm).FirstOrDefaultAsync(x=>x.Id==request.LoanProductId);

                if (entity != null)
                {
                    // Map the LoanProduct entity to LoanProductDto and return it with a success response
                    var LoanProductDto = _mapper.Map<LoanProductDto>(entity);
                    LoanProductDto.RepaymentCycles = entity.LoanProductRepaymentCycles.Select(item => item.RepaymentCycle).ToList();
                    //LoanProductDto.RefundOrders = entity.LoanProductRepaymentOrders.Select(item => item.RepaymentTypeName).ToList();
                    if (LoanProductDto.LoanTerm==null)
                    {
                        LoanProductDto.LoanTerm = new Data.Entity.LoanTermP.LoanTerm { Name="N/A"};
                    }
                    else
                    {
                        LoanProductDto.MinimumDurationPeriod = LoanProductDto.LoanTerm.MinInMonth;
                        LoanProductDto.MaximumDurationPeriod = LoanProductDto.LoanTerm.MaxInMonth;
                    }
                    if (LoanProductDto.LoanProductCategory == null)
                    {
                        LoanProductDto.LoanProductCategory = new Data.Entity.LoanApplicationP.LoanProductCategory { Name = "N/A" };
                    }
                    return ServiceResponse<LoanProductDto>.ReturnResultWith200(LoanProductDto);
                }
                else
                {
                    // If the LoanProduct entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanProduct not found.");
                    return ServiceResponse<LoanProductDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProduct: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductDto>.Return500(e);
            }
        }
    }

}
