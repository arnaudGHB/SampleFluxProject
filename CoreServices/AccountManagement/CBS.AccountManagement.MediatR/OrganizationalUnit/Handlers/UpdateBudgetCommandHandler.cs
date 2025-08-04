using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to update a OrganizationalUnitName based on UpdateOrganizationalUnitNameCommand.
    /// </summary>
    public class UpdateOrganizationalUnitCommandHandler : IRequestHandler<UpdateOrganizationalUnitCommand, ServiceResponse<OrganizationalUnitDto>>
    {
        private readonly IOrganizationalUnitRepository _OrganizationalUnitNameRepository; // Repository for accessing OrganizationalUnitName data.
        private readonly ILogger<UpdateOrganizationalUnitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the UpdateOrganizationalUnitNameCommandHandler.
        /// </summary>
        /// <param name="OrganizationalUnitNameRepository">Repository for OrganizationalUnitName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateOrganizationalUnitCommandHandler(
            IOrganizationalUnitRepository OrganizationalUnitNameRepository,
            ILogger<UpdateOrganizationalUnitCommandHandler> logger,
            IMapper mapper,
            UserInfoToken? userInfoToken,
            IUnitOfWork<POSContext> uow = null)
        {
            _OrganizationalUnitNameRepository = OrganizationalUnitNameRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;

            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateOrganizationalUnitNameCommand to update a OrganizationalUnitName.
        /// </summary>
        /// <param name="request">The UpdateOrganizationalUnitNameCommand containing updated OrganizationalUnitName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OrganizationalUnitDto>> Handle(UpdateOrganizationalUnitCommand request, CancellationToken cancellationToken)
        {
           string errorMessage = string.Empty;
            try
            {
                // Retrieve the OrganizationalUnitName entity to be updated from the repository
                var existingOrganizationalUnitName = await _OrganizationalUnitNameRepository.FindAsync(request.Id);

                // Check if the OrganizationalUnitName entity exists
                if (existingOrganizationalUnitName != null)
                {
                    // Update OrganizationalUnitName entity properties with values from the request

                    existingOrganizationalUnitName= _mapper.Map(request, existingOrganizationalUnitName);
                    // Use the repository to update the existing OrganizationalUnitName entity
                    _OrganizationalUnitNameRepository.Update(existingOrganizationalUnitName);
                    await _uow.SaveAsync();

                    errorMessage = $"OrganizationalUnit {request.Id} was successfully updated.";
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<OrganizationalUnitDto>.ReturnResultWith200(_mapper.Map<OrganizationalUnitDto>(existingOrganizationalUnitName));
                    _logger.LogInformation(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateOrganizationalUnitCommand",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return response;
                }
                else
                {
                    // If the OrganizationalUnitName entity was not found, return 404 Not Found response with an error message
                      errorMessage = $"OrganizationalUnit {request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateOrganizationalUnitCommand",
                request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return ServiceResponse<OrganizationalUnitDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                  errorMessage = $"Error occurred while updating OrganizationalUnitName: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "UpdateOrganizationalUnitCommand",
          request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<OrganizationalUnitDto>.Return500(e);
            }
        }
    }
}