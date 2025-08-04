using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Bank : BaseEntity
    {
        public string Id { get; set; }
        public string BankCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? Capital { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImmatriculationNumber { get; set; }
        public string? TaxPayerNUmber { get; set; }
        public string? PBox { get; set; }
        public string? WebSite { get; set; }
        public string? DateOfCreation { get; set; }
        public string? BankInitial { get; set; }
        public string? Motto { get; set; }
        public string? CustomerServiceContact { get; set; }
        public string? OrganizationId { get; set; } // Foreign key
        public virtual Organization Organization { get; set; }
        public virtual ICollection<Branch> Branches { get; set; } = new HashSet<Branch>();
        public virtual ICollection<Subdivision> Subdivisions { get; set; } = new HashSet<Subdivision>();
        public virtual ICollection<Town> Towns { get; set; } = new HashSet<Town>();
    }
}
