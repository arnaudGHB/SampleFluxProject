namespace CBS.AccountManagement.Data
{
    public class AccountClass : BaseEntity
    {
        public string? Id { get; set; }
        public string AccountNumber { get; set; }
        public string AccountCategoryId { get; set; }
        public virtual AccountCategory AccountCategory { get; set; }
        public static List<AccountClass> SetAccountCategoriesEntities(List<AccountClass> accountCategories, UserInfoToken _userInfoToken)
        {
            List<AccountClass> categories = new List<AccountClass>();
            foreach (AccountClass entity in accountCategories)
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

        public static AccountClass SetAccountClassEntity(AccountClass entity, UserInfoToken _userInfoToken)
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