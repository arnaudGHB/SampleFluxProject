using AutoMapper;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Queries;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeValidationP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanCommiteeValidationHistoryHandler : IRequestHandler<GetAllLoanCommiteeValidationHistoryQuery, ServiceResponse<List<LoanCommiteeGroupDto>>>
    {
        private readonly ILoanCommiteeValidationHistoryRepository _LoanCommiteeValidationRepository; // Repository for accessing LoanCommiteeValidations data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanCommiteeValidationHistoryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanCommiteeValidationQueryHandler.
        /// </summary>
        /// <param name="LoanCommiteeValidationRepository">Repository for LoanCommiteeValidations data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanCommiteeValidationHistoryHandler(
            ILoanCommiteeValidationHistoryRepository LoanCommiteeValidationRepository,
            IMapper mapper, ILogger<GetAllLoanCommiteeValidationHistoryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanCommiteeValidationRepository = LoanCommiteeValidationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanCommiteeValidationQuery to retrieve all LoanCommiteeValidations.
        /// </summary>
        /// <param name="request">The GetAllLoanCommiteeValidationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanCommiteeGroupDto>>> Handle(GetAllLoanCommiteeValidationHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanCommiteeValidations entities from the repository
                var entities = await _LoanCommiteeValidationRepository.All.ToListAsync();
                return ServiceResponse<List<LoanCommiteeGroupDto>>.ReturnResultWith200(_mapper.Map<List<LoanCommiteeGroupDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanCommiteeValidations: {e.Message}");
                return ServiceResponse<List<LoanCommiteeGroupDto>>.Return500(e, "Failed to get all LoanCommiteeValidations");
            }
        }
    }
}
