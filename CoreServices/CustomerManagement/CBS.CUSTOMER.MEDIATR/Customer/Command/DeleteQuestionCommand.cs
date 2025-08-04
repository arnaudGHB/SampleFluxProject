using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands
{

    /// <summary>
    /// Represents a command to delete a Question.
    /// </summary>
    public class DeleteQuestionCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the Question to be deleted.
        /// </summary>
        public string? QuestionId { get; set; }

    }

}
