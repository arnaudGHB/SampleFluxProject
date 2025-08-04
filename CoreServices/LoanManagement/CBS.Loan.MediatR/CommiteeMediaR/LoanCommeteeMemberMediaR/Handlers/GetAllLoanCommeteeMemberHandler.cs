using AutoMapper;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Queries;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanCommeteeMemberQueryHandler : IRequestHandler<GetAllLoanCommeteeMemberQuery, ServiceResponse<List<LoanCommiteeMemberDto>>>
    {
        private readonly ILoanCommeteeMemberRepository _LoanCommeteeMemberRepository; // Repository for accessing LoanCommeteeMembers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanCommeteeMemberQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanCommeteeMemberQueryHandler.
        /// </summary>
        /// <param name="LoanCommeteeMemberRepository">Repository for LoanCommeteeMembers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanCommeteeMemberQueryHandler(
            ILoanCommeteeMemberRepository LoanCommeteeMemberRepository,
            IMapper mapper, ILogger<GetAllLoanCommeteeMemberQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanCommeteeMemberRepository = LoanCommeteeMemberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanCommeteeMemberQuery to retrieve all LoanCommeteeMembers.
        /// </summary>
        /// <param name="request">The GetAllLoanCommeteeMemberQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanCommiteeMemberDto>>> Handle(GetAllLoanCommeteeMemberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanCommeteeMembers entities from the repository
                var entities = await _LoanCommeteeMemberRepository.AllIncluding(x=>x.LoanCommiteeGroup,x=>x.LoanCommiteeValidationHistories).Where(x=>!x.IsDeleted).ToListAsync();
                List<LoanCommiteeMemberDto> loanCommiteeMembers = new List<LoanCommiteeMemberDto>();
                var loanMemberDto = _mapper.Map<List<LoanCommiteeMemberDto>>(entities);
                return ServiceResponse<List<LoanCommiteeMemberDto>>.ReturnResultWith200(_mapper.Map<List<LoanCommiteeMemberDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanCommeteeMembers: {e.Message}");
                return ServiceResponse<List<LoanCommiteeMemberDto>>.Return500(e, "Failed to get all LoanCommeteeMembers");
            }
        }
    }
}
