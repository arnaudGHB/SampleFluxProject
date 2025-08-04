using AutoMapper;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.RefundMediaR.Queries;
using CBS.NLoan.Repository.RefundP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.RefundMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetRefundHandler : IRequestHandler<GetRefundQuery, ServiceResponse<RefundDto>>
    {
        private readonly IRefundRepository _RefundRepository; // Repository for accessing Refund data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetRefundHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetRefundQueryHandler.
        /// </summary>
        /// <param name="RefundRepository">Repository for Refund data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetRefundHandler(
            IRefundRepository RefundRepository,
            IMapper mapper,
            ILogger<GetRefundHandler> logger)
        {
            _RefundRepository = RefundRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetRefundQuery to retrieve a specific Refund.
        /// </summary>
        /// <param name="request">The GetRefundQuery containing Refund ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<RefundDto>> Handle(GetRefundQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Refund entity with the specified ID from the repository
                var entity = await _RefundRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the Refund entity to RefundDto and return it with a success response
                    var RefundDto = _mapper.Map<RefundDto>(entity);
                    return ServiceResponse<RefundDto>.ReturnResultWith200(RefundDto);
                }
                else
                {
                    // If the Refund entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Refund not found.");
                    return ServiceResponse<RefundDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Refund: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<RefundDto>.Return500(e);
            }
        }
    }

}
