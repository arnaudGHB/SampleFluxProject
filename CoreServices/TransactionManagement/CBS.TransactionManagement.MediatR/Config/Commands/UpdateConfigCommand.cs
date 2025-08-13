using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using MediatR;

namespace CBS.TransactionManagement.Commands
{
    /// <summary>
    /// Represents a command to update a Config.
    /// </summary>
    public class UpdateConfigCommand : IRequest<ServiceResponse<ConfigDto>>
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? Value { get; set; }
        public bool IsYearOpen { get; set; }
        public bool IsDayOpen { get; set; }
        public bool IsDayClosedAutomatic { get; set; }
        public bool UseAutomaticChargingSystem { get; set; }
        public bool MondayIsHoliday { get; set; }
        public bool TuesDayIsHoliday { get; set; }
        public bool MWednessdayIsHoliday { get; set; }
        public bool ThursdayIsHoliday { get; set; }
        public bool FridayIsHoliday { get; set; }
        public bool SaturdayIsHoliday { get; set; }
        public bool SundayIsHoliday { get; set; }
        public DateTime SetCloseDayTime { get; set; }
        public string? LastModifiedOperation { get; set; }
        public string? Description { get; set; }
    }

}
