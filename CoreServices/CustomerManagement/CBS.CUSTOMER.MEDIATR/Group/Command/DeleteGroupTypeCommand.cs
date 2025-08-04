using CBS.CUSTOMER.HELPER.Helper;
using MediatR;

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Represents a command to delete a group type.
    /// </summary>
    public class DeleteGroupTypeCommand : IRequest<ServiceResponse<bool>>
    {
        /// <summary>
        /// Gets or sets the unique identifier of the group type to be deleted.
        /// </summary>
        public  string Id { get; set; }
        public  string UserId { get; set; }
    }

}
