using CBS.AccountManagement.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.AccountManagement.Data 
{
    public class OperationEvent : BaseEntity
    {
        public string Id { get; set; }
        public string? BankId { get; set; } = null;
        public string? OperationEventName { get; set; } 
        public string? Description { get; set; }
        public string EventCode { get; set; }
        public bool HasMultipleBalancingEntries { get; set; }
        public string? AccountTypeId { get;   set; }

        public virtual AccountType AccountTypes { get; set; }

        public ICollection<OperationEventAttributes> OperationEventNameAttributes { get; set; }
    
        public static List<OperationEvent> SetOperationEventEntities(List<OperationEvent> accountCategories, UserInfoToken _userInfoToken)
        {
            List<OperationEvent> categories = new List<OperationEvent>();
            foreach (OperationEvent entity in accountCategories)
            {
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.CreatedDate = DateTime.Now.ToLocalTime();
                entity.ModifiedDate = DateTime.Now.ToLocalTime();
                entity.Id = Guid.NewGuid().ToString();
                
                categories.Add(entity);
            }
            return categories;
        }

        public static OperationEvent SetOperationEventEntity(OperationEvent entity, UserInfoToken _userInfoToken)
        {

            entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
            entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
            entity.CreatedDate = DateTime.Now.ToLocalTime();
            entity.ModifiedDate = DateTime.Now.ToLocalTime();
            entity.Id = Guid.NewGuid().ToString();


            return entity;
        }
        public static List<string> OperationSAVINGEventEntities()
        {
            List<string> categories = new List<string> 
            {
              "DEPOSIT","WITHDRAWAL","INCOMING_TRANSFER","OUTGOING_TRANSFER","TRANSFER"
            };
             
            return categories;
        }
        public static List<string> OperationLOANEventEntities()
        {
            List<string> categories = new List<string>
            {
              "LOAN","REFUND","PARTIAL_REFUND","RESTRUCTURED"
            };

            return categories;
        }

        public static List<OperationEvent>  OperationEventEntities(string productName, UserInfoToken _userInfoToken,string accountType,string ProductType)
        {
            var collection = GetCollectionTobeUsed(ProductType);
            List<OperationEvent> categories = new List<OperationEvent>();
            foreach (var item in collection)
            {
                int i = 0;
                OperationEvent entity = new OperationEvent();
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.AccountTypeId = accountType;
                entity.CreatedDate = DateTime.Now.ToLocalTime();
                entity.ModifiedDate = DateTime.Now.ToLocalTime();
                entity.DeletedBy = default;
                entity.DeletedDate = default;
                entity.IsDeleted = false;
                entity.Id = Guid.NewGuid().ToString();
                entity.EventCode = item+"@"+ productName;
                entity.Description = ProductType;
                entity.OperationEventName = item;
                entity.HasMultipleBalancingEntries = false;
                entity.BankId = _userInfoToken.BankId;
                
                categories.Add(entity);
                i++;
            }
            return categories;
        }

        private static List<string> GetCollectionTobeUsed(string productType)
        {
            if (productType.ToLower()==AccountType_Product.Saving_Product.ToString().ToLower())
            {
               return  OperationSAVINGEventEntities();
            }
            else
            {
                return OperationLOANEventEntities();
            }
        }
    }


}

