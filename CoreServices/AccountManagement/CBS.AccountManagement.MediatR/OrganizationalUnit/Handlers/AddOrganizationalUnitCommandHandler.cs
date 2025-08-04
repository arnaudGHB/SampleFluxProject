using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new OrganizationalUnitName.
    /// </summary>
    public class AddOrganizationalUnitCommandHandler : IRequestHandler<AddOrganizationalUnitCommand, ServiceResponse<OrganizationalUnitDto>>
    {
        private readonly IOrganizationalUnitRepository _OrganizationalUnitNameRepository; // Repository for accessing OrganizationalUnitName data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOrganizationalUnitCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the AddOrganizationalUnitNameCommandHandler.
        /// </summary>
        /// <param name="OrganizationalUnitNameRepository">Repository for OrganizationalUnitName data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOrganizationalUnitCommandHandler(
            IOrganizationalUnitRepository OrganizationalUnitNameRepository,
            IMapper mapper,
            ILogger<AddOrganizationalUnitCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _OrganizationalUnitNameRepository = OrganizationalUnitNameRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken= userInfoToken;
        }

        /// <summary>
        /// Handles the AddOrganizationalUnitNameCommand to add a new OrganizationalUnitName.
        /// </summary>
        /// <param name="request">The AddOrganizationalUnitNameCommand containing OrganizationalUnitName data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<OrganizationalUnitDto>> Handle(AddOrganizationalUnitCommand request, CancellationToken cancellationToken)
        {
            var errorMessage = "";
             try
            {
                // Check if a OrganizationalUnitName with the same name already exists (case-insensitive)
                var existingOrganizationalUnitName =   _OrganizationalUnitNameRepository.All.Where(c => c.Name == request.Name&&c.IsDeleted==false && c.BranchId==_userInfoToken.BranchId);

                // If a OrganizationalUnitName with the same name already exists, return a conflict response
                if (existingOrganizationalUnitName.Any())
                {
                      errorMessage = $"OrganizationalUnit of {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "AddOrganizationalUnitCommand",
                      request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
       
                    return ServiceResponse<OrganizationalUnitDto>.Return409(errorMessage);
                }

                // Map the AddOrganizationalUnitNameAttributesCommand to a OrganizationalUnitNameAttributes entity
                var OrganizationalUnitNameAttributesEntity = _mapper.Map<OrganizationalUnit>(request);
                OrganizationalUnitNameAttributesEntity.Id = BaseUtilities.GenerateInsuranceUniqueNumber(12, "BGTC");
                _OrganizationalUnitNameRepository.Add(OrganizationalUnitNameAttributesEntity);
                await _uow.SaveAsync();
                errorMessage = $"OrganizationalUnit of {request.Name} was successfully created.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddOrganizationalUnitCommand",
                        request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);


                // Map the OrganizationalUnitName entity to OrganizationalUnitNameDto and return it with a success response
                var OrganizationalUnitNameDto = _mapper.Map<OrganizationalUnitDto>(OrganizationalUnitNameAttributesEntity);
                return ServiceResponse<OrganizationalUnitDto>.ReturnResultWith200(OrganizationalUnitNameDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while saving OrganizationalUnitName: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "AddOrganizationalUnitCommand",
                    request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);


                return ServiceResponse<OrganizationalUnitDto>.Return500(e);
            }
        }
    }
}