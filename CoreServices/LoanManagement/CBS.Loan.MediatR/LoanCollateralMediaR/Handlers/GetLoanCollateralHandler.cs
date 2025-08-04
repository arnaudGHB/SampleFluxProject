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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanCollateralHandler : IRequestHandler<GetLoanCollateralQuery, ServiceResponse<LoanApplicationCollateralDto>>
    {
        private readonly ILoanCollateralRepository _LoanCollateralRepository; // Repository for accessing LoanCollateral data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanCollateralHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanCollateralQueryHandler.
        /// </summary>
        /// <param name="LoanCollateralRepository">Repository for LoanCollateral data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanCollateralHandler(
            ILoanCollateralRepository LoanCollateralRepository,
            IMapper mapper,
            ILogger<GetLoanCollateralHandler> logger)
        {
            _LoanCollateralRepository = LoanCollateralRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanCollateralQuery to retrieve a specific LoanCollateral.
        /// </summary>
        /// <param name="request">The GetLoanCollateralQuery containing LoanCollateral ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanApplicationCollateralDto>> Handle(GetLoanCollateralQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanCollateral entity with the specified ID from the repository
                var entity = await _LoanCollateralRepository.AllIncluding(x => x.LoanProductCollateral, x => x.LoanApplication).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the LoanCollateral entity to LoanCollateralDto and return it with a success response
                    var LoanCollateralDto = _mapper.Map<LoanApplicationCollateralDto>(entity);
                    return ServiceResponse<LoanApplicationCollateralDto>.ReturnResultWith200(LoanCollateralDto);
                }
                else
                {
                    // If the LoanCollateral entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanCollateral not found.");
                    return ServiceResponse<LoanApplicationCollateralDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanCollateral: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanApplicationCollateralDto>.Return500(e);
            }
        }
    }

}
