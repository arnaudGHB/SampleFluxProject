using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class SubdivisionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DivisionId { get; set; } // Foreign key
        public DivisionDto Division { get; set; }
        public List<TownDto> Towns { get; set; }
    }
}
