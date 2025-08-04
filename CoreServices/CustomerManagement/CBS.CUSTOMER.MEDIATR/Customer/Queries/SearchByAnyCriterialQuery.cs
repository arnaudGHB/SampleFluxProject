using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{


    public class SearchByAnyCriterialQuery : IRequest<ServiceResponse<CustomersList>>
    {
        public CustomerResource CustomerResource { get; set; }

    }

}
