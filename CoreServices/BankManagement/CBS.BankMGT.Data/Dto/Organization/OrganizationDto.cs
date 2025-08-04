using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class OrganizationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CountryId { get; set; } // Foreign key
        public CountryDto Country { get; set; }
        public List<BankDto> Banks { get; set; }
    }
}
