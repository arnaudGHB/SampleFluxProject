using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.BankingOperation.Handlers
{
    public class GetDepositNotificationQueryHandler : IRequestHandler<GetDepositNotificationQuery, ServiceResponse<DepositNotificationDto>>
    {
        private readonly IDepositNotifcationRepository _depositNotificationRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetDepositNotificationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetDepositNotificationQueryHandler(
            IDepositNotifcationRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<GetDepositNotificationQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _depositNotificationRepository = cashReplenishmentRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DepositNotificationDto>> Handle(GetDepositNotificationQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entitiesAccounts = await _depositNotificationRepository.FindAsync(request.Id);
                if (entitiesAccounts == null)
                {
                    errorMessage = $"No cash DepositNotification request was found";
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetDepositNotificationQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);

                    return  ServiceResponse<DepositNotificationDto>.Return404(errorMessage);
                }
                else
                {
                   var _mapped = _mapper.Map<DepositNotificationDto>(entitiesAccounts);
                    return ServiceResponse<DepositNotificationDto>.ReturnResultWith200(_mapped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetDepositNotificationQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<DepositNotificationDto>.Return500(e);
            }
        }
    }
}
