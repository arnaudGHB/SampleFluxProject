using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data.Dto
{
    public class SubcriptionPackageItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubcriptionPackageID { get; set; }
        public double ServiceFee { get; set; }
    }
}
