using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AccountClass based on the GetAllAccountCategoryQuery.
    /// </summary>
    public class GetAllAccountClassQueryHandler : IRequestHandler<GetAllAccountClassesQuery, ServiceResponse<List<AccountClassDto>>>
    {
        private readonly IAccountClassRepository _AccountClassRepository; // Repository for accessing AccountClasss data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountClassQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountClassQueryHandler.
        /// </summary>
        /// <param name="AccountClassRepository">Repository for AccountClass data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountClassQueryHandler(
            IAccountClassRepository AccountClassRepository,
            IMapper mapper, ILogger<GetAllAccountClassQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _AccountClassRepository = AccountClassRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllAccountClassQuery to retrieve all AccountClass.
        /// </summary>
        /// <param name="request">The GetAllAccountClassQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountClassDto>>> Handle(GetAllAccountClassesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountClasss entities from the repository
                var entities = await _AccountClassRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                return ServiceResponse<List<AccountClassDto>>.ReturnResultWith200(_mapper.Map<List<AccountClassDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all AccountClassDto: {e.Message}");
                return ServiceResponse<List<AccountClassDto>>.Return500(e, "Failed to get all AccountClass");
            }
        }
    }
}