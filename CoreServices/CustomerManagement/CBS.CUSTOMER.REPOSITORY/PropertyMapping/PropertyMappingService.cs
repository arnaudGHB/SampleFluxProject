using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DATA.Dto.Groups;
using CBS.CUSTOMER.DATA.Entity;
using CBS.CUSTOMER.DATA.Entity.CMoney;

namespace CBS.CUSTOMER.REPOSITORY
{


    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _customerMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                { "CustomerId", new PropertyMappingValue(new List<string>() { "CustomerId" } ) },
                { "MembershipApprovalStatus", new PropertyMappingValue(new List<string>() { "MembershipApprovalStatus" } )},
                { "FirstName", new PropertyMappingValue(new List<string>() { "FirstName" } )},
                { "LastName", new PropertyMappingValue(new List<string>() { "LastName" } )},
                { "Phone", new PropertyMappingValue(new List<string>() { "Phone" } )},
                { "Matricule", new PropertyMappingValue(new List<string>() { "Matricule" } )},
                { "AccountConfirmationNumber", new PropertyMappingValue(new List<string>() { "AccountConfirmationNumber" } )},
                { "Active", new PropertyMappingValue(new List<string>() { "Active" } )}
            };

        private Dictionary<string, PropertyMappingValue> _groupMapping =
           new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
           {
                { "GroupName", new PropertyMappingValue(new List<string>() { "GroupName" } ) },
                { "RegistrationNumber", new PropertyMappingValue(new List<string>() { "RegistrationNumber" } )},
                { "TaxPayerNumber", new PropertyMappingValue(new List<string>() { "TaxPayerNumber" } )},
           };

        private Dictionary<string, PropertyMappingValue> _cmoney =
          new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
          {
                { "CustomerId", new PropertyMappingValue(new List<string>() { "CustomerId" } ) },
                { "LoginId", new PropertyMappingValue(new List<string>() { "LoginId" } )},
                { "Name", new PropertyMappingValue(new List<string>() { "Name" } )},
          };

        private IList<IPropertyMapping> propertyMappings = new List<IPropertyMapping>();
        public PropertyMappingService()
        {
            propertyMappings.Add(new PropertyMapping<CustomerDto, Customer>(_customerMapping));
            propertyMappings.Add(new PropertyMapping<GroupDto, Group>(_groupMapping));
            propertyMappings.Add(new PropertyMapping<CMoneyMembersActivationAccountDto, CMoneyMembersActivationAccount>(_cmoney));

        }
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            // get matching mapping
            var matchingMapping = propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
            }

            throw new Exception($"Cannot find exact property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            // the string is separated by ",", so we split it.
            var fieldsAfterSplit = fields.Split(',');

            // run through the fields clauses
            foreach (var field in fieldsAfterSplit)
            {
                // trim
                var trimmedField = field.Trim();

                // remove everything after the first " " - if the fields 
                // are coming from an orderBy string, this part must be 
                // ignored
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                // find the matching property
                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;

        }

    }
}
