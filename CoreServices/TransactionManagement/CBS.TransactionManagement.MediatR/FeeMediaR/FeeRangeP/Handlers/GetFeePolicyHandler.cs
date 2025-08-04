using AutoMapper;
using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Data.Dto.FeeP;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.FeeP.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.FeePolicyMediaR.FeePolicyRangeP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetFeePolicyHandler : IRequestHandler<GetFeePolicyQuery, ServiceResponse<FeePolicyDto>>
    {
        private readonly IFeePolicyRepository _FeePolicyRepository; // Repository for accessing FeePolicy data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetFeePolicyHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetFeePolicyHandler.
        /// </summary>
        /// <param name="FeePolicyRepository">Repository for FeePolicy data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetFeePolicyHandler(
            IFeePolicyRepository FeePolicyRepository,
            IMapper mapper,
            ILogger<GetFeePolicyHandler> logger)
        {
            _FeePolicyRepository = FeePolicyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetFeePolicyQuery to retrieve a specific FeePolicy.
        /// </summary>
        /// <param name="request">The GetFeePolicyQuery containing FeePolicy ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<FeePolicyDto>> Handle(GetFeePolicyQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the FeePolicy entity with the specified ID from the repository
                var entity = await _FeePolicyRepository.FindBy(x=>x.Id==request.Id).Include(x=>x.Fee).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the FeePolicy entity to FeePolicyDto and return it with a success response
                    var FeePolicyDto = _mapper.Map<FeePolicyDto>(entity);
                    return ServiceResponse<FeePolicyDto>.ReturnResultWith200(FeePolicyDto);
                }
                else
                {
                    // If the FeePolicy entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("FeePolicy not found.");
                    return ServiceResponse<FeePolicyDto>.Return404();
                }
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError($"Error occurred while getting FeePolicy: {e.Message}");
                return ServiceResponse<FeePolicyDto>.Return500(e);
            }
        }
    }

}
