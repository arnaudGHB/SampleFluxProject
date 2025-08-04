using AutoMapper;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Queries;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeGroupMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanCommiteeGroupHandler : IRequestHandler<GetAllLoanCommiteeGroupQuery, ServiceResponse<List<LoanCommiteeGroupDto>>>
    {
        private readonly ILoanCommiteeGroupRepository _LoanCommiteeGroupRepository; // Repository for accessing LoanCommiteeGroups data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanCommiteeGroupHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanCommiteeGroupQueryHandler.
        /// </summary>
        /// <param name="LoanCommiteeGroupRepository">Repository for LoanCommiteeGroups data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanCommiteeGroupHandler(
            ILoanCommiteeGroupRepository LoanCommiteeGroupRepository,
            IMapper mapper, ILogger<GetAllLoanCommiteeGroupHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanCommiteeGroupRepository = LoanCommiteeGroupRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanCommiteeGroupQuery to retrieve all LoanCommiteeGroups.
        /// </summary>
        /// <param name="request">The GetAllLoanCommiteeGroupQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanCommiteeGroupDto>>> Handle(GetAllLoanCommiteeGroupQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanCommiteeGroups entities from the repository
                var entities = await _LoanCommiteeGroupRepository.AllIncluding(x=>x.LoanCommiteeMembers).Where(x => !x.IsDeleted).ToListAsync();
                return ServiceResponse<List<LoanCommiteeGroupDto>>.ReturnResultWith200(_mapper.Map<List<LoanCommiteeGroupDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanCommiteeGroups: {e.Message}");
                return ServiceResponse<List<LoanCommiteeGroupDto>>.Return500(e, "Failed to get all LoanCommiteeGroups");
            }
        }
    }
}
