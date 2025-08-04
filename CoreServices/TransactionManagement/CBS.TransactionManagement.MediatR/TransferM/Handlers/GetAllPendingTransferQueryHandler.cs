using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.TransferM.Queries;
using CBS.TransactionManagement.Repository.VaultP;
using CBS.TransactionManagement.MediatR.VaultP.Queries;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Transfer based on the GetAllPendingTransferQuery.
    /// </summary>
    public class GetAllPendingTransferQueryHandler : IRequestHandler<GetAllPendingTransferQuery, ServiceResponse<List<TransferDto>>>
    {
        private readonly ITransferRepository _TransferRepository; // Repository for accessing Transfer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllPendingTransferQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllPendingTransferQueryHandler.
        /// </summary>
        /// <param name="TransferRepository">Repository for Transfer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllPendingTransferQueryHandler(ITransferRepository TransferRepository,UserInfoToken UserInfoToken,IMapper mapper, ILogger<GetAllPendingTransferQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _TransferRepository = TransferRepository;
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllPendingTransferQuery to retrieve all Transfer.
        /// </summary>
        /// <param name="request">The GetAllPendingTransferQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TransferDto>>> Handle(GetAllPendingTransferQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //Retrieve all Transfer entities from the repository
                var entities = await _TransferRepository.FindBy(x=>x.Status==Status.Pending.ToString() && x.IsDeleted==false && x.BranchId==_userInfoToken.BranchID).ToListAsync();
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "System Transfers returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                return ServiceResponse<List<TransferDto>>.ReturnResultWith200(_mapper.Map<List<TransferDto>>(entities));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Transfer: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all Transfer: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<List<TransferDto>>.Return500(e, "Failed to get all Transfer");
            }
        }
     
    }
}
