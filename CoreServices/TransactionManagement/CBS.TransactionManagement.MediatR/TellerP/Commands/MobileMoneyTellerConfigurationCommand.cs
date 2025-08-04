using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.TellerP.Commands
{
    /// <summary>
    /// Represents a command to update a Teller.
    /// </summary>
    public class MobileMoneyTellerConfigurationCommand : IRequest<ServiceResponse<TellerDto>>
    {
        public string Id { get; set; }
        public string? OperationEventCode { get; set; }
        public string? MobileMoneyUserKeepingThePhone { get; set; }
        public string? MobileMoneyFloatNumber { get; set; }
        public decimal? MobileMoneyMinimumBalanceAlertLevel { get; set; }
        public decimal? MobileMoneyMaximumBalanceAlertLevel { get; set; }
        public string BranchCode { get; set; }
        public string? MapMobileMoneyToNoneMemberMobileMoneyReference { get; set; }

        public string? FromAuxillaryAccountNumber_A { get; set; }
        public string? ToBranchFloatAccountNumberAuxillary_A { get; set; }

        public string? FromHeadOfficeAccountNumber_B { get; set; }
        public string? ToBranchFloatAccountNumberHeadOffice_B { get; set; }

        public string? FromBranchAccountNumber_C { get; set; }
        public string? ToBranchFloatAccountNumberBranch_C { get; set; }

        public string? FromBranchFloatAccountNumber_D { get; set; }
        public string? ToHeadOfficeFloatAccountNumber_D { get; set; }

        public string? PhoneNumberToRecieveAlert { get; set; }
        public string? MobileMoneyAlertMessageInFrench { get; set; }
        public string? MobileMoneyAlertMessageInEnglish { get; set; }
        public string? AccountNumber { get; set; }
        public string? Option { get; set; }
    }

}
