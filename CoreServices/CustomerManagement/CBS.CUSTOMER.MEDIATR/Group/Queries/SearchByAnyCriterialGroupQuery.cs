using CBS.CUSTOMER.DATA.Resources;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries
{


    public class SearchByAnyCriterialGroupQuery : IRequest<ServiceResponse<GroupsList>>
    {
        public GroupResource ResourceParameter { get; set; }

    }

}
