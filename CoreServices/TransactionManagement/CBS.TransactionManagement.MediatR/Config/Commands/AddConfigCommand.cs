using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a Config.
    /// </summary>
    public class AddConfigCommand : IRequest<ServiceResponse<ConfigDto>>
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string? Description { get; set; }
    }

}
