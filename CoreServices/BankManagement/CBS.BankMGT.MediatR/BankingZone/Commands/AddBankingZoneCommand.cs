 
using CBS.BankMGT.Data;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to add a new BankingZone.
    /// </summary>
    public class AddBankingZoneCommand : IRequest<ServiceResponse<BankingZoneDto>>
    {
        public string Code { get; set; } // Unique code representing the zone
        public string Name { get; set; } // Name of the banking zone
        public string LocationType { get; set; } 
        public string LocationId{ get; set; }

        internal BankingZone ToBankingZone()
        {
            return new BankingZone
            {
                Code = this.Code,
                Name = this.Name,
                LocationType = this.LocationType,
                LocationId = this.LocationId,
                Id= BaseUtilities.GenerateInsuranceUniqueNumber(12,"BKZ")
            };
        }

        
    }

}
