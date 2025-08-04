using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.LoanProductMediaR.Queries;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanProductQueryHandler : IRequestHandler<GetAllLoanProductQuery, ServiceResponse<List<LoanProductDto>>>
    {
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing LoanProducts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanProductQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanProductQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProducts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanProductQueryHandler(
            ILoanProductRepository LoanProductRepository,
            IMapper mapper, ILogger<GetAllLoanProductQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanProductQuery to retrieve all LoanProducts.
        /// </summary>
        /// <param name="request">The GetAllLoanProductQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanProductDto>>> Handle(GetAllLoanProductQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanProducts including LoanProductCategory
                var entities = await _LoanProductRepository.All
                    .Include(x => x.LoanProductCategory).Include(x => x.LoanTerm)
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();

                // Handle null LoanProductCategory in a loop
                foreach (var entity in entities)
                {
                    if (entity.LoanProductCategory == null)
                    {
                        entity.LoanProductCategory = new LoanProductCategory { Name = "N/A" };
                    }
                    if (entity.LoanTerm == null)
                    {
                        entity.LoanTerm = new LoanTerm { Name = "N/A", MinInMonth = 0, MaxInMonth = 0 };
                    }
                }

                // Map entities to DTOs and return the response
                return ServiceResponse<List<LoanProductDto>>.ReturnResultWith200(_mapper.Map<List<LoanProductDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanProducts: {e.Message}");
                return ServiceResponse<List<LoanProductDto>>.Return500(e, "Failed to get all LoanProducts");
            }
        }

    }
}
