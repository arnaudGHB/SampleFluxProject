using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanCommentryMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanCommentryHandler : IRequestHandler<GetAllLoanCommentryQuery, ServiceResponse<List<LoanCommentryDto>>>
    {
        private readonly ILoanCommentryRepository _LoanCommentryRepository; // Repository for accessing LoanCommentrys data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanCommentryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanCommentryQueryHandler.
        /// </summary>
        /// <param name="LoanCommentryRepository">Repository for LoanCommentrys data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanCommentryHandler(
            ILoanCommentryRepository LoanCommentryRepository,
            IMapper mapper, ILogger<GetAllLoanCommentryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanCommentryRepository = LoanCommentryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanCommentryQuery to retrieve all LoanCommentrys.
        /// </summary>
        /// <param name="request">The GetAllLoanCommentryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanCommentryDto>>> Handle(GetAllLoanCommentryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanCommentrys entities from the repository
                var entities = await _LoanCommentryRepository.All.ToListAsync();
                return ServiceResponse<List<LoanCommentryDto>>.ReturnResultWith200(_mapper.Map<List<LoanCommentryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanCommentrys: {e.Message}");
                return ServiceResponse<List<LoanCommentryDto>>.Return500(e, "Failed to get all LoanCommentrys");
            }
        }
    }
}
