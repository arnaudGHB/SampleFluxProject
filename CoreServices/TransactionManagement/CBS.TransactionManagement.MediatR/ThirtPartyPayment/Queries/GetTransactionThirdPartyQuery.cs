using CBS.TransactionManagement.Data.Dto.Transaction;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.ThirtPartyPayment.Queries
{
    public class GetTransactionThirdPartyQuery : IRequest<ServiceResponse<List<TransactionThirdPartyDto>>>
    {
        public string AccountNumber { get; set; }
    }
}
