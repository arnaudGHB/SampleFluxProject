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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetFeeHandler : IRequestHandler<GetFeeQuery, ServiceResponse<FeeDto>>
    {
        private readonly IFeeRepository _FeeRepository; // Repository for accessing Fee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFeeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetFeeQueryHandler.
        /// </summary>
        /// <param name="FeeRepository">Repository for Fee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetFeeHandler(
            IFeeRepository FeeRepository,
            IMapper mapper,
            ILogger<GetFeeHandler> logger)
        {
            _FeeRepository = FeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetFeeQuery to retrieve a specific Fee.
        /// </summary>
        /// <param name="request">The GetFeeQuery containing Fee ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeeDto>> Handle(GetFeeQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Fee entity with the specified ID from the repository
                var entity = await _FeeRepository.FindBy(x => x.Id == request.Id).Include(x => x.FeeRanges).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the Fee entity to FeeDto and return it with a success response
                    var FeeDto = _mapper.Map<FeeDto>(entity);
                    return ServiceResponse<FeeDto>.ReturnResultWith200(FeeDto);
                }
                else
                {
                    // If the Fee entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Fee not found.");
                    return ServiceResponse<FeeDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Fee: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<FeeDto>.Return500(e);
            }
        }
    }

}
