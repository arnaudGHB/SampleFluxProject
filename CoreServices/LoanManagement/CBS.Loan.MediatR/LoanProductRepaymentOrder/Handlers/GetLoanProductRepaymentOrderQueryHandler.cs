using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanProductRepaymentOrderQueryHandler : IRequestHandler<GetLoanProductRepaymentOrderQuery, ServiceResponse<LoanProductRepaymentOrderDto>>
    {
        private readonly ILoanProductRepaymentOrderRepository _LoanProductRepaymentOrderRepository; // Repository for accessing LoanProductRepaymentOrder data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductRepaymentOrderQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanProductRepaymentOrderQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepaymentOrderRepository">Repository for LoanProductRepaymentOrder data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductRepaymentOrderQueryHandler(
            ILoanProductRepaymentOrderRepository LoanProductRepaymentOrderRepository,
            IMapper mapper,
            ILogger<GetLoanProductRepaymentOrderQueryHandler> logger)
        {
            _LoanProductRepaymentOrderRepository = LoanProductRepaymentOrderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductRepaymentOrderQuery to retrieve a specific LoanProductRepaymentOrder.
        /// </summary>
        /// <param name="request">The GetLoanProductRepaymentOrderQuery containing LoanProductRepaymentOrder ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductRepaymentOrderDto>> Handle(GetLoanProductRepaymentOrderQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProductRepaymentOrder entity with the specified ID from the repository
                var entity = await _LoanProductRepaymentOrderRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanProductRepaymentOrder entity to LoanProductRepaymentOrderDto and return it with a success response
                    var LoanProductRepaymentOrderDto = _mapper.Map<LoanProductRepaymentOrderDto>(entity);
                    return ServiceResponse<LoanProductRepaymentOrderDto>.ReturnResultWith200(LoanProductRepaymentOrderDto);
                }
                else
                {
                    // If the LoanProductRepaymentOrder entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanProductRepaymentOrder not found.");
                    return ServiceResponse<LoanProductRepaymentOrderDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProductRepaymentOrder: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductRepaymentOrderDto>.Return500(e);
            }
        }
    }

}
