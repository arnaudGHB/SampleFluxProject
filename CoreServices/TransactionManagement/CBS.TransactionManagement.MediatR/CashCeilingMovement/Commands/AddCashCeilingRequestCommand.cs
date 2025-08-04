using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.Data.Entity.CashCeilingMovement;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.CashCeilingMovement.Commands
{
    /// <summary>
    /// Represents a command to add a new Transaction.
    /// </summary>
    public class AddCashCeilingRequestCommand : IRequest<ServiceResponse<bool>>
    {
        public decimal CashoutRequestAmount { get; set; }
        public string RequestType { get; set; }//Cash_To_Vault Or Subteller_Cash_To_PrimaryTeller
        public string Requetcomment { get; set; }
    }

}
