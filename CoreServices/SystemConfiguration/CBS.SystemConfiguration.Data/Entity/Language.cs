using System.ComponentModel.DataAnnotations;

namespace CBS.SystemConfiguration.Data.Entity
{
    public class Language : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string Code { get; set; } // ex: "fr-FR", "en-US"
        public string Label { get; set; } // ex: "FranÃ§ais (France)"
        public bool IsActive { get; set; }
    }
}
