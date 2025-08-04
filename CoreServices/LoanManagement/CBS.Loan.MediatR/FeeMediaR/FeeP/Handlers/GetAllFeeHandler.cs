using AutoMapper;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Queries;
using CBS.NLoan.Repository.FeeP.FeeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeeMediaR.FeeP.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllFeeHandler : IRequestHandler<GetAllFeeQuery, ServiceResponse<List<FeeDto>>>
    {
        private readonly IFeeRepository _FeeRepository; // Repository for accessing Fees data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllFeeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllFeeQueryHandler.
        /// </summary>
        /// <param name="FeeRepository">Repository for Fees data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllFeeHandler(
            IFeeRepository FeeRepository,
            IMapper mapper, ILogger<GetAllFeeHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _FeeRepository = FeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllFeeQuery to retrieve all Fees.
        /// </summary>
        /// <param name="request">The GetAllFeeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<FeeDto>>> Handle(GetAllFeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Fees entities from the repository
                var entities = await _FeeRepository.All.Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<FeeDto>>.ReturnResultWith200(_mapper.Map<List<FeeDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Fees: {e.Message}");
                return ServiceResponse<List<FeeDto>>.Return500(e, "Failed to get all Fees");
            }
        }
    }
}
