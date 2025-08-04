using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class SubcriptionPackageItem:BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SubcriptionPackageID { get; set; }
        public double ServiceFee { get; set; }
        public virtual SubcriptionPackage? SubcriptionPackage { get; set; }

    }
    public class CustomerSubcribedPackage:BaseEntity

    {
        public string CustomerID { get; set; }
        public string SubcriptionPackageID { get; set; }
    }
}
