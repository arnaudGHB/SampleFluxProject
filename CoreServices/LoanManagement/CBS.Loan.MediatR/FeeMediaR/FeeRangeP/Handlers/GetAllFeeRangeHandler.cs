using AutoMapper;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Queries;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Queries;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllFeeRangeHandler : IRequestHandler<GetAllFeeRangeQuery, ServiceResponse<List<FeeRangeDto>>>
    {
        private readonly IFeeRangeRepository _feeRangeRepository; // Repository for accessing Fees data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFeeRangeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllFeeRangeHandler.
        /// </summary>
        /// <param name="feeRangeRepository">Repository for Fees data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFeeRangeHandler(
            IFeeRangeRepository feeRangeRepository,
            IMapper mapper,
            ILogger<GetAllFeeRangeHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _feeRangeRepository = feeRangeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllFeeRangeQuery to retrieve all Fees.
        /// </summary>
        /// <param name="request">The GetAllFeeRangeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FeeRangeDto>>> Handle(GetAllFeeRangeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all FeeRange entities from the repository
                var entities = await _feeRangeRepository.All.Include(x=>x.Fee).Where(x => !x.IsDeleted).ToListAsync();
                var dtos = _mapper.Map<List<FeeRangeDto>>(entities);
                return ServiceResponse<List<FeeRangeDto>>.ReturnResultWith200(dtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Fee ranges: {e.Message}");
                return ServiceResponse<List<FeeRangeDto>>.Return500(e, "Failed to get all Fee ranges");
            }
        }
    }
}
