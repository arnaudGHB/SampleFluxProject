using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class AssetTypeDto
    {
        public string Id { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public string DepreciationMethodName { get; set; }
        public int UsefulLifeYears { get; set; }
    }

}
