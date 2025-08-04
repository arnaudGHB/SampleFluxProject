namespace CBS.AccountManagement.Data
{
    public class CustomFieldValue
    {
        public int Id { get; set; }
        public int? FieldId { get; set; }
        public int OwnerId { get; set; }
        public string Value { get; set; }
    }
}