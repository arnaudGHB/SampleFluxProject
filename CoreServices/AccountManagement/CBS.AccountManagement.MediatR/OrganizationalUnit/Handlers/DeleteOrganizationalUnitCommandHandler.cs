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
    /// Handles the command to delete a OrganizationalUnitName based on DeleteOrganizationalUnitNameCommand.
    /// </summary>
    public class DeleteOrganizationalUnitCommandHandler : IRequestHandler<DeleteOrganizationalUnitCommand, ServiceResponse<bool>>
    {
        private readonly IOrganizationalUnitRepository _OrganizationalUnitRepository; // Repository for accessing OrganizationalUnitName data.
        private readonly ILogger<DeleteOrganizationalUnitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the DeleteOrganizationalUnitNameCommandHandler.
        /// </summary>
        /// <param name="OrganizationalUnitNameRepository">Repository for OrganizationalUnitName data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteOrganizationalUnitCommandHandler(
            IOrganizationalUnitRepository OrganizationalUnitRepository, IMapper mapper,
            ILogger<DeleteOrganizationalUnitCommandHandler> logger
, IUnitOfWork<POSContext> uow, UserInfoToken? userInfoToken)
        {
            _OrganizationalUnitRepository = OrganizationalUnitRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the DeleteOrganizationalUnitNameCommand to delete a OrganizationalUnitName.
        /// </summary>
        /// <param name="request">The DeleteOrganizationalUnitNameCommand containing OrganizationalUnitName ID to be deleted.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteOrganizationalUnitCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Check if the OrganizationalUnitName entity with the specified ID exists
                var existingOrganizationalUnitName = await _OrganizationalUnitRepository.FindAsync(request.Id);
                if (existingOrganizationalUnitName == null)
                {
                    errorMessage = $"OrganizationalUnit with ID {request.Id} not found.";
                    _logger.LogError(errorMessage);
                 await   APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteOrganizationalUnitCommand",
            request, errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    

                    return ServiceResponse<bool>.Return404(errorMessage);
                }

                existingOrganizationalUnitName.IsDeleted = true;

                _OrganizationalUnitRepository.Update(existingOrganizationalUnitName);
                errorMessage = $"OrganizationalUnit with ID {request.Id} has been deleted successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteOrganizationalUnitCommand",
request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                await _uow.SaveAsync();
                return ServiceResponse<bool>.ReturnResultWith200(true);
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while deleting OrganizationalUnit: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "DeleteOrganizationalUnitCommand",
request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                // Log error and return 500 Internal Server Error response with error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }
}