using Microsoft.Web.Administration;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.UserServiceManagement.Data
{


    public abstract class BaseEntity
    {
        private DateTime _createdDate;

        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
       
        public string CreatedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";

        public string? FullName
        {
            get => _fullName;
            set => _fullName = value;
        }

        private DateTime _modifiedDate;

        public DateTime ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value.ToLocalTime();
        }

        public string ModifiedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";
        private DateTime _deletedDate;

        public DateTime DeletedDate
        {
            get => _deletedDate.ToLocalTime();
            set => _deletedDate = value.ToLocalTime();
        }

        public string? DeletedBy { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }

        public bool IsDeleted { get; set; } = false;

        private string _TempData = "";
        private string? _fullName;

        public string TempData
        {
            get => _TempData;
            set => _TempData = value;
        }
        public BaseEntity GetBaseEntity()
        {
            return this;
        }

    }

    public abstract class BaseEntityNotification
    {
        private DateTime _createdDate;

        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
        
        private string _createdBy;

        public string CreatedBy
        {
            get => _createdBy;
            set => _createdBy = value;
        }
        private DateTime? _modifiedDate;

        public DateTime? ModifiedDate
        {
            get => _modifiedDate?.ToLocalTime();
            set => _modifiedDate = value?.ToLocalTime();
        }

        public string ModifiedBy { get; set; } = "b956f743-f846-4d83-8804-4046f259b5cf";
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

    }

}