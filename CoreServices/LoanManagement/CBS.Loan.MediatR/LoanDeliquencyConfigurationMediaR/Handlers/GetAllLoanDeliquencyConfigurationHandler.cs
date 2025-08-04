using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanDeliquencyConfigurationHandler : IRequestHandler<GetAllLoanDeliquencyConfigurationQuery, ServiceResponse<List<LoanDeliquencyConfigurationDto>>>
    {
        private readonly ILoanDeliquencyConfigurationRepository _LoanDeliquencyConfigurationRepository; // Repository for accessing LoanDeliquencyConfigurations data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanDeliquencyConfigurationHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanDeliquencyConfigurationQueryHandler.
        /// </summary>
        /// <param name="LoanDeliquencyConfigurationRepository">Repository for LoanDeliquencyConfigurations data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanDeliquencyConfigurationHandler(
            ILoanDeliquencyConfigurationRepository LoanDeliquencyConfigurationRepository,
            IMapper mapper, ILogger<GetAllLoanDeliquencyConfigurationHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanDeliquencyConfigurationRepository = LoanDeliquencyConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanDeliquencyConfigurationQuery to retrieve all LoanDeliquencyConfigurations.
        /// </summary>
        /// <param name="request">The GetAllLoanDeliquencyConfigurationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanDeliquencyConfigurationDto>>> Handle(GetAllLoanDeliquencyConfigurationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanDeliquencyConfigurations entities from the repository
                var entities = await _LoanDeliquencyConfigurationRepository.All.ToListAsync();
                return ServiceResponse<List<LoanDeliquencyConfigurationDto>>.ReturnResultWith200(_mapper.Map<List<LoanDeliquencyConfigurationDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanDeliquencyConfigurations: {e.Message}");
                return ServiceResponse<List<LoanDeliquencyConfigurationDto>>.Return500(e, "Failed to get all LoanDeliquencyConfigurations");
            }
        }
    }
}
