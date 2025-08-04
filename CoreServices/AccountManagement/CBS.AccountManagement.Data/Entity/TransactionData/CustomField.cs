namespace CBS.AccountManagement.Data
{
    public class CustomField
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public string Owner { get; set; }
        public string Tab { get; set; }
        public bool Unique { get; set; }
        public bool Mandatory { get; set; }
        public bool IsCopied { get; set; }
        public int Order { get; set; }
        public string Extra { get; set; }
        public bool Deleted { get; set; }
    }
}