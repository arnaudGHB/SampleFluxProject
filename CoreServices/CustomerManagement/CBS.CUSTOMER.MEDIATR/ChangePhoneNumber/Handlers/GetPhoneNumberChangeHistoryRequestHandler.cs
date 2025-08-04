using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Queries;
using CBS.CUSTOMER.REPOSITORY.ChangePhoneNumber;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.ChangePhoneNumber.Handlers
{
    /// <summary>
    /// Handles the query to get the phone number change history by PhoneNumberChangeHistoryId for a C-MONEY member.
    /// </summary>
    public class GetPhoneNumberChangeHistoryRequestHandler : IRequestHandler<GetPhoneNumberChangeHistoryRequestQuery, ServiceResponse<PhoneNumberChangeHistory>>
    {
        private readonly IPhoneNumberChangeHistoryRepository _phoneNumberChangeHistoryRepository;
        private readonly ILogger<GetPhoneNumberChangeHistoryRequestHandler> _logger;

        /// <summary>
        /// Constructor for initializing the GetPhoneNumberChangeHistoryRequestHandler.
        /// </summary>
        public GetPhoneNumberChangeHistoryRequestHandler(
            IPhoneNumberChangeHistoryRepository phoneNumberChangeHistoryRepository,
            ILogger<GetPhoneNumberChangeHistoryRequestHandler> logger)
        {
            _phoneNumberChangeHistoryRepository = phoneNumberChangeHistoryRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the request to retrieve a phone number change history record by its ID.
        /// </summary>
        public async Task<ServiceResponse<PhoneNumberChangeHistory>> Handle(GetPhoneNumberChangeHistoryRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching phone number change history for ID: {request.PhoneNumberChangeHistoryId}.");

                if (string.IsNullOrWhiteSpace(request.PhoneNumberChangeHistoryId))
                {
                    var errorMessage = "PhoneNumberChangeHistoryId cannot be null or empty.";
                    _logger.LogWarning(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.PhoneNumberChangeHistory, LogLevelInfo.Error);
                    return ServiceResponse<PhoneNumberChangeHistory>.Return404(errorMessage);
                }

                var phoneNumberChangeHistory =  _phoneNumberChangeHistoryRepository.Find(request.PhoneNumberChangeHistoryId);

                if (phoneNumberChangeHistory == null)
                {
                    var notFoundMessage = $"No phone number change history found for ID: {request.PhoneNumberChangeHistoryId}.";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.PhoneNumberChangeHistory, LogLevelInfo.Error);
                    return ServiceResponse<PhoneNumberChangeHistory>.Return404(notFoundMessage);
                }

           
                await BaseUtilities.LogAndAuditAsync($"Successfully retrieved phone number change history for ID: {request.PhoneNumberChangeHistoryId}.", request, HttpStatusCodeEnum.OK, LogAction.ChangePhoneNumber, LogLevelInfo.Information);
                return ServiceResponse<PhoneNumberChangeHistory>.ReturnResultWith200(phoneNumberChangeHistory);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving phone number change history for ID: {request.PhoneNumberChangeHistoryId}. Error: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.PhoneNumberChangeHistory, LogLevelInfo.Error);
                return ServiceResponse<PhoneNumberChangeHistory>.Return500(errorMessage);
            }
        }
    }

}
