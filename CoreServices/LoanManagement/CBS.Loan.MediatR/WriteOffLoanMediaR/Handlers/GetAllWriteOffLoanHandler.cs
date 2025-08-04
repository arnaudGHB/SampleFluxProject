using AutoMapper;
using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Queries;
using CBS.NLoan.Repository.WriteOffLoanP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.WriteOffLoanMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllWriteOffLoanHandler : IRequestHandler<GetAllWriteOffLoanQuery, ServiceResponse<List<WriteOffLoanDto>>>
    {
        private readonly IWriteOffLoanRepository _WriteOffLoanRepository; // Repository for accessing WriteOffLoans data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllWriteOffLoanHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllWriteOffLoanQueryHandler.
        /// </summary>
        /// <param name="WriteOffLoanRepository">Repository for WriteOffLoans data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllWriteOffLoanHandler(
            IWriteOffLoanRepository WriteOffLoanRepository,
            IMapper mapper, ILogger<GetAllWriteOffLoanHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _WriteOffLoanRepository = WriteOffLoanRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllWriteOffLoanQuery to retrieve all WriteOffLoans.
        /// </summary>
        /// <param name="request">The GetAllWriteOffLoanQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<WriteOffLoanDto>>> Handle(GetAllWriteOffLoanQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all WriteOffLoans entities from the repository
                var entities = await _WriteOffLoanRepository.All.ToListAsync();
                return ServiceResponse<List<WriteOffLoanDto>>.ReturnResultWith200(_mapper.Map<List<WriteOffLoanDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all WriteOffLoans: {e.Message}");
                return ServiceResponse<List<WriteOffLoanDto>>.Return500(e, "Failed to get all WriteOffLoans");
            }
        }
    }
}
