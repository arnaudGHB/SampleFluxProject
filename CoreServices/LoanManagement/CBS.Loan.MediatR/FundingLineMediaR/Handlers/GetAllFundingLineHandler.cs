using AutoMapper;
using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FundingLineMediaR.Queries;
using CBS.NLoan.Repository.FundingLineP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllFundingLineHandler : IRequestHandler<GetAllFundingLineQuery, ServiceResponse<List<FundingLineDto>>>
    {
        private readonly IFundingLineRepository _FundingLineRepository; // Repository for accessing FundingLines data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFundingLineHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllFundingLineQueryHandler.
        /// </summary>
        /// <param name="FundingLineRepository">Repository for FundingLines data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFundingLineHandler(
            IFundingLineRepository FundingLineRepository,
            IMapper mapper, ILogger<GetAllFundingLineHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _FundingLineRepository = FundingLineRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllFundingLineQuery to retrieve all FundingLines.
        /// </summary>
        /// <param name="request">The GetAllFundingLineQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FundingLineDto>>> Handle(GetAllFundingLineQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all FundingLines entities from the repository
                var entities = await _FundingLineRepository.All.ToListAsync();
                return ServiceResponse<List<FundingLineDto>>.ReturnResultWith200(_mapper.Map<List<FundingLineDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all FundingLines: {e.Message}");
                return ServiceResponse<List<FundingLineDto>>.Return500(e, "Failed to get all FundingLines");
            }
        }
    }
}
