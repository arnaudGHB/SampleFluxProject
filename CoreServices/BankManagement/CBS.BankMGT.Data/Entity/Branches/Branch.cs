using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.BankMGT.Data
{
    public class Branch : BaseEntity
    {
        public string Id { get; set; }
        public string BranchCode { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? BankId { get; set; }
        public string? Capital { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImmatriculationNumber { get; set; }
        public string? TaxPayerNUmber { get; set; }
        public bool IsHavingBank { get; set; }
        public string? PBox { get; set; }
        public string? WebSite { get; set; }
        public string? DateOfCreation { get; set; }
        public string? BankInitial { get; set; }
        public string? Motto { get; set; }
        public string? HeadOfficeTelehoneNumber { get; set; }
        public string? HeadOfficeAddress { get; set; }
        public bool ActiveStatus { get; set; }
        public bool IsHeadOffice { get; set; }
        public virtual Bank Bank { get; set; }
        public virtual ICollection<Subdivision> Subdivisions { get; set; } = new HashSet<Subdivision>();
        public virtual ICollection<Town> Towns { get; set; } = new HashSet<Town>();
    }
}
