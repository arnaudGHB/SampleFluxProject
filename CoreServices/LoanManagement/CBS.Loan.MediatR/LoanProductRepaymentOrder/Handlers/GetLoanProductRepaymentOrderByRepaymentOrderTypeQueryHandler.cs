using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve loan product repayment orders by RepaymentOrderType.
    /// </summary>
    public class GetLoanProductRepaymentOrderByRepaymentOrderTypeQueryHandler : IRequestHandler<GetLoanProductRepaymentOrderByRepaymentOrderTypeQuery, ServiceResponse<LoanProductRepaymentOrderDto>>
    {
        private readonly ILoanProductRepaymentOrderRepository _LoanProductRepository; // Repository for accessing LoanProduct data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductRepaymentOrderByRepaymentOrderTypeQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanProductRepaymentOrderByRepaymentOrderTypeQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProduct data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductRepaymentOrderByRepaymentOrderTypeQueryHandler(
            ILoanProductRepaymentOrderRepository LoanProductRepository,
            IMapper mapper,
            ILogger<GetLoanProductRepaymentOrderByRepaymentOrderTypeQueryHandler> logger)
        {
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductRepaymentOrderByRepaymentOrderTypeQuery to retrieve loan product repayment orders by product ID.
        /// </summary>
        /// <param name="request">The GetLoanProductRepaymentOrderByRepaymentOrderTypeQuery containing the product ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductRepaymentOrderDto>> Handle(GetLoanProductRepaymentOrderByRepaymentOrderTypeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProductRepaymentOrder entities with the specified product ID from the repository
                var entities =  _LoanProductRepository.FindBy(x => x.LoanProductRepaymentOrderType == request.LoanProductRepaymentOrderType).FirstOrDefault();
                if (entities != null)
                {
                    // Map the LoanProductRepaymentOrder entities to LoanProductRepaymentOrderDto and return them with a success response
                    var LoanProductRepaymentOrderDtos = _mapper.Map<LoanProductRepaymentOrderDto>(entities);
                    return ServiceResponse<LoanProductRepaymentOrderDto>.ReturnResultWith200(LoanProductRepaymentOrderDtos);
                }
                else
                {
                    // If the LoanProductRepaymentOrder entities were not found, log the error and return a 404 Not Found response
                    var LoanProductRepaymentOrderDtos = _mapper.Map<LoanProductRepaymentOrderDto>(new LoanProductRepaymentOrder());
                    return ServiceResponse<LoanProductRepaymentOrderDto>.ReturnResultWith200(LoanProductRepaymentOrderDtos);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProductRepaymentOrders: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductRepaymentOrderDto>.Return500(e);
            }
        }
    }

}
