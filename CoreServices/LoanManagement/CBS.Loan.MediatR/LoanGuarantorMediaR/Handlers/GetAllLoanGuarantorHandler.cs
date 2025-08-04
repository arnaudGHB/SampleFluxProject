using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanGuarantorMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanGuarantorHandler : IRequestHandler<GetAllLoanGuarantorQuery, ServiceResponse<List<LoanGuarantorDto>>>
    {
        private readonly ILoanGuarantorRepository _LoanGuarantorRepository; // Repository for accessing LoanGuarantors data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanGuarantorHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanGuarantorQueryHandler.
        /// </summary>
        /// <param name="LoanGuarantorRepository">Repository for LoanGuarantors data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanGuarantorHandler(
            ILoanGuarantorRepository LoanGuarantorRepository,
            IMapper mapper, ILogger<GetAllLoanGuarantorHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanGuarantorRepository = LoanGuarantorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanGuarantorQuery to retrieve all LoanGuarantors.
        /// </summary>
        /// <param name="request">The GetAllLoanGuarantorQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanGuarantorDto>>> Handle(GetAllLoanGuarantorQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanGuarantors entities from the repository
                var entities = await _LoanGuarantorRepository.All.Where(x=>!x.IsDeleted).ToListAsync();
                return ServiceResponse<List<LoanGuarantorDto>>.ReturnResultWith200(_mapper.Map<List<LoanGuarantorDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanGuarantors: {e.Message}");
                return ServiceResponse<List<LoanGuarantorDto>>.Return500(e, "Failed to get all LoanGuarantors");
            }
        }
    }
}
