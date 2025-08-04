using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CBS.BudgetItemDetailManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all BudgetItemDetails based on the GetAllBudgetItemDetailQuery.
    /// </summary>
    public class GetAllBudgetItemDetailQueryHandler : IRequestHandler<GetAllBudgetItemDetailQuery, ServiceResponse<List<BudgetItemDetailDto>>>
    {
        private readonly IBudgetItemDetailRepository _BudgetItemDetailRepository; // Repository for accessing BudgetItemDetails data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllBudgetItemDetailQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllBudgetItemDetailQueryHandler.
        /// </summary>
        /// <param name="BudgetItemDetailRepository">Repository for BudgetItemDetails data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllBudgetItemDetailQueryHandler( UserInfoToken userInfoToken,
            IBudgetItemDetailRepository BudgetItemDetailRepository,
            IMapper mapper, ILogger<GetAllBudgetItemDetailQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _BudgetItemDetailRepository = BudgetItemDetailRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllBudgetItemDetailQuery to retrieve all BudgetItemDetails.
        /// </summary>
        /// <param name="request">The GetAllBudgetItemDetailQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
    
        public async Task<ServiceResponse<List<BudgetItemDetailDto>>> Handle(GetAllBudgetItemDetailQuery request, CancellationToken cancellationToken)
        {
            List < BudgetItemDetailDto > detailDtos = new List<BudgetItemDetailDto>();
            try
            {
                // Retrieve all BudgetItemDetails entities from the repository
                var entities = await _BudgetItemDetailRepository.All.Where(x => x.IsDeleted.Equals(false)&&x.BranchId==_userInfoToken.BranchId).ToListAsync();
           string     errorMessage = $"BudgetItemDetailDto with {entities.Count()} a success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetItemDetailQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                detailDtos = _mapper.Map(entities, detailDtos);
                return ServiceResponse<List<BudgetItemDetailDto>>.ReturnResultWith200(detailDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all BudgetItemDetails: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting BudgetItemDetail: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllBudgetItemDetailQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<BudgetItemDetailDto>>.Return500(e, "Failed to get all BudgetItemDetails");
            }
        }
    }
}