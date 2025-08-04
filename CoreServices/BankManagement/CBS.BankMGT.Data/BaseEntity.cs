using Microsoft.Web.Administration;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBS.BankMGT.Data
{
    public abstract  class BaseEntity
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
  
    }

    public abstract class BaseEntityBd
    {
        private DateTime _createdDate;
        [BsonRepresentation(BsonType.String)]
        public DateTime CreatedDate
        {
            get => _createdDate.ToLocalTime();
            set => _createdDate = value.ToLocalTime();
        }
        //[BsonElement("CreatedBy")]

        [BsonRepresentation(BsonType.Binary)]
        public byte[] CreatedBy { get; set; }


        private DateTime _modifiedDate;
        [BsonRepresentation(BsonType.String)]
        public DateTime ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value.ToLocalTime();
        }
        [BsonRepresentation(BsonType.String)]
        public string ModifiedBy { get; set; }
        private DateTime? _deletedDate;
        [BsonRepresentation(BsonType.String)]
        public DateTime? DeletedDate
        {
            get => _deletedDate?.ToLocalTime();
            set => _deletedDate = value?.ToLocalTime();
        }
        [BsonRepresentation(BsonType.String)]
        public string? DeletedBy { get; set; }
        [BsonRepresentation(BsonType.String)]
        public ObjectState ObjectState { get; set; }

        public bool IsDeleted { get; set; } = false;
        private string _branchId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string BranchId
        {
            get => _branchId;
            set => _branchId = value;
        }
        private string _bankId { get; set; }
        [BsonRepresentation(BsonType.String)]
        public string BankId
        {
            get => _bankId;
            set => _bankId = value;
        }
    }
}
