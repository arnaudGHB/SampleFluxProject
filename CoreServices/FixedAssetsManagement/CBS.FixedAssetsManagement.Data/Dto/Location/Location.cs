using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.FixedAssetsManagement.Data
{
    public class LocationDto
    {
        [Key]
        public string  Id { get; set; }
        [Required, StringLength(100)]
        public string LocationName { get; set; }
        [StringLength(200)]
        public string Address { get; set; }

        public string BranchId { get; set; }

        public ICollection<Asset> Assets { get; set; }
    }
}
