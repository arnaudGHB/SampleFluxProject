using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Currency : BaseEntity
    {

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryID { get; set; }
        public virtual Country Country { get; set; }
    }
}
