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
    public class GetTrialBalanceFileQueryHandler : IRequestHandler<GetTrialBalanceFileQuery, ServiceResponse<TrialBalanceFileDto>>
    {
        private readonly ITrialBalanceFileRepository _TrialBalanceFileRepository; // Repository for accessing TrialBalanceFiles data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTrialBalanceFileQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllTrialBalanceFileQueryHandler.
        /// </summary>
        /// <param name="TrialBalanceFileRepository">Repository for TrialBalanceFiles data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTrialBalanceFileQueryHandler( UserInfoToken userInfoToken,
            ITrialBalanceFileRepository TrialBalanceFileRepository,
            IMapper mapper, ILogger<GetTrialBalanceFileQueryHandler> logger)
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
    
        public async Task<ServiceResponse<TrialBalanceFileDto>> Handle(GetTrialBalanceFileQuery request, CancellationToken cancellationToken)
        {
                   try
            {
                var entities = new Data.TrialBalanceFileDto();


               var model = await _TrialBalanceFileRepository.FindAsync(request.Id);

                  
 
                string    errorMessage = $"Return TrialBalanceFileDto with a success response";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetTrialBalanceFileByTrialBalanceFileNumberQuery",
                   request, errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                var dto = _mapper.Map(model,entities); 
                return ServiceResponse<TrialBalanceFileDto>.ReturnResultWith200(dto);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all TrialBalanceFiles: {BaseUtilities.GetInnerExceptionMessages(e)}");
                string errorMessage = $"Error occurred while getting TrialBalanceFile: {BaseUtilities.GetInnerExceptionMessages(e)}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAllTrialBalanceFileQuery",
                   request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<TrialBalanceFileDto>.Return500(e, "Failed to get all TrialBalanceFiles");
            }
        }
    }


 
}