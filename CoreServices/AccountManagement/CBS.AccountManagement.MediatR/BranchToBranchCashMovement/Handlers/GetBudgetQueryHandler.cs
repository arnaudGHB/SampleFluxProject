using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Command;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific BudgetName based on its unique identifier.
    /// </summary>
    public class GetBranchToBranchTransferDtoHandler : IRequestHandler<GetBranchToBranchTransferDto, ServiceResponse<BranchToBranchTransferDto>>
    {
        private readonly IMongoUnitOfWork _mongoUnitOfWork;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetBranchToBranchTransferDtoHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetBudgetNameQueryHandler.
        /// </summary>
        /// <param name="BudgetRepository">Repository for BudgetName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
 
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetBranchToBranchTransferDtoHandler(
         
            IMapper mapper,
            ILogger<GetBranchToBranchTransferDtoHandler> logger,
            UserInfoToken? userInfoToken,
            IMongoUnitOfWork? mongoUnitOfWork)
        {
            _mongoUnitOfWork = mongoUnitOfWork; 
                    _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetBudgetNameQuery to retrieve a specific BudgetName.
        /// </summary>
        /// <param name="request">The GetBudgetNameQuery containing BudgetName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<BranchToBranchTransferDto>> Handle(GetBranchToBranchTransferDto request, CancellationToken cancellationToken)
        {
            string message = null;
            try
            {
                var auditTrailRepository = _mongoUnitOfWork.GetRepository<BranchToBranchTransferDto>();
                // Retrieve the BudgetName entity with the specified ID from the repository
                var entity = await auditTrailRepository.GetByIdAsync(request.Id);
          
                    return ServiceResponse<BranchToBranchTransferDto>.ReturnResultWith200(entity);
                
            }
            catch (Exception e)
            {
                message = $"Error occurred while getting BudgetName: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetBranchToBranchTransferDto",
request, message, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(message);
                return ServiceResponse<BranchToBranchTransferDto>.Return500(e);
            }
        }
    }
}