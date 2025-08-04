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
using System.Security.Cryptography.X509Certificates;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all TrialBalanceFile based on the GetAllTrialBalanceFileQuery.
    /// </summary>
    public class GetAllTrialBalanceFileQueryHandler : IRequestHandler<GetAllTrialBalanceFileQuery, ServiceResponse<List<TrialBalanceFileDto>>>
    {
        private readonly ITrialBalanceFileRepository _TrialBalanceFileRepository; // Repository for accessing TrialBalanceFiles data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTrialBalanceFileQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllTrialBalanceFileQueryHandler.
        /// </summary>
        /// <param name="TrialBalanceFileRepository">Repository for TrialBalanceFiles data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTrialBalanceFileQueryHandler( UserInfoToken userInfoToken,
            ITrialBalanceFileRepository TrialBalanceFileRepository,
            IMapper mapper, ILogger<GetAllTrialBalanceFileQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TrialBalanceFileRepository = TrialBalanceFileRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAllTrialBalanceFileQuery to retrieve all TrialBalanceFiles.
        /// </summary>
        /// <param name="request">The GetAllTrialBalanceFileQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
    
        public async Task<ServiceResponse<List<TrialBalanceFileDto>>> Handle(GetAllTrialBalanceFileQuery request, CancellationToken cancellationToken)
        {
            List <TrialBalanceFileDto>  listTrialBalanceFileData = new List<TrialBalanceFileDto>();
            try
            {
                var entities = new List<Data.TrialBalanceFile>();
                // Retrieve all TrialBalanceFiles entities from the repository&&
                if (_userInfoToken.IsHeadOffice)
                {
                  
                        entities = await _TrialBalanceFileRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();

                  

                }
                else
                {
                    entities = await _TrialBalanceFileRepository.All.Where(x => x.IsDeleted.Equals(false) && x.BranchId == _userInfoToken.BranchId).ToListAsync();

                }
                string    errorMessage = $"Return TrialBalanceFileDto with a success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetTrialBalanceFileByTrialBalanceFileNumberQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                listTrialBalanceFileData = _mapper.Map(entities, listTrialBalanceFileData); 
                return ServiceResponse<List<TrialBalanceFileDto>>.ReturnResultWith200(listTrialBalanceFileData);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all TrialBalanceFiles: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting TrialBalanceFile: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllTrialBalanceFileQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<TrialBalanceFileDto>>.Return500(e, "Failed to get all TrialBalanceFiles");
            }
        }
    }


 
}