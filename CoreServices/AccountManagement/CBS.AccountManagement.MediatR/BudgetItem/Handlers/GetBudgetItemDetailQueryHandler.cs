using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.BudgetItemDetailManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BudgetItemDetail based on its unique identifier.
    /// </summary>
    public class GetBudgetItemDetailQueryHandler : IRequestHandler<GetBudgetItemDetailQuery, ServiceResponse<BudgetItemDetailDto>>
    {
        private readonly IBudgetItemDetailRepository _budgetItemDetailRepository; // Repository for accessing BudgetItemDetail data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBudgetItemDetailQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetBudgetItemDetailQueryHandler.
        /// </summary>
        /// <param name="BudgetItemDetailRepository">Repository for BudgetItemDetail data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBudgetItemDetailQueryHandler(
            IBudgetItemDetailRepository budgetItemDetailRepository,
            IMapper mapper,
            ILogger<GetBudgetItemDetailQueryHandler> logger)
        {
            _budgetItemDetailRepository = budgetItemDetailRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetBudgetItemDetailQuery to retrieve a specific BudgetItemDetail.
        /// </summary>
        /// <param name="request">The GetBudgetItemDetailQuery containing BudgetItemDetail ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetItemDetailDto>> Handle(GetBudgetItemDetailQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the BudgetItemDetail entity with the specified ID from the repository
                var entity = await _budgetItemDetailRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "BudgetItemDetail has been deleted.";
                        // If the BudgetItemDetailCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<BudgetItemDetailDto>.Return404(message);
                    }
                    else
                    {
                        // Map the BudgetItemDetail entity to BudgetItemDetailDto and return it with a success response
                        var BudgetItemDetailDto = _mapper.Map<BudgetItemDetailDto>(entity);
                        return ServiceResponse<BudgetItemDetailDto>.ReturnResultWith200(BudgetItemDetailDto);

                    }
                }
                else
                {
                    // If the BudgetItemDetail entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("BudgetItemDetail not found.");
                    return ServiceResponse<BudgetItemDetailDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting BudgetItemDetail: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<BudgetItemDetailDto>.Return500(e);
            }
        }
    }
}