using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Entity.RemittanceP
{
    public class Remittance : BaseEntity
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; } 
        public string AccountId { get; set; }
        public string SourceBranchCode { get; set; }
        public string SourceBranchId { get; set; }
        public string? SourceTellerId { get; set; } 
        public string SourceBranchName { get; set; } 
        public string? SenderSecreteCode { get; set; } 
        public string SenderName { get; set; }
        public string? SenderCNI { get; set; }
        public string? SourceTellerName { get; set; }
        public string? SenderPhoneNumber { get; set; }
        public string? SenderAddress { get; set; }
        public string? ReceiverName { get; set; }
        public string? ReceiverCNI { get; set; }
        public string? ReceiverCNIIssueDate { get; set; }
        public string? ReceiverCNIExpiryDate { get; set; }
        public string? ReceiverCNIPlaceOfIssue { get; set; }
        public string? ReceiverAddress { get; set; }
        public string? ReceiverLanguage { get; set; }
        public string? ReceiverPhoneNumber { get; set; }
        public string? ReceivingBranchCode { get; set; }
        public string? ReceivingBranchId { get; set; }
        public string? ReceivingBranchName { get; set; }
        public string? ReceivingTellerName { get; set; }
        public string? ReceivingTellerId { get; set; }
        public decimal Amount { get; set; }
        public decimal InitailAmount { get; set; }
        public string ChargeType { get; set; }
        public decimal Fee { get; set; }
        public decimal SourceBranchCommision { get; set; }
        public decimal RecivingBranchCommision { get; set; }
        public decimal HeadOfficeCommision { get; set; }
        public string Status { get; set; }
        public string? ApprovalComment { get; set; }
        public string? ApprovedBy { get; set; }
        public string InitiatedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime InitiationDate { get; set; }
        public DateTime? DateOfCashOut { get; set; }
        public DateTime AccountingDate { get; set; }
        public DateTime? DatePaidToCashDesk { get; set; }
        public string TransactionReference { get; set; }
        public decimal TotalAmount { get; set; }
        public string? SenderNote { get; set; }
        public string? RemittanceType { get; set; }
        public bool SendSMSTotReceiver { get; set; }
        public string DateOfIssue { get; set; }
        public string ExpirationDate { get; set; }
        public string PlaceOfIssue { get; set; }
        public string ExternalReference { get; set; }
        public string TransferType { get; set; } //Incoming & Out_Going Transfer
        public string TransferSource { get; set; }//International_Remittance Or Local_Remittance
        public DateTime? InternationalTransfterDate { get; set; }
        public bool IsOTPVerified { get; set; }
        public bool IsAutoVerifyReceiver { get; set; }
        public bool IsAutoVerifySender { get; set; }
        public bool IsManualVerification { get; set; }
        public string? SenderCountry { get; set; }
        public string? ReceiverCountry { get; set; }
        public string? CapturedReceiverCNI { get; set; }
        public string? CapturedReceiverName { get; set; }
        public string? CapturedReceiverCNIDateOfIssue { get; set; }
        public string? CapturedReceiverCNIDateOfExpiration { get; set; }
        public string? CapturedReceiverCNIPlcaceOfIssue { get; set; }
        public string? CapturedOTP { get; set; }
        public string? CapturedSenderName { get; set; }
        public string? CapturedSenderPhoneNumber { get; set; }
        public string? CapturedReceiverPhoneNumber { get; set; }
        public string? CapturedSenderSecretCode { get; set; }
        public string? CapturedSenderAddress { get; set; }
        public string? CapturedReceiverAddress { get; set; }
        public decimal CapturedRemittanceAmount { get; set; }
        public bool IsChargesInclussive { get; set; }
        public decimal ReceiverAmount { get; set; }
        public decimal RecevingBranchTotalAmount { get; set; }
        public DateTime? CapturedRemittanceDate { get; set; }
    }

}
