using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Queries
{
    public class CustomerPinValidationCommand : IRequest<ServiceResponse<CustomerPinValidationDto>>
    {
        public string Telephone { get; set; }
        public string Pin { get; set; }
        public string Channel { get; set; }
    }
}
