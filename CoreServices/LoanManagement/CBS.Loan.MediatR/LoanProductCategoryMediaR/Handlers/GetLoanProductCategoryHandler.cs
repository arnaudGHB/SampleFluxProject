using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CycleNameMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCycleP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanProductCategoryHandler : IRequestHandler<GetLoanProductCategoryQuery, ServiceResponse<LoanProductCategoryDto>>
    {
        private readonly ILoanProductCategoryRepository _LoanCycleRepository; // Repository for accessing LoanProductCategory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanProductCategoryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanCycleQueryHandler.
        /// </summary>
        /// <param name="LoanCycleRepository">Repository for LoanProductCategory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanProductCategoryHandler(
            ILoanProductCategoryRepository LoanCycleRepository,
            IMapper mapper,
            ILogger<GetLoanProductCategoryHandler> logger)
        {
            _LoanCycleRepository = LoanCycleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanProductCategoryQuery to retrieve a specific LoanProductCategory.
        /// </summary>
        /// <param name="request">The GetLoanProductCategoryQuery containing LoanProductCategory ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanProductCategoryDto>> Handle(GetLoanProductCategoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanProductCategory entity with the specified ID from the repository
                var entity = await _LoanCycleRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanProductCategory entity to LoanProductCategoryDto and return it with a success response
                    var LoanCycleDto = _mapper.Map<LoanProductCategoryDto>(entity);
                    return ServiceResponse<LoanProductCategoryDto>.ReturnResultWith200(LoanCycleDto);
                }
                else
                {
                    // If the LoanProductCategory entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanProductCategory not found.");
                    return ServiceResponse<LoanProductCategoryDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanProductCategory: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductCategoryDto>.Return500(e);
            }
        }
    }

}
