using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific OrganizationalUnitName based on its unique identifier.
    /// </summary>
    public class GetOrganizationalUnitQueryHandler : IRequestHandler<GetOrganizationalUnitQuery, ServiceResponse<OrganizationalUnitDto>>
    {
        private readonly IOrganizationalUnitRepository _OrganizationalUnitRepository; // Repository for accessing OrganizationalUnitName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetOrganizationalUnitQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the GetOrganizationalUnitNameQueryHandler.
        /// </summary>
        /// <param name="OrganizationalUnitRepository">Repository for OrganizationalUnitName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
 
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetOrganizationalUnitQueryHandler(
            IOrganizationalUnitRepository OrganizationalUnitRepository,
            IMapper mapper,
            ILogger<GetOrganizationalUnitQueryHandler> logger,
            UserInfoToken? userInfoToken)
        {
            _OrganizationalUnitRepository = OrganizationalUnitRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetOrganizationalUnitNameQuery to retrieve a specific OrganizationalUnitName.
        /// </summary>
        /// <param name="request">The GetOrganizationalUnitNameQuery containing OrganizationalUnitName ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OrganizationalUnitDto>> Handle(GetOrganizationalUnitQuery request, CancellationToken cancellationToken)
        {
            string message = null;
            try
            {
                // Retrieve the OrganizationalUnitName entity with the specified ID from the repository
                var entity = await _OrganizationalUnitRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                          message = $"OrganizationalUnit Id:{request.Id} has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetOrganizationalUnitQuery",
request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                        _logger.LogError(message);
                        return ServiceResponse<OrganizationalUnitDto>.Return404(message);
                    }
                    else
                    {

                        // Map the OrganizationalUnitName entity to OrganizationalUnitNameDto and return it with a success response
                        var OrganizationalUnitNameDto = _mapper.Map<OrganizationalUnitDto>(entity);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, "GetOrganizationalUnitQuery",
request, message, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                        return ServiceResponse<OrganizationalUnitDto>.ReturnResultWith200(OrganizationalUnitNameDto);
                    }

                }
                else
                {
                      message = $"OrganizationalUnit Id:{request.Id} has been deleted.";
                    // If the OrganizationalUnitName entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError(message);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetOrganizationalUnitQuery",
  request, message, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<OrganizationalUnitDto>.Return404();
                }
            }
            catch (Exception e)
            {
                message = $"Error occurred while getting OrganizationalUnitName: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetOrganizationalUnitQuery",
request, message, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(message);
                return ServiceResponse<OrganizationalUnitDto>.Return500(e);
            }
        }
    }
}