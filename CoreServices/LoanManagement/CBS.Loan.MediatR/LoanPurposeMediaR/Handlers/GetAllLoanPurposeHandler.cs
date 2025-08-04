using AutoMapper;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Queries;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanPurposeMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanPurposeHandler : IRequestHandler<GetAllLoanPurposeQuery, ServiceResponse<List<LoanPurposeDto>>>
    {
        private readonly ILoanPurposeRepository _LoanPurposeRepository; // Repository for accessing LoanPurposes data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanPurposeHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanPurposeQueryHandler.
        /// </summary>
        /// <param name="LoanPurposeRepository">Repository for LoanPurposes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanPurposeHandler(
            ILoanPurposeRepository LoanPurposeRepository,
            IMapper mapper, ILogger<GetAllLoanPurposeHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanPurposeRepository = LoanPurposeRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanPurposeQuery to retrieve all LoanPurposes.
        /// </summary>
        /// <param name="request">The GetAllLoanPurposeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanPurposeDto>>> Handle(GetAllLoanPurposeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanPurposes entities from the repository
                var entities = await _LoanPurposeRepository
                    .All
                    .Include(x => x.LoanProductCategory)
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();

                // Map to LoanPurposeDto and handle null LoanProductCategory
                var loanPurposeDtos = _mapper.Map<List<LoanPurposeDto>>(entities).Select(dto =>
                {
                    // If LoanProductCategory is null, create a new LoanProductCategory with "N/A"
                    if (dto.LoanProductCategory == null)
                    {
                        dto.LoanProductCategory = new LoanProductCategory
                        {
                            Name = "N/A"
                        };
                    }

                    // Return the dto with the adjusted LoanProductCategory
                    return dto;
                }).ToList();

                // Return the mapped list with the adjusted LoanProductCategory
                return ServiceResponse<List<LoanPurposeDto>>.ReturnResultWith200(loanPurposeDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanPurposes: {e.Message}");
                return ServiceResponse<List<LoanPurposeDto>>.Return500(e, "Failed to get all LoanPurposes");
            }
        }
    }
}
