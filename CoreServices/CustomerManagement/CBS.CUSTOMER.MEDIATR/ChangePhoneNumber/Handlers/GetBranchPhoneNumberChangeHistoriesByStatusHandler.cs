using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.ChangePhoneNumber;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.ChangePhoneNumber.Queries;
using CBS.CUSTOMER.REPOSITORY.ChangePhoneNumber;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.ChangePhoneNumber.Handlers
{
    /// <summary>
    /// Handles the query to retrieve the phone number change history for a C-MONEY member based on status.
    /// </summary>
    public class GetBranchPhoneNumberChangeHistoriesByStatusHandler : IRequestHandler<GetBranchPhoneNumberChangeHistoriesByStatusRequestQuery, ServiceResponse<List<PhoneNumberChangeHistory>>>
    {
        private readonly IPhoneNumberChangeHistoryRepository _phoneNumberChangeHistoryRepository;
        private readonly ILogger<GetBranchPhoneNumberChangeHistoriesByStatusHandler> _logger;

        /// <summary>
        /// Constructor for initializing the GetBranchPhoneNumberChangeHistoriesByStatusHandler.
        /// </summary>
        public GetBranchPhoneNumberChangeHistoriesByStatusHandler(
            IPhoneNumberChangeHistoryRepository phoneNumberChangeHistoryRepository,
            ILogger<GetBranchPhoneNumberChangeHistoriesByStatusHandler> logger)
        {
            _phoneNumberChangeHistoryRepository = phoneNumberChangeHistoryRepository;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<PhoneNumberChangeHistory>>> Handle(GetBranchPhoneNumberChangeHistoriesByStatusRequestQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Retrieving phone number change history with status or branch filter: {request.Status}");

                // Validate input
                if (string.IsNullOrWhiteSpace(request.Status))
                {
                    var message = "Status or BranchId is required to fetch phone number change histories.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.BadRequest, LogAction.PhoneNumberChangeHistory, LogLevelInfo.Warning);
                    return ServiceResponse<List<PhoneNumberChangeHistory>>.Return400(message);
                }

                // Fetch all records first (if repository does not support IQueryable)
                var histories =  _phoneNumberChangeHistoryRepository.All.ToList();

                // Apply filtering in-memory
                if (!request.Status.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    if (long.TryParse(request.Status, out long branchId))
                    {
                        histories = histories.Where(x => x.BranchId == branchId.ToString()).ToList();
                    }
                    else
                    {
                        histories = histories.Where(x => x.Status == request.Status).ToList();
                    }
                }

                // Check if results are empty
                if (!histories.Any())
                {
                    var message = "No phone number change histories found for the given filter.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.PhoneNumberChangeHistory, LogLevelInfo.Warning);
                    return ServiceResponse<List<PhoneNumberChangeHistory>>.Return404(message);
                }

                _logger.LogInformation($"Successfully retrieved {histories.Count} phone number change histories.");
                return ServiceResponse<List<PhoneNumberChangeHistory>>.ReturnResultWith200(histories);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error occurred while retrieving phone number change history: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.PhoneNumberChangeHistory, LogLevelInfo.Error);
                return ServiceResponse<List<PhoneNumberChangeHistory>>.Return500(errorMessage);
            }
        }
    }

}
