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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanProductCollateralHandler : IRequestHandler<GetLoanProductCollateralQuery, ServiceResponse<LoanProductCollateralDto>>
    {
        private readonly ILoanProductCollateralRepository _LoanProductCollateralRepository; // Repository for accessing LoanProductCollateral data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductCollateralHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanProductCollateralQueryHandler.
        /// </summary>
        /// <param name="LoanProductCollateralRepository">Repository for LoanProductCollateral data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductCollateralHandler(
            ILoanProductCollateralRepository LoanProductCollateralRepository,
            IMapper mapper,
            ILogger<GetLoanProductCollateralHandler> logger)
        {
            _LoanProductCollateralRepository = LoanProductCollateralRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductCollateralQuery to retrieve a specific LoanProductCollateral.
        /// </summary>
        /// <param name="request">The GetLoanProductCollateralQuery containing LoanProductCollateral ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductCollateralDto>> Handle(GetLoanProductCollateralQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProductCollateral entity with the specified ID from the repository
                var entity = await _LoanProductCollateralRepository.AllIncluding(x=>x.Collateral,x=>x.LoanProduct).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the LoanProductCollateral entity to LoanProductCollateralDto and return it with a success response
                    var LoanProductCollateralDto = _mapper.Map<LoanProductCollateralDto>(entity);
                    return ServiceResponse<LoanProductCollateralDto>.ReturnResultWith200(LoanProductCollateralDto);
                }
                else
                {
                    // If the LoanProductCollateral entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanProductCollateral not found.");
                    return ServiceResponse<LoanProductCollateralDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProductCollateral: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductCollateralDto>.Return500(e);
            }
        }
    }

}
