using AutoMapper;
using CBS.AccountManagement.Data;

using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.Helper.DataModel;
using CBS.AccountManagement.MediatR.Account.Queries;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.MediatR.BankingOperation.Handlers
{
    public class GetAllDepositNotificationQueryHandler : IRequestHandler<GetAllDepositNotificationQuery, ServiceResponse<List<DepositNotificationDto>>>
    {
        private readonly IDepositNotifcationRepository _depositNotifcationRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDepositNotificationQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDepositNotificationQueryHandler(
            IDepositNotifcationRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<GetAllDepositNotificationQueryHandler> logger, UserInfoToken userInfoToken)
        {
            _depositNotifcationRepository = cashReplenishmentRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DepositNotificationDto>>> Handle(GetAllDepositNotificationQuery request, CancellationToken cancellationToken)
        {
            List<DepositNotificationDto> _listMpped = new List<DepositNotificationDto>();
            List<DepositNotification> entitiesAccounts = new List<DepositNotification>();
          string errorMessage = null;
            try
            {
                if (_userInfoToken.IsHeadOffice)
                {
                    entitiesAccounts = await _depositNotifcationRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                }
                else
                {


                    entitiesAccounts = await _depositNotifcationRepository.All.Where(x => x.BranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false)).ToListAsync();

                }

                if (entitiesAccounts == null)
                {
                    errorMessage = $"No cash replenishment request was found";
                   
                    await APICallHelper.AuditLogger(_userInfoToken.Email, "GetCashReplenishmentQuery",
                        JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                    return  ServiceResponse<List<DepositNotificationDto>>.Return404(errorMessage);
                }
                else
                {
                  _listMpped = _mapper.Map(entitiesAccounts, _listMpped);
                    return ServiceResponse<List<DepositNotificationDto>>.ReturnResultWith200(_listMpped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByReferenceQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<DepositNotificationDto>>.Return500(e);
            }
        }
    }


    public class GetAllDepositNotificationQueryAsBranchHandler : IRequestHandler<GetAllDepositNotificationQueryAsBranch, ServiceResponse<List<DepositNotificationDto>>>
    {
        private readonly IDepositNotifcationRepository _depositNotifcationRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllDepositNotificationQueryAsBranchHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAccountByReferenceQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllDepositNotificationQueryAsBranchHandler(
            IDepositNotifcationRepository cashReplenishmentRepository,
            IMapper mapper,
            ILogger<GetAllDepositNotificationQueryAsBranchHandler> logger, UserInfoToken userInfoToken)
        {
            _depositNotifcationRepository = cashReplenishmentRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the GetAccountQuery to retrieve a specific Account.
        /// </summary>
        /// <param name="request">The GetAccountQuery containing Account ID to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<DepositNotificationDto>>> Handle(GetAllDepositNotificationQueryAsBranch request, CancellationToken cancellationToken)
        {
            List<DepositNotificationDto> _listMpped = new List<DepositNotificationDto>();
            List<DepositNotification> entitiesAccounts = new List<DepositNotification>();
            string errorMessage = null;
            try
            {

                if (_userInfoToken.IsHeadOffice)
                {
                    entitiesAccounts = await _depositNotifcationRepository.All.Where(x => x.IsDeleted.Equals(false)).ToListAsync();
                }
                else
                {

                
                        entitiesAccounts = await _depositNotifcationRepository.All.Where(x => x.CorrepondingBranchId.Equals(_userInfoToken.BranchId) && x.IsDeleted.Equals(false)).ToListAsync();
               
                }

                if (entitiesAccounts == null)
                {
                    errorMessage = $"No cash deposit request was found";

                      await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.BadRequest, LogAction.GetAllDepositNotificationQueryAsBranch, LogLevelInfo.Warning);

                    return ServiceResponse<List<DepositNotificationDto>>.Return404(errorMessage);
                }
                else
                {
                    _listMpped = _mapper.Map(entitiesAccounts, _listMpped);
                    errorMessage = $"GetAllDepositNotificationQueryAsBranch request was found with {_listMpped.Count()}";

                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetAllDepositNotificationQueryAsBranch, LogLevelInfo.Information);

                    return ServiceResponse<List<DepositNotificationDto>>.ReturnResultWith200(_listMpped);
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";
                await APICallHelper.AuditLogger(_userInfoToken.Email, "GetAccountByReferenceQuery",
                    JsonConvert.SerializeObject(request), errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<List<DepositNotificationDto>>.Return500(e);
            }
        }
    }
}
