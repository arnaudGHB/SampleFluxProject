using AutoMapper;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.CycleNameMediaR.Queries;
using CBS.NLoan.Repository.LoanApplicationP.LoanCycleP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace CBS.NLoan.MediatR.CycleNameMediaR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Loans based on the GetAllLoanQuery.
    /// </summary>
    public class GetAllLoanProductCategoryHandler : IRequestHandler<GetAllLoanProductCategoryQuery, ServiceResponse<List<LoanProductCategoryDto>>>
    {
        private readonly ILoanProductCategoryRepository _LoanCycleRepository; // Repository for accessing LoanCycles data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanProductCategoryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanCycleQueryHandler.
        /// </summary>
        /// <param name="LoanCycleRepository">Repository for LoanCycles data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanProductCategoryHandler(
            ILoanProductCategoryRepository LoanCycleRepository,
            IMapper mapper, ILogger<GetAllLoanProductCategoryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanCycleRepository = LoanCycleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanProductCategoryQuery to retrieve all LoanCycles.
        /// </summary>
        /// <param name="request">The GetAllLoanProductCategoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanProductCategoryDto>>> Handle(GetAllLoanProductCategoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all LoanCycles entities from the repository
                var entities = await _LoanCycleRepository.All.Where(x=>x.IsDeleted==false).ToListAsync();
                return ServiceResponse<List<LoanProductCategoryDto>>.ReturnResultWith200(_mapper.Map<List<LoanProductCategoryDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanCycles: {e.Message}");
                return ServiceResponse<List<LoanProductCategoryDto>>.Return500(e, "Failed to get all LoanCycles");
            }
        }
    }
}
