namespace CBS.TransactionManagement.Data
{
    public class WithdrawalParameter:BaseEntity
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal PhysicalPersonWithdrawalFormFee { get; set; } = 0;
        public decimal MoralPersonWithdrawalFormFee { get; set; } = 0;
        public int NotificationPeriodInMonths { get; set; }
        public string WithdrawalType { get; set; }
        public decimal CamCCULShare { get; set; }
        public decimal FluxAndPTMShare { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public bool MustNotifyOnWithdrawal { get; set; }
        public decimal SourceBrachOfficeShareCMoney { get; set; }
        public decimal DestinationBranchOfficeShareCMoney { get; set; }
        public decimal CamCCULShareCMoney { get; set; }
        public decimal FluxAndPTMShareCMoney { get; set; }
        public decimal HeadOfficeShareCMoney { get; set; }

        public string BankId { get; set; }
        public SavingProduct Product { get; set; }
    }
    public class WithdrawalCharges
    {
        public decimal WithdrawalFormCharge { get; set; }
        public decimal WithdrawalChargeWithoutNotification { get; set; }
        public decimal CloseOfAccountCharge { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal NormalCharge { get; set; }
    }
    public class TransferCharges
    {
        public decimal TransfeFormCharge { get; set; }
        public decimal TransfeFormChargeWithoutNotification { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal TotalCharges { get; set; }
    }

}
