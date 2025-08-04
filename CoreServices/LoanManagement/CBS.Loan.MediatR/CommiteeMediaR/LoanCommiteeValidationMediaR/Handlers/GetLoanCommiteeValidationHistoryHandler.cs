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
    /// Handles the request to retrieve a specific Loan based on its unique identifier.
    /// </summary>
    public class GetLoanCommiteeValidationHistoryHandler : IRequestHandler<GetLoanCommiteeValidationHistoryQuery, ServiceResponse<LoanCommiteeGroupDto>>
    {
        private readonly ILoanCommiteeValidationHistoryRepository _LoanCommiteeValidationRepository; // Repository for accessing LoanCommiteeValidation data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetLoanCommiteeValidationHistoryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetLoanCommiteeValidationQueryHandler.
        /// </summary>
        /// <param name="LoanCommiteeValidationRepository">Repository for LoanCommiteeValidation data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetLoanCommiteeValidationHistoryHandler(
            ILoanCommiteeValidationHistoryRepository LoanCommiteeValidationRepository,
            IMapper mapper,
            ILogger<GetLoanCommiteeValidationHistoryHandler> logger)
        {
            _LoanCommiteeValidationRepository = LoanCommiteeValidationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetLoanCommiteeValidationQuery to retrieve a specific LoanCommiteeValidation.
        /// </summary>
        /// <param name="request">The GetLoanCommiteeValidationQuery containing LoanCommiteeValidation ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<LoanCommiteeGroupDto>> Handle(GetLoanCommiteeValidationHistoryQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the LoanCommiteeValidation entity with the specified ID from the repository
                var entity = await _LoanCommiteeValidationRepository.AllIncluding(x=>x.LoanApplication,x=>x.LoanCommiteeMember).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the LoanCommiteeValidation entity to LoanCommiteeValidationDto and return it with a success response
                    var LoanCommiteeValidationDto = _mapper.Map<LoanCommiteeGroupDto>(entity);
                    return ServiceResponse<LoanCommiteeGroupDto>.ReturnResultWith200(LoanCommiteeValidationDto);
                }
                else
                {
                    // If the LoanCommiteeValidation entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("LoanCommiteeValidation not found.");
                    return ServiceResponse<LoanCommiteeGroupDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting LoanCommiteeValidation: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanCommiteeGroupDto>.Return500(e);
            }
        }
    }

}
