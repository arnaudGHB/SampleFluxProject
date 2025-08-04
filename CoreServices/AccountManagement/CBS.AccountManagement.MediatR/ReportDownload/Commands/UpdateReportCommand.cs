using CBS.AccountManagement.Data;

using CBS.AccountManagement.Helper;

using MediatR;

namespace CBS.AccountManagement.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a customer.
    /// </summary>
    public class UpdateReportCommand : IRequest<ServiceResponse<ReportDto>>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}