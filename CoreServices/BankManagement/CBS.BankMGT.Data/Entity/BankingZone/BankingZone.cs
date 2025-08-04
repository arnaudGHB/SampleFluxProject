using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    [BsonIgnoreExtraElements]
    public class BankingZone:BaseEntity
    {
        public string Id { get; set; } // Unique identifier for the banking zone
        public string Code { get; set; } // Unique code representing the zone
        public string Name { get; set; } // Name of the banking zone
        public string LocationType { get; set; } // Type of location (e.g., Urban, Rural)
        public string LocationId { get; set; } // Identifier for the location related to this zone
        [BsonElement("BranchId")]
        public string BranchId { get; set; }
        public string BankId { get; set; }

    }

    public class BankZoneBranch:BaseEntity
    {
        public string Id { get; set; }
        public string BankingZoneId { get; set; } // Identifier for the associated Banking Zone
        public string Type { get; set; } // Bank or Branche
        private string _branchId { get; set; }

        public string BranchId
        {
            get => _branchId;
            set => _branchId = value;
        }
        private string _bankId { get; set; }

        public string BankId
        {
            get => _bankId;
            set => _bankId = value;
        }
    }

}
