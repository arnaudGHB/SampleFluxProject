using AutoMapper;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Queries;
using CBS.NLoan.Repository.FeeRangeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeRangeMediaR.FeeRangeRangeP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetFeeRangeHandler : IRequestHandler<GetFeeRangeQuery, ServiceResponse<FeeRangeDto>>
    {
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing FeeRange data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFeeRangeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetFeeRangeHandler.
        /// </summary>
        /// <param name="feeRangeRepository">Repository for FeeRange data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetFeeRangeHandler(
            IFeeRangeRepository feeRangeRepository,
            IMapper mapper,
            ILogger<GetFeeRangeHandler> logger)
        {
            _feeRangeRepository = feeRangeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetFeeRangeQuery to retrieve a specific FeeRange.
        /// </summary>
        /// <param name="request">The GetFeeRangeQuery containing FeeRange ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeeRangeDto>> Handle(GetFeeRangeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the FeeRange entity with the specified ID from the repository
                var entity = await _feeRangeRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.Fee).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the FeeRange entity to FeeRangeDto and return it with a success response
                    var feeRangeDto = _mapper.Map<FeeRangeDto>(entity);
                    return ServiceResponse<FeeRangeDto>.ReturnResultWith200(feeRangeDto);
                }
                else
                {
                    // If the FeeRange entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("FeeRange not found.");
                    return ServiceResponse<FeeRangeDto>.Return404();
                }
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError($"Error occurred while getting FeeRange: {e.Message}");
                return ServiceResponse<FeeRangeDto>.Return500(e);
            }
        }
    }

}
