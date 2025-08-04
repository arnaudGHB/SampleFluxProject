
using MediatR;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.HELPER.Helper;

namespace CBS.Customer.MEDIATR
{
    /// <summary>
    /// Represents a command to add a new JobTitle.
    /// </summary>
    public class AddJobTitleCommand : IRequest<ServiceResponse<CreateJobTitle>>
    {

        public string? Title { get; set; }
        public decimal SalaryMidpoint { get; set; }



    }

}
