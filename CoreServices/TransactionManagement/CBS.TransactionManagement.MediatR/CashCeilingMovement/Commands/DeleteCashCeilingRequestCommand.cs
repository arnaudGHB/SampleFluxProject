using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.Data.Entity.CashCeilingMovement;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.CashCeilingMovement.Commands
{
    public class DeleteCashCeilingRequestCommand : IRequest<ServiceResponse<bool>>
    {
        public string Id { get; set; } // ID of the cash ceiling request to delete
    }

}
