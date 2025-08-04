using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class CountryDto
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<RegionDto> Regions { get; set; }
        public List<CurrencyDto> Currencies { get; set; }
    }

}
