namespace CBS.TransactionManagement.Data
{
    public class CashDepositParameter:BaseEntity
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string DepositType { get; set; }
        public string? InterDepositOperationType { get; set; }
        public bool IsConfiguredForShareing { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string BankId { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public decimal CamCCULShare { get; set; }
        public decimal FluxAndPTMShare { get; set; }
        public decimal HeadOfficeShare { get; set; }

        public decimal SourceBrachOfficeShareCMoney { get; set; }
        public decimal DestinationBranchOfficeShareCMoney { get; set; }
        public decimal CamCCULShareCMoney { get; set; }
        public decimal FluxAndPTMShareCMoney { get; set; }
        public decimal HeadOfficeShareCMoney { get; set; }

        public string? EventAttributForDepositFormFee { get; set; }
        public string? EventAttributForDepositFee { get; set; }
        public virtual SavingProduct Product { get; set; }

    }

}
