
using CBS.BankMGT.Data;
using CBS.BankMGT.Helper;
using MediatR;

namespace CBS.BankingZoneMGT.MediatR.Commands
{
    /// <summary>
    /// Represents a command to update a BankingZone.
    /// </summary>
    public class UpdateBankingZoneCommand : IRequest<ServiceResponse<BankingZoneDto>>
    {
        public string Id { get; set; } // Unique identifier for the BankingZoneing zone
        public string Code { get; set; } // Unique code representing the zone
        public string Name { get; set; } // Name of the BankingZoneing zone
        public string LocationType { get; set; } // Type of location (e.g., Urban, Rural)
        public string LocationId { get; set; } // Identifier for the location related to this zone

    }

}
