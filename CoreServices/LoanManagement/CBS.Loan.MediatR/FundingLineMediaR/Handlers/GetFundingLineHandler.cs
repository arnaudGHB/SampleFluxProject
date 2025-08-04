using AutoMapper;
using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.FundingLineMediaR.Queries;
using CBS.NLoan.Repository.FundingLineP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FundingLineMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetFundingLineHandler : IRequestHandler<GetFundingLineQuery, ServiceResponse<FundingLineDto>>
    {
        private readonly IFundingLineRepository _FundingLineRepository; // Repository for accessing FundingLine data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFundingLineHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetFundingLineQueryHandler.
        /// </summary>
        /// <param name="FundingLineRepository">Repository for FundingLine data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetFundingLineHandler(
            IFundingLineRepository FundingLineRepository,
            IMapper mapper,
            ILogger<GetFundingLineHandler> logger)
        {
            _FundingLineRepository = FundingLineRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetFundingLineQuery to retrieve a specific FundingLine.
        /// </summary>
        /// <param name="request">The GetFundingLineQuery containing FundingLine ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FundingLineDto>> Handle(GetFundingLineQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the FundingLine entity with the specified ID from the repository
                var entity = await _FundingLineRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the FundingLine entity to FundingLineDto and return it with a success response
                    var FundingLineDto = _mapper.Map<FundingLineDto>(entity);
                    return ServiceResponse<FundingLineDto>.ReturnResultWith200(FundingLineDto);
                }
                else
                {
                    // If the FundingLine entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("FundingLine not found.");
                    return ServiceResponse<FundingLineDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting FundingLine: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<FundingLineDto>.Return500(e);
            }
        }
    }

}
