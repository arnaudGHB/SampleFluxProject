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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanCommiteeGroupHandler : IRequestHandler<GetLoanCommiteeGroupQuery, ServiceResponse<LoanCommiteeGroupDto>>
    {
        private readonly ILoanCommiteeGroupRepository _LoanCommiteeGroupRepository; // Repository for accessing LoanCommiteeGroup data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanCommiteeGroupHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanCommiteeGroupQueryHandler.
        /// </summary>
        /// <param name="LoanCommiteeGroupRepository">Repository for LoanCommiteeGroup data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanCommiteeGroupHandler(
            ILoanCommiteeGroupRepository LoanCommiteeGroupRepository,
            IMapper mapper,
            ILogger<GetLoanCommiteeGroupHandler> logger)
        {
            _LoanCommiteeGroupRepository = LoanCommiteeGroupRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanCommiteeGroupQuery to retrieve a specific LoanCommiteeGroup.
        /// </summary>
        /// <param name="request">The GetLoanCommiteeGroupQuery containing LoanCommiteeGroup ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeGroupDto>> Handle(GetLoanCommiteeGroupQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanCommiteeGroup entity with the specified ID from the repository
                var entity = await _LoanCommiteeGroupRepository.AllIncluding(x=>x.LoanCommiteeMembers).Where(x=>!x.IsDeleted).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the LoanCommiteeGroup entity to LoanCommiteeGroupDto and return it with a success response
                    var LoanCommiteeGroupDto = _mapper.Map<LoanCommiteeGroupDto>(entity);
                    return ServiceResponse<LoanCommiteeGroupDto>.ReturnResultWith200(LoanCommiteeGroupDto);
                }
                else
                {
                    // If the LoanCommiteeGroup entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanCommiteeGroup not found.");
                    return ServiceResponse<LoanCommiteeGroupDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanCommiteeGroup: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeGroupDto>.Return500(e);
            }
        }
    }

}
