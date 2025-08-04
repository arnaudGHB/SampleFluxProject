using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands.ChargesWaivedP
{
    /// <summary>
    /// Represents a command to add a new WithdrawalLimits.
    /// </summary>
    public class AddChargesWaivedCommand : IRequest<ServiceResponse<ChargesWaivedDto>>
    {
        public string CustomerId { get; set; }
        public decimal CustomCharge { get; set; }
        public string Comment { get; set; }
    }

}

