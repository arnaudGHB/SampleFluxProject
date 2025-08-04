using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.BudgetItemDetailManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a BudgetItemDetail based on UpdateBudgetItemDetailCommand.
    /// </summary>
    public class UpdateBudgetItemDetailCommandHandler : IRequestHandler<UpdateBudgetItemDetailCommand, ServiceResponse<BudgetItemDetailDto>>
    {
        private readonly IBudgetItemDetailRepository _BudgetItemDetailRepository; // Repository for accessing BudgetItemDetail data.
        private readonly ILogger<UpdateBudgetItemDetailCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userTokenInfo;
        /// <summary>
        /// Constructor for initializing the UpdateBudgetItemDetailCommandHandler.
        /// </summary>
        /// <param name="BudgetItemDetailRepository">Repository for BudgetItemDetail data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateBudgetItemDetailCommandHandler(
            IBudgetItemDetailRepository BudgetItemDetailRepository,
            ILogger<UpdateBudgetItemDetailCommandHandler> logger,
            IMapper mapper, UserInfoToken userTokenInfo,
            IUnitOfWork<POSContext> uow = null)
        {
            _BudgetItemDetailRepository = BudgetItemDetailRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow; 
            _userTokenInfo = userTokenInfo;
        }

        /// <summary>
        /// Handles the UpdateBudgetItemDetailCommand to update a BudgetItemDetail.
        /// </summary>
        /// <param name="request">The UpdateBudgetItemDetailCommand containing updated BudgetItemDetail data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BudgetItemDetailDto>> Handle(UpdateBudgetItemDetailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the BudgetItemDetail entity to be updated from the repository
                var existingBudgetItemDetail = await _BudgetItemDetailRepository.FindAsync(request.Id);

                // Check if the BudgetItemDetail entity exists
                if (existingBudgetItemDetail != null)
                {
                    // Update BudgetItemDetail entity properties with values from the request
                    existingBudgetItemDetail = _mapper.Map(request, existingBudgetItemDetail) ;
                    // Use the repository to update the existing BudgetItemDetail entity
                    _BudgetItemDetailRepository.Update(existingBudgetItemDetail);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<BudgetItemDetailDto>.ReturnResultWith200(_mapper.Map<BudgetItemDetailDto>(existingBudgetItemDetail));
                    _logger.LogInformation($"BudgetItemDetail {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the BudgetItemDetail entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"BudgetItemDetail {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<BudgetItemDetailDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating BudgetItemDetail: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<BudgetItemDetailDto>.Return500(e);
            }
        }
    }
}