namespace CBS.AccountManagement.Data
{
    public class AccountClassDto
    {
        public string? Id { get; set; }

        public string? AccountNumber { get; set; }
        public string? AccountCategoryId { get; set; }
    }

    public class AccountClassCategoryDto
    {
        public string? Id { get; set; }

        public string? Name { get; set; }
 
    }
}