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
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllSavingProductFeeHandler : IRequestHandler<GetAllSavingProductFeeQuery, ServiceResponse<List<SavingProductFeeDto>>>
    {
        private readonly ISavingProductFeeRepository _SavingProductFeeRepository; // Repository for accessing SavingProductFees data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllSavingProductFeeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllSavingProductFeeHandler.
        /// </summary>
        /// <param name="SavingProductFeeRepository">Repository for SavingProductFees data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllSavingProductFeeHandler(
            ISavingProductFeeRepository SavingProductFeeRepository,
            IMapper mapper,
            ILogger<GetAllSavingProductFeeHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _SavingProductFeeRepository = SavingProductFeeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllSavingProductFeeQuery to retrieve all SavingProductFees.
        /// </summary>
        /// <param name="request">The GetAllSavingProductFeeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<SavingProductFeeDto>>> Handle(GetAllSavingProductFeeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all SavingProductFees entities from the repository
                var entities = await _SavingProductFeeRepository.All.Include(x=>x.Fee).Include(x => x.SavingProduct).Where(x => !x.IsDeleted).ToListAsync();
                return ServiceResponse<List<SavingProductFeeDto>>.ReturnResultWith200(_mapper.Map<List<SavingProductFeeDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all SavingProductFees: {e.Message}");
                return ServiceResponse<List<SavingProductFeeDto>>.Return500(e, "Failed to get all SavingProductFees");
            }
        }
    }
}
