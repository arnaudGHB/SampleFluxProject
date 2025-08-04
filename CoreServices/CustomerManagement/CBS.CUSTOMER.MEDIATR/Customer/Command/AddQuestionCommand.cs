
using CBS.CUSTOMER.DATA.Dto.Customers;
using CBS.CUSTOMER.HELPER.Helper;
using MediatR;


namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR
{
    /// <summary>
    /// Represents a command to add a new Question.
    /// </summary>
    public class AddQuestionCommand : IRequest<ServiceResponse<CreateQuestion>>
    {
        public string SecretQuestion { get; set; }


    }

}
