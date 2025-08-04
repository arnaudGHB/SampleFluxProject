using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.TransferLimits;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.TransactionManagement.Dto
{
    public class TransferParameterDto
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string TransferType { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public bool IsConfiguredForShareing { get; set; }
        public decimal TransferFeeRate { get; set; }
        public decimal TransferFeeFlat { get; set; }
        public decimal CamCCULShare { get; set; }
        public decimal FluxAndPTMShare { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public decimal SourceBrachOfficeShareCMoney { get; set; }
        public decimal DestinationBranchOfficeShareCMoney { get; set; }
        public decimal CamCCULShareCMoney { get; set; }
        public decimal FluxAndPTMShareCMoney { get; set; }
        public decimal HeadOfficeShareCMoney { get; set; }

        public string BankId { get; set; }
        public SavingProduct Product { get; set; }
    }
}