using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Commands
{
    /// <summary>
    /// Represents a command to add a new Loan.
    /// </summary>
    public class AddHolyDayCommand : IRequest<ServiceResponse<HolyDayDto>>
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string? BranchId { get; set; }
        public bool IsCentralisedConfiguration { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsNormalOperationDay { get; set; }

    }

}
