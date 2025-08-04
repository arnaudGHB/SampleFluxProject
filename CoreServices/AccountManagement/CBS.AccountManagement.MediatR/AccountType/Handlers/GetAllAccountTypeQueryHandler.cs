using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all AccountTypes based on the GetAllAccountTypeQuery.
    /// </summary>
    public class GetAllAccountTypeQueryHandler : IRequestHandler<GetAllAccountTypeQuery, ServiceResponse<List<AccountTypeDto>>>
    {
        private readonly IAccountTypeRepository _AccountTypeRepository; // Repository for accessing AccountTypes data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountTypeQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllAccountTypeQueryHandler.
        /// </summary>
        /// <param name="AccountTypeRepository">Repository for AccountTypes data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllAccountTypeQueryHandler(
            IAccountTypeRepository AccountTypeRepository,
            IMapper mapper, ILogger<GetAllAccountTypeQueryHandler> logger,UserInfoToken userInfoToken)
        {
            // Assign provided dependencies to local variables.
            _AccountTypeRepository = AccountTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllAccountTypeQuery to retrieve all AccountTypes.
        /// </summary>
        /// <param name="request">The GetAllAccountTypeQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountTypeDto>>> Handle(GetAllAccountTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve all AccountTypes entities from the repository
                var entities = await _AccountTypeRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                var errorMessag = $" GetAllAccountTypeQuery executed Successfully.";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountTypeQuery",
                    JsonConvert.SerializeObject(request), errorMessag, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                return ServiceResponse<List<AccountTypeDto>>.ReturnResultWith200(_mapper.Map<List<AccountTypeDto>>(entities));
            }
            catch (Exception e)
            {
               
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Failed to get all AccountTypes: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllAccountTypeQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
 
                return ServiceResponse<List<AccountTypeDto>>.Return500(e, errorMessage);
            }
        }
    }
}