using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class GetAccountByTellerIdQuery : IRequest<ServiceResponse<AccountDto>>
    {
        public string TellerId { get; set; }
    }
}
