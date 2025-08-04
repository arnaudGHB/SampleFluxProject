using AutoMapper;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductCollateralMediaR.Queries;
using CBS.NLoan.Repository.CollateralP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductCollateralMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanProductCollateralHandler : IRequestHandler<GetAllLoanProductCollateralQuery, ServiceResponse<List<LoanProductCollateralDto>>>
    {
        private readonly ILoanProductCollateralRepository _LoanProductCollateralRepository; // Repository for accessing LoanProductCollaterals data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanProductCollateralHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanProductCollateralQueryHandler.
        /// </summary>
        /// <param name="LoanProductCollateralRepository">Repository for LoanProductCollaterals data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanProductCollateralHandler(
            ILoanProductCollateralRepository LoanProductCollateralRepository,
            IMapper mapper, ILogger<GetAllLoanProductCollateralHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanProductCollateralRepository = LoanProductCollateralRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanProductCollateralQuery to retrieve all LoanProductCollaterals.
        /// </summary>
        /// <param name="request">The GetAllLoanProductCollateralQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanProductCollateralDto>>> Handle(GetAllLoanProductCollateralQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanProductCollaterals entities from the repository
                var entities = await _LoanProductCollateralRepository.AllIncluding(x=>x.LoanProduct,x=>x.Collateral).Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<LoanProductCollateralDto>>.ReturnResultWith200(_mapper.Map<List<LoanProductCollateralDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanProductCollaterals: {e.Message}");
                return ServiceResponse<List<LoanProductCollateralDto>>.Return500(e, "Failed to get all LoanProductCollaterals");
            }
        }
    }
}
