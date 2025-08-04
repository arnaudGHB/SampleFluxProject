using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to add a new Config.
    /// </summary>
    public class OpeningOfYearCommand : IRequest<ServiceResponse<string>>
    {
        public string EndOfYearDate { get; set; }
    }

}
