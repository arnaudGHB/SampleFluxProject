using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.CashCeilingMovement.Queries
{
    public class GetCashCeilingRequestByIdQuery : IRequest<ServiceResponse<CashCeilingRequestDto>>
    {
        public string Id { get; set; }
    }
}
