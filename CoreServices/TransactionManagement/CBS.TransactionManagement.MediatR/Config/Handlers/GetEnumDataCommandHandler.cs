using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Helper.Model;
using Newtonsoft.Json.Linq;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all TempAccount based on the GetAllTempAccountQuery.
    /// </summary>
    public class GetEnumDataCommandHandler : IRequestHandler<GetAllEnumDataCommand, ServiceResponse<EnumData>>
    {
        private readonly ILogger<GetEnumDataCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the GetAllTempAccountQueryHandler.
        /// </summary>
        /// <param name="TempAccountRepository">Repository for Config data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEnumDataCommandHandler(
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetEnumDataCommandHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _userInfoToken = UserInfoToken;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTempAccountQuery to retrieve all Config.
        /// </summary>
        /// <param name="request">The GetAllTempAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EnumData>> Handle(GetAllEnumDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                EnumData enumData = new EnumData();
                //loop through enum SMSTypes and add to list
                List<StringValue> entities = new List<StringValue>();

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(DepositType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.depositTypes = entities;
                //loop through enum TransactionType and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(TransactionType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.transactionTypes = entities;
                //loop through enum transaction status and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(TransactionStatus)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.transactionStatus = entities;
                //loop through enum account status and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(AccountStatus)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.accountStatus = entities;
                //loop through enum XAFDenomination and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(XAFDenomination)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.XAFDenomination = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(InterOperationType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.withdrawalTypes = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(TransferType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.transferTypes = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(ManagementFeeFrequency)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.managementFeeFrequencies = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(PostingFrequency)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.postingFrequencies = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(InterestCalculationFrequency)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.interestCalculationFrequencies = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(TermDepositDurations)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.termDepositDurations = entities;
                foreach (var item in Enum.GetValues(typeof(CloseOfDayActions)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.closeOfDayStatuses = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(PostingFrequency)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.freeQuencies = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(Currency)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.currencies = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(OperationEventRubbriqueName)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.operationAccounts = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(CloseOfDayActions)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.primaryTellerEODStatuses = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(CloseOfDayStatus)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.accountantEODStatuses = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(Status)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.Statuses = entities;

                //InterOperationType

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(CashInCashOutTransfer)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.CashInCashOutTransfer = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(InterOperationType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.InterOperationType = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LegalForms)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(HolidayType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.HolidayTypes = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(RecurrencePattern)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.RecurrencePatterns = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(DayOfWeek)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.DayOfWeeks = entities;

                return ServiceResponse<EnumData>.ReturnResultWith200(enumData);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all enum data: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all enum data: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<EnumData>.Return500(e, "Failed to get all enum data");
            }
        }
       
    }
}
