using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new DepositLimit.
    /// </summary>
    public class AddDepositLimitCommand : IRequest<ServiceResponse<CashDepositParameterDto>>
    {
        public string ProductId { get; set; }
        public string DepositType { get; set; }
        public string? InterDepositOperationType { get; set; }
        public bool IsConfiguredForShareing { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal DepositFeeRate { get; set; }
        public string BankId { get; set; }
        public decimal HeadOfficeShare { get; set; }
        public decimal CamCCULShare { get; set; }
        public decimal FluxAndPTMShare { get; set; }
        public decimal SourceBrachOfficeShareCMoney { get; set; }
        public decimal DestinationBranchOfficeShareCMoney { get; set; }
        public decimal CamCCULShareCMoney { get; set; }
        public decimal FluxAndPTMShareCMoney { get; set; }
        public decimal HeadOfficeShareCMoney { get; set; }
        public string? Path { get; set; }

        public decimal PartnerShare { get; set; }
        public decimal SourceBrachOfficeShare { get; set; }
        public decimal DestinationBranchOfficeShare { get; set; }
        public decimal AmountFrom_A { get; set; }
        public decimal AmountTo_A { get; set; }
        public decimal AmountCharge_A { get; set; }
        public decimal AmountFrom_B { get; set; }
        public decimal AmountTo_B { get; set; }
        public decimal AmountCharge_B { get; set; }
        public decimal AmountFrom_C { get; set; }
        public decimal AmountTo_C { get; set; }
        public decimal AmountCharge_C { get; set; }
        public decimal AmountFrom_D { get; set; }
        public decimal AmountTo_D { get; set; }
        public decimal AmountCharge_D { get; set; }
        public decimal AmountFrom_E { get; set; }
        public decimal AmountTo_E { get; set; }
        public decimal AmountCharge_E { get; set; }
        public decimal AmountFrom_F { get; set; }
        public decimal AmountTo_F { get; set; }
        public decimal AmountCharge_F { get; set; }
        public decimal AmountFrom_G { get; set; }
        public decimal AmountTo_G { get; set; }
        public decimal AmountCharge_G { get; set; }
        public decimal DepositFormFee { get; set; }
        public string? EventAttributForDepositFormFee { get; set; }
        public string? EventAttributForDepositFee { get; set; }
    }

}
