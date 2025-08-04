using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all OrganizationalUnit based on the GetAllOrganizationalUnitNameQuery.
    /// </summary>
    public class GetAllOrganizationalUnitQueryHandler : IRequestHandler<GetAllOrganizationalUnitQuery, ServiceResponse<List<OrganizationalUnitDto>>>
    {
        private readonly IOrganizationalUnitRepository _OrganizationalUnitRepository; // Repository for accessing OrganizationalUnit data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllOrganizationalUnitQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetAllOrganizationalUnitQueryHandler.
        /// </summary>
        /// <param name="OrganizationalUnitRepository">Repository for OrganizationalUnitName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllOrganizationalUnitQueryHandler(
            IOrganizationalUnitRepository OrganizationalUnitRepository,
            IMapper mapper, ILogger<GetAllOrganizationalUnitQueryHandler> logger, UserInfoToken? userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _OrganizationalUnitRepository = OrganizationalUnitRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllOrganizationalUnitNameQuery to retrieve all OrganizationalUnitName.
        /// </summary>
        /// <param name="request">The GetAllOrganizationalUnitNameQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<OrganizationalUnitDto>>> Handle(GetAllOrganizationalUnitQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            try
            {
                // Retrieve all OrganizationalUnit entities from the repository
                var entities = await _OrganizationalUnitRepository.All.Where(x=>x.IsDeleted.Equals(false)&&x.BranchId==_userInfoToken.BranchId).ToListAsync();
                 errorMessage = $"OrganizationalUnit : {entities.Count()} has been successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllOrganizationalUnitQuery",
request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<OrganizationalUnitDto>>.ReturnResultWith200(_mapper.Map<List<OrganizationalUnitDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all OrganizationalUnits: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllOrganizationalUnitQuery",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<OrganizationalUnitDto>>.Return500(e, "Failed to get all OrganizationalUnitName");
            }
        }
    }
}