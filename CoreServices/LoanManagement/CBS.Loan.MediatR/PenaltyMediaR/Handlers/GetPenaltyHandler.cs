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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetPenaltyHandler : IRequestHandler<GetPenaltyQuery, ServiceResponse<PenaltyDto>>
    {
        private readonly IPenaltyRepository _PenaltyRepository; // Repository for accessing Penalty data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetPenaltyHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetPenaltyQueryHandler.
        /// </summary>
        /// <param name="PenaltyRepository">Repository for Penalty data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetPenaltyHandler(
            IPenaltyRepository PenaltyRepository,
            IMapper mapper,
            ILogger<GetPenaltyHandler> logger)
        {
            _PenaltyRepository = PenaltyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetPenaltyQuery to retrieve a specific Penalty.
        /// </summary>
        /// <param name="request">The GetPenaltyQuery containing Penalty ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<PenaltyDto>> Handle(GetPenaltyQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Penalty entity with the specified ID from the repository
                var entity = await _PenaltyRepository.AllIncluding(x=>x.LoanProduct).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the Penalty entity to PenaltyDto and return it with a success response
                    var PenaltyDto = _mapper.Map<PenaltyDto>(entity);
                    return ServiceResponse<PenaltyDto>.ReturnResultWith200(PenaltyDto);
                }
                else
                {
                    // If the Penalty entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Penalty not found.");
                    return ServiceResponse<PenaltyDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Penalty: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<PenaltyDto>.Return500(e);
            }
        }
    }

}
