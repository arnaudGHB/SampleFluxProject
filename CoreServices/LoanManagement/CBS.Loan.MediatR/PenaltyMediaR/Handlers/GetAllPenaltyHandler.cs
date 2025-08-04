using AutoMapper;
using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.PenaltyMediaR.Queries;
using CBS.NLoan.Repository.PenaltyP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.PenaltyMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllPenaltyHandler : IRequestHandler<GetAllPenaltyQuery, ServiceResponse<List<PenaltyDto>>>
    {
        private readonly IPenaltyRepository _PenaltyRepository; // Repository for accessing Penaltys data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllPenaltyHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllPenaltyQueryHandler.
        /// </summary>
        /// <param name="PenaltyRepository">Repository for Penaltys data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllPenaltyHandler(
            IPenaltyRepository PenaltyRepository,
            IMapper mapper, ILogger<GetAllPenaltyHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _PenaltyRepository = PenaltyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllPenaltyQuery to retrieve all Penaltys.
        /// </summary>
        /// <param name="request">The GetAllPenaltyQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<PenaltyDto>>> Handle(GetAllPenaltyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all Penaltys entities from the repository
                var entities = await _PenaltyRepository.AllIncluding(x=>x.LoanProduct).Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<PenaltyDto>>.ReturnResultWith200(_mapper.Map<List<PenaltyDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Penaltys: {e.Message}");
                return ServiceResponse<List<PenaltyDto>>.Return500(e, "Failed to get all Penaltys");
            }
        }
    }
}
