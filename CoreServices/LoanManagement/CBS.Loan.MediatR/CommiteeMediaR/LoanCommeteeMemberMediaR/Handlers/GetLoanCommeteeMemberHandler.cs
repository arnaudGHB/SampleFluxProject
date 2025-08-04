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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanCommeteeMemberQueryHandler : IRequestHandler<GetLoanCommeteeMemberQuery, ServiceResponse<LoanCommiteeMemberDto>>
    {
        private readonly ILoanCommeteeMemberRepository _LoanCommeteeMemberRepository; // Repository for accessing LoanCommeteeMember data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanCommeteeMemberQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanCommeteeMemberQueryHandler.
        /// </summary>
        /// <param name="LoanCommeteeMemberRepository">Repository for LoanCommeteeMember data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanCommeteeMemberQueryHandler(
            ILoanCommeteeMemberRepository LoanCommeteeMemberRepository,
            IMapper mapper,
            ILogger<GetLoanCommeteeMemberQueryHandler> logger)
        {
            _LoanCommeteeMemberRepository = LoanCommeteeMemberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanCommeteeMemberQuery to retrieve a specific LoanCommeteeMember.
        /// </summary>
        /// <param name="request">The GetLoanCommeteeMemberQuery containing LoanCommeteeMember ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeMemberDto>> Handle(GetLoanCommeteeMemberQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanCommeteeMember entity with the specified ID from the repository
                var entity = await _LoanCommeteeMemberRepository.AllIncluding(x=>x.LoanCommiteeGroup,x=>x.LoanCommiteeValidationHistories).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the LoanCommeteeMember entity to LoanCommeteeMemberDto and return it with a success response
                    var LoanCommeteeMemberDto = _mapper.Map<LoanCommiteeMemberDto>(entity);
                    return ServiceResponse<LoanCommiteeMemberDto>.ReturnResultWith200(LoanCommeteeMemberDto);
                }
                else
                {
                    // If the LoanCommeteeMember entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanCommeteeMember not found.");
                    return ServiceResponse<LoanCommiteeMemberDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanCommeteeMember: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeMemberDto>.Return500(e);
            }
        }
    }

}
