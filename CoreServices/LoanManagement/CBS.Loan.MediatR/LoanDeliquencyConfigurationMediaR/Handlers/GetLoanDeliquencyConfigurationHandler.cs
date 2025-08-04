using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanDeliquencyConfigurationHandler : IRequestHandler<GetLoanDeliquencyConfigurationQuery, ServiceResponse<LoanDeliquencyConfigurationDto>>
    {
        private readonly ILoanDeliquencyConfigurationRepository _LoanDeliquencyConfigurationRepository; // Repository for accessing LoanDeliquencyConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanDeliquencyConfigurationHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanDeliquencyConfigurationQueryHandler.
        /// </summary>
        /// <param name="LoanDeliquencyConfigurationRepository">Repository for LoanDeliquencyConfiguration data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanDeliquencyConfigurationHandler(
            ILoanDeliquencyConfigurationRepository LoanDeliquencyConfigurationRepository,
            IMapper mapper,
            ILogger<GetLoanDeliquencyConfigurationHandler> logger)
        {
            _LoanDeliquencyConfigurationRepository = LoanDeliquencyConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanDeliquencyConfigurationQuery to retrieve a specific LoanDeliquencyConfiguration.
        /// </summary>
        /// <param name="request">The GetLoanDeliquencyConfigurationQuery containing LoanDeliquencyConfiguration ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanDeliquencyConfigurationDto>> Handle(GetLoanDeliquencyConfigurationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanDeliquencyConfiguration entity with the specified ID from the repository
                var entity = await _LoanDeliquencyConfigurationRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    // Map the LoanDeliquencyConfiguration entity to LoanDeliquencyConfigurationDto and return it with a success response
                    var LoanDeliquencyConfigurationDto = _mapper.Map<LoanDeliquencyConfigurationDto>(entity);
                    return ServiceResponse<LoanDeliquencyConfigurationDto>.ReturnResultWith200(LoanDeliquencyConfigurationDto);
                }
                else
                {
                    // If the LoanDeliquencyConfiguration entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanDeliquencyConfiguration not found.");
                    return ServiceResponse<LoanDeliquencyConfigurationDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanDeliquencyConfiguration: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanDeliquencyConfigurationDto>.Return500(e);
            }
        }
    }

}
