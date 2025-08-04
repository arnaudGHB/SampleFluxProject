using CBS.NLoan.Data.Dto.CustomerP;
using CBS.NLoan.Helper.Helper;
using MediatR;

namespace CBS.NLoan.MediatR.CustomerP.Query
{
    public class GetCustomerCallCommand : IRequest<ServiceResponse<CustomerDto>>
    {
        public string CustomerId { get; set; }
    }
}
