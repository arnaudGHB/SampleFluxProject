using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.TransferM.Queries;
using CBS.TransactionManagement.Data.Dto.TransferLimits;
using CBS.TransactionManagement.Repository.VaultP;
using CBS.TransactionManagement.MediatR.VaultP.Queries;

namespace CBS.TransactionManagement.TransferM.Handlers
{
    /// <summary>
    /// Handles the request to retrieve a specific Transfer based on its unique identifier.
    /// </summary>
    public class GetTransferQueryHandler : IRequestHandler<GetTransferQuery, ServiceResponse<TransferDto>>
    {
        private readonly ITransferRepository _TransferRepository; // Repository for accessing Transfer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetTransferQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetTransferQueryHandler.
        /// </summary>
        /// <param name="TransferRepository">Repository for Transfer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetTransferQueryHandler(
            ITransferRepository TransferRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetTransferQueryHandler> logger)
        {
            _TransferRepository = TransferRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetTransferQuery to retrieve a specific Transfer.
        /// </summary>
        /// <param name="request">The GetTransferQuery containing Transfer ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TransferDto>> Handle(GetTransferQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Transfer entity with the specified ID from the repository
                var entity = await _TransferRepository.AllIncluding(x=>x.Teller).FirstOrDefaultAsync(x=>x.Id==request.Id);
                if (entity != null)
                {
                    // Map the Transfer entity to TransferDto and return it with a success response
                    var TransferDto = _mapper.Map<TransferDto>(entity);
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<TransferDto>.ReturnResultWith200(TransferDto);
                }
                else
                {
                    // If the Transfer entity was not found, log the error and return a 404 Not Found response
                    errorMessage="Transfer not found.";
                    _logger.LogError("Transfer not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Transfer not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<TransferDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Transfer: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<TransferDto>.Return500(e);
            }
        }

    }

}
