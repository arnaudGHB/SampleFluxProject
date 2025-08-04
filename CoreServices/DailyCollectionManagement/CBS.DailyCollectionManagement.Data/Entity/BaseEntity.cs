using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.DailyCollectionManagement.Data.Entity
{
    public abstract class BaseEntity
    {
        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
        public string CreatedBy { get; set; }

        private DateTime _modifiedDate;
        public DateTime ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value.ToLocalTime();
        }
        public string ModifiedBy { get; set; }
        private DateTime? _deletedDate;
        public DateTime? DeletedDate
        {
            get => _deletedDate?.ToLocalTime();
            set => _deletedDate = value?.ToLocalTime();
        }
        public string? DeletedBy { get; set; }
        [NotMapped]
        public ObjectState ObjectState { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string FullName { get; set; }
    }
}
