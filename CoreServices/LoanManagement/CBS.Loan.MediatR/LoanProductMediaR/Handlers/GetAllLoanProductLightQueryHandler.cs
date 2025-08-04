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
    public class GetAllLoanProductLightQueryHandler : IRequestHandler<GetAllLoanProductLightQuery, ServiceResponse<List<LoanProductLightDto>>>
    {
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing LoanProducts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllLoanProductLightQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllLoanProductLightQueryHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProducts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllLoanProductLightQueryHandler(
            ILoanProductRepository LoanProductRepository,
            IMapper mapper, ILogger<GetAllLoanProductLightQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _LoanProductRepository = LoanProductRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllLoanProductLightQuery to retrieve all LoanProducts.
        /// </summary>
        /// <param name="request">The GetAllLoanProductLightQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanProductLightDto>>> Handle(GetAllLoanProductLightQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch only necessary fields using Select (avoid loading unnecessary relationships)
                var entities = await _LoanProductRepository.All.AsNoTracking()
                    .Where(x => !x.IsDeleted)
                    .Select(x => new
                    {
                        x.Id,
                        x.ProductCode,
                        x.ProductName,
                        x.IsProductWithSavingFacilities,
                        x.LoanMinimumAmount,
                        x.LoanMaximumAmount,
                        x.TargetType,
                        x.LoanProductCategoryId,
                        LoanProductCategoryName = x.LoanProductCategory != null ? x.LoanProductCategory.Name : "N/A", // Handle null category
                        x.LoanTermId,
                        LoanTermName = x.LoanTerm != null ? x.LoanTerm.Name : "N/A", // Handle null term
                        LoanTermMinMonth = x.LoanTerm != null ? x.LoanTerm.MinInMonth : 0, // Handle null term
                        LoanTermMaxMonth = x.LoanTerm != null ? x.LoanTerm.MaxInMonth : 0, // Handle null term
                        x.ActiveStatus
                    })
                    .ToListAsync(cancellationToken);

                // Map to LoanProductLightDto manually
                var result = entities.Select(entity => new LoanProductLightDto
                {
                    Id = entity.Id,
                    ProductCode = entity.ProductCode,
                    ProductName = entity.ProductName,
                    IsProductWithSavingFacilities = entity.IsProductWithSavingFacilities,
                    LoanMinimumAmount = entity.LoanMinimumAmount,
                    LoanMaximumAmount = entity.LoanMaximumAmount,
                    TargetType = entity.TargetType,
                    LoanProductCategoryId = entity.LoanProductCategoryId,
                    LoanProductCategory = new LoanProductCategory
                    {
                        Name = entity.LoanProductCategoryName // Set to "N/A" if null
                    },
                    LoanTermId = entity.LoanTermId,
                    LoanTerm = new LoanTerm
                    {
                        Name = entity.LoanTermName, // Set to "N/A" if null
                        MinInMonth = entity.LoanTermMinMonth, // Default to 0 if null
                        MaxInMonth = entity.LoanTermMaxMonth // Default to 0 if null
                    },
                    ActiveStatus = entity.ActiveStatus
                }).ToList();

                // Return optimized response
                return ServiceResponse<List<LoanProductLightDto>>.ReturnResultWith200(result);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all LoanProducts: {e.Message}");
                return ServiceResponse<List<LoanProductLightDto>>.Return500(e, "Failed to get all LoanProducts");
            }
        }

    }
}
