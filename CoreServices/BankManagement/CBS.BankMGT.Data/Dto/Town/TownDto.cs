using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class TownDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubdivisionId { get; set; } // Foreign key
        public SubdivisionDto Subdivision { get; set; }
    }
}
