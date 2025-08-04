using AutoMapper;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RefundMediaR.Queries;
using CBS.NLoan.Repository.RefundP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllRefundHandler : IRequestHandler<GetAllRefundQuery, ServiceResponse<List<RefundDto>>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refunds data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllRefundHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllRefundQueryHandler.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refunds data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllRefundHandler(
            IRefundRepository RefundRepository,
            IMapper mapper, ILogger<GetAllRefundHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _RefundRepository = RefundRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllRefundQuery to retrieve all Refunds.
        /// </summary>
        /// <param name="request">The GetAllRefundQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<RefundDto>>> Handle(GetAllRefundQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Refunds entities from the repository
                var entities = await _RefundRepository.All.ToListAsync();
                return ServiceResponse<List<RefundDto>>.ReturnResultWith200(_mapper.Map<List<RefundDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Refunds: {e.Message}");
                return ServiceResponse<List<RefundDto>>.Return500(e, "Failed to get all Refunds");
            }
        }
    }
}
