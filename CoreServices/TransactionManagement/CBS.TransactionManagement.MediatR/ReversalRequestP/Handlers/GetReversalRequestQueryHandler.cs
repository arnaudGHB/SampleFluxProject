using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries.ReversalRequestP;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;

namespace CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Handlers.ReversalRequestP
{
    /// <summary>
    /// Handles the request to retrieve a specific CashReplenishment based on its unique identifier.
    /// </summary>
    public class GetReversalRequestQueryHandler : IRequestHandler<GetReversalRequestQuery, ServiceResponse<ReversalRequestDto>>
    {
        private readonly IReversalRequestRepository _CashReplenishmentRepository; // Repository for accessing CashReplenishment data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetReversalRequestQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly ITransactionRepository _transactionRepository; // Repository for accessing CashReplenishment data.

        /// <summary>
        /// Constructor for initializing the ReversalRequestQueryHandler.
        /// </summary>
        /// <param name="CashReplenishmentRepository">Repository for CashReplenishment data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetReversalRequestQueryHandler(
            IReversalRequestRepository CashReplenishmentRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetReversalRequestQueryHandler> logger,
            ITransactionRepository transactionRepository = null)
        {
            _CashReplenishmentRepository = CashReplenishmentRepository;
            _mapper = mapper;
            _userInfoToken = userInfoToken;
            _logger = logger;
            _transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Handles the ReversalRequestQuery to retrieve a specific CashReplenishment.
        /// </summary>
        /// <param name="request">The ReversalRequestQuery containing CashReplenishment ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ReversalRequestDto>> Handle(GetReversalRequestQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the CashReplenishment entity with the specified ID from the repository
                var entity = await _CashReplenishmentRepository.FindAsync(request.Id);
                if (entity != null)
                {
                    var transactions = await _transactionRepository.FindBy(x => x.TransactionReference == entity.TransactionReference).ToListAsync();
                    var transactionsDto = _mapper.Map<List<TransactionDto>>(transactions);
                    var CashReplenishmentDto = _mapper.Map<ReversalRequestDto>(entity);
                    CashReplenishmentDto.Transactions = transactionsDto;
                    //await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _userInfoToken.Token);
                    return ServiceResponse<ReversalRequestDto>.ReturnResultWith200(CashReplenishmentDto);
                }
                else
                {
                    // If the CashReplenishment entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("CashReplenishment not found.");
                    //await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "CashReplenishment not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<ReversalRequestDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting CashReplenishment: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<ReversalRequestDto>.Return500(e);
            }
        }

    }

}
