using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.AccountManagement.Data.Enum;
using Newtonsoft.Json;

namespace CBS.AccountManagement.Data
{
    public class OperationEventAttributes : BaseEntity
    {
        public string Id { get; set; }
        public string? Name { get; set; } //Fee,Pripal/Commission
        public string? Description { get; set; }
        public string? OperationEventAttributeCode { get; set; }
        public string? OperationEventId { get; set; }

        public virtual OperationEvent? OperationEvent { get; set; }

        public static List<OperationEventAttributes> SetOperationEventEntities(string JsonString, List<OperationEventAttributes> accountCategories, UserInfoToken _userInfoToken)
        {
            var listEventData = accountCategories;
            List<OperationEventAttributes> categories = new List<OperationEventAttributes>();
            var OperationEvents = JsonConvert.DeserializeObject<List<OperationEvent>>(JsonString);
            foreach (OperationEventAttributes entity in accountCategories)
            {
                entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                entity.CreatedDate = DateTime.Now.ToLocalTime();
                entity.ModifiedDate = DateTime.Now.ToLocalTime();

                /* entity.OperationEventId = GetOperationEventId(OperationEvents, listEventData, entity.OperationEventCode); *///OperationEvents.Find(x => x.OperationEventCode.Equals()).Id;
                entity.Id = Guid.NewGuid().ToString();
                categories.Add(entity);
            }
            return categories;
        }

        public static List<string> OperationEventAttributesSaving()
        {
            List<string> OperationEventAttributes = new List<string>
            {
                  "Fee",
                "Entry_Fee",
                "Other_Fee",
                "Management_Fee",
                "Interest",
                "Closing_Fee",
                "Tax",
            };

            return OperationEventAttributes;
        }
        public static List<string> OperationEventAttributesLoan()
        {
            List<string> OperationEventAttributes = new List<string>
            {
             
              
                //"amount",
                "commission",
                //"commissions",
                //"fee",

                //"fees",
                "interest",
                //"interests",
                "olb",
                "overdue",
                "overdue_principal",
                //"past_due_days",
                "penalties",
                "principal",
                  "accrued_penalties",
                       "accrued_interest",
                //"accrued_interests",
            };

            return OperationEventAttributes;
        }

        public static List<OperationEventAttributes> SetOperationEventAttributesEntities(UserInfoToken _userInfoToken, List<OperationEvent> operationEvents, string Product, string accountType)
        {
            List<string> collection = GetCollectionTobeUsed(Product);
            var model = operationEvents.Find(x => x.AccountTypeId == accountType);
            List<OperationEventAttributes> ListOperationEventAttributes = new List<OperationEventAttributes>();
            int i = 0; int j = 0; var table = operationEvents.ToArray(); var table1 = collection.ToArray();
            while (i <= table.Length - 1)
            {
                j = 0;
                while (j <= table1.Length - 1)
                {
                    var entity = new OperationEventAttributes();
                    entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
                    entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
                    entity.CreatedDate = DateTime.Now.ToLocalTime();
                    entity.ModifiedDate = DateTime.Now.ToLocalTime();
                    entity.OperationEventId = table[i].Id; ///OperationEvents.Find(x => x.OperationEventCode.Equals()).Id;
                    entity.Id = Guid.NewGuid().ToString();
                    entity.Name = table[i].OperationEventName + "-" + table1[j];
                    entity.OperationEventAttributeCode = model.EventCode + "@" + table1[j];
                    ListOperationEventAttributes.Add(entity);
                    j++;
                }
                i++;
            }


            return ListOperationEventAttributes;
        }

        private static List<string> GetCollectionTobeUsed(string productType)
        {
            if (productType.ToLower() == AccountType_Product.Saving_Product.ToString().ToLower())
            {
                return OperationEventAttributesSaving();
            }
            else
            {
                return OperationEventAttributesLoan();
            }
        }
        private static string? GetOperationEventId(List<OperationEvent>? operationEvents, List<OperationEventAttributes> listEventData, string? OperationEventName)
        {
            var result = operationEvents.Find(x => x.OperationEventName == OperationEventName);

            if (result == null)
            {

            }
            else
            {
                return result.Id;
            }

            return null;
        }

        public static OperationEventAttributes SetOperationEventEntity(OperationEventAttributes entity, UserInfoToken _userInfoToken)
        {

            entity.CreatedBy = Guid.Parse(_userInfoToken.Id).ToString();
            entity.ModifiedBy = Guid.Parse(_userInfoToken.Id).ToString();
            entity.CreatedDate = DateTime.Now.ToLocalTime();
            entity.ModifiedDate = DateTime.Now.ToLocalTime();
            entity.Id = Guid.NewGuid().ToString();


            return entity;
        }
    }


}
