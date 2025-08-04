using AutoMapper;
using CBS.APICaller.Helper;
using CBS.NLoan.Data.Entity;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.NLoan.MediatR.EnumData.Handler
{
    public class GetEnumDataCommandHandler : IRequestHandler<GetAllEnumDataCommand, ServiceResponse<EnumDto>>
    {
        private readonly ILogger<GetEnumDataCommandHandler> _logger; // Logger for logging handler actions and errors.
        /// <summary>
        /// Constructor for initializing the GetAllTempAccountQueryHandler.
        /// </summary>
        /// <param name="TempAccountRepository">Repository for Config data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetEnumDataCommandHandler(ILogger<GetEnumDataCommandHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetAllTempAccountQuery to retrieve all Config.
        /// </summary>
        /// <param name="request">The GetAllTempAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<EnumDto>> Handle(GetAllEnumDataCommand request, CancellationToken cancellationToken)
        {
            try
            {
                EnumDto enumData = new EnumDto();
                //loop through enum SMSTypes and add to list
                List<StringValue> entities = new List<StringValue>();

              
                //loop through enum TransactionType and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(InterestPeriod)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanInterestPeriods = entities;
                //loop through enum transaction status and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(InterestType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanInterestTypes = entities;
                //loop through enum account status and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanDurationPeriod)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanDurationPeriods = entities;
                //loop through enum XAFDenomination and add to list
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(CalculateInterestOn)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.CalculateInterestOn = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(RepaymentCycle)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.RepaymentCycles = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanApplicationStatus)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanStatuses = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(RefundOrder)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.RefundOrders = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanDisburstmentType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanDisburstmentTypes = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanTypes = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(PaymentModes)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.PaymentModes = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(PenaltyTypes)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.PenaltyTypes = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(YesOrNo)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.YesOrNo = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanCommiteeValidationStatuses)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanCommiteeValidationStatuses = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(AmortizationType)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.AmortizationTypes = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(DisbursmentStatus)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.DisbursmentStatuses = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanTerms)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanTerms = entities;

                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanTargets)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanTargets = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanCategories)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanCategories = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanDeliquentStatus)))
                {
                    entities.Add(new StringValue(item.ToString()));
                }
                enumData.LoanDeliquenciesStatus = entities;
                entities = new List<StringValue>();
                foreach (var item in Enum.GetValues(typeof(LoanApplicationTypes)))
                {
                    // Exclude both Reschedule and Restructure
                    if (item.ToString() != LoanApplicationTypes.Reschedule.ToString() && item.ToString() != LoanApplicationTypes.Restructure.ToString())
                    {
                        entities.Add(new StringValue(item.ToString()));
                    }
                }
                enumData.LoanApplicationTypes = entities;
                //LoanDeliquenciesStatus
                return ServiceResponse<EnumDto>.ReturnResultWith200(enumData);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all enum data: {e.Message}");
                //await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all enum data: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<EnumDto>.Return500(e, "Failed to get all enum data");
            }
        }

    }
}
