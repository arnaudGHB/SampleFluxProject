// Ignore Spelling: Mediat MEDIATR

using CBS.CUSTOMER.DATA.Dto.Global;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands.Global
{
    public class GetCustomerByIDOrCINOrNameCommand : IRequest<ServiceResponse<List<GetCustomerByIDOrCINOrNameDto>>>
    {
        public string? SearchItem { get; set; }
    }
}
