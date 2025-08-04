using AutoMapper;
using CBS.TransactionManagement.Data.Dto.SavingProductFeeP;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SavingProductFeeP.Queries;
using CBS.TransactionManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.MediatR.SavingProductFeeP.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetSavingProductFeeHandler : IRequestHandler<GetSavingProductFeeQuery, ServiceResponse<SavingProductFeeDto>>
    {
        private readonly ISavingProductFeeRepository _SavingProductFeeRepository; // Repository for accessing SavingProductFee data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetSavingProductFeeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetSavingProductFeeHandler.
        /// </summary>
        /// <param name="SavingProductFeeRepository">Repository for SavingProductFee data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetSavingProductFeeHandler(
            ISavingProductFeeRepository SavingProductFeeRepository,
            IMapper mapper,
            ILogger<GetSavingProductFeeHandler> logger)
        {
            _SavingProductFeeRepository = SavingProductFeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetSavingProductFeeQuery to retrieve a specific SavingProductFee.
        /// </summary>
        /// <param name="request">The GetSavingProductFeeQuery containing SavingProductFee ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SavingProductFeeDto>> Handle(GetSavingProductFeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the SavingProductFee entity with the specified ID from the repository
                var entity = await _SavingProductFeeRepository.FindBy(x => x.Id == request.Id).Include(x => x.Fee).Include(x => x.SavingProduct).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Map the SavingProductFee entity to SavingProductFeeDto and return it with a success response
                    var SavingProductFeeDto = _mapper.Map<SavingProductFeeDto>(entity);
                    return ServiceResponse<SavingProductFeeDto>.ReturnResultWith200(SavingProductFeeDto);
                }
                else
                {
                    // If the SavingProductFee entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("SavingProductFee not found.");
                    return ServiceResponse<SavingProductFeeDto>.Return404();
                }
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                var errorMessage = $"Error occurred while getting SavingProductFee: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SavingProductFeeDto>.Return500(e);
            }
        }
    }

}
