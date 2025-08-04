namespace CBS.AccountManagement.Data
{
    public class AccountFeature:BaseEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
 
    
        public static List<AccountFeature> AddWithEntity(List<AccountFeature> listAccountFeatures, UserInfoToken _userInfoToken)
       
  
        {
                List<AccountFeature> categories = new List<AccountFeature>();
                foreach (AccountFeature entity in listAccountFeatures)
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

        public static AccountFeature SetAccountFeatureEntity(AccountFeature entity, UserInfoToken _userInfoToken)
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