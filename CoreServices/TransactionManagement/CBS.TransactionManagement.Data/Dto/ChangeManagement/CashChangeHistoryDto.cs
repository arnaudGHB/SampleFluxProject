using CBS.TransactionManagement.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto
{
    public class CashChangeHistoryDto
    {
        public string Id { get; set; }
        public string Reference { get; set; }
        public DateTime ChangeDate { get; set; }
        public decimal AmountGiven { get; set; }
        public decimal AmountReceive { get; set; }
        // Denominations Given
        public int GivenNote10000 { get; set; }
        public int GivenNote5000 { get; set; }
        public int GivenNote2000 { get; set; }
        public int GivenNote1000 { get; set; }
        public int GivenNote500 { get; set; }
        public int GivenCoin500 { get; set; }
        public int GivenCoin100 { get; set; }
        public int GivenCoin50 { get; set; }
        public int GivenCoin25 { get; set; }
        public int GivenCoin10 { get; set; }
        public int GivenCoin5 { get; set; }
        public int GivenCoin1 { get; set; }

        // Denominations Received
        public int ReceivedNote10000 { get; set; }
        public int ReceivedNote5000 { get; set; }
        public int ReceivedNote2000 { get; set; }
        public int ReceivedNote1000 { get; set; }
        public int ReceivedNote500 { get; set; }
        public int ReceivedCoin500 { get; set; }
        public int ReceivedCoin100 { get; set; }
        public int ReceivedCoin50 { get; set; }
        public int ReceivedCoin25 { get; set; }
        public int ReceivedCoin10 { get; set; }
        public int ReceivedCoin5 { get; set; }
        public int ReceivedCoin1 { get; set; }
        public string ServiceOperationType { get; set; }
        public string BranchId { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string ChangedBy { get; set; }
        public string ChangeReason { get; set; }
        public string? VaultId { get; set; }
        public string? SubTellerId { get; set; }
        public string? PrimaryTellerId { get; set; }
        public string? SystemName { get; set; }

    }

}
