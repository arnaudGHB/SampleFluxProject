using AutoMapper;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Queries;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLaonApplicationCollateralByApplicationIdQueryHandler : IRequestHandler<GetAllLaonApplicationCollateralByApplicationIdQuery, ServiceResponse<List<LoanApplicationCollateralDto>>>
    {
        private readonly ILoanCollateralRepository _LoanCollateralRepository; // Repository for accessing LoanCollaterals data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLaonApplicationCollateralByApplicationIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLaonApplicationCollateralByApplicationIdQueryHandler.
        /// </summary>
        /// <param name="LoanCollateralRepository">Repository for LoanCollaterals data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLaonApplicationCollateralByApplicationIdQueryHandler(
            ILoanCollateralRepository LoanCollateralRepository,
            IMapper mapper, ILogger<GetAllLaonApplicationCollateralByApplicationIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanCollateralRepository = LoanCollateralRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLaonApplicationCollateralByApplicationIdQuery to retrieve all LoanCollaterals.
        /// </summary>
        /// <param name="request">The GetAllLaonApplicationCollateralByApplicationIdQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanApplicationCollateralDto>>> Handle(GetAllLaonApplicationCollateralByApplicationIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanCollaterals entities from the repository
                var entities = await _LoanCollateralRepository.AllIncluding(x=>x.LoanProductCollateral, x=>x.LoanApplication).Where(x=>x.LoanApplicationId==request.loanApplicationId).ToListAsync();
                return ServiceResponse<List<LoanApplicationCollateralDto>>.ReturnResultWith200(_mapper.Map<List<LoanApplicationCollateralDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanCollaterals: {e.Message}");
                return ServiceResponse<List<LoanApplicationCollateralDto>>.Return500(e, "Failed to get all LoanCollaterals");
            }
        }
    }
}
