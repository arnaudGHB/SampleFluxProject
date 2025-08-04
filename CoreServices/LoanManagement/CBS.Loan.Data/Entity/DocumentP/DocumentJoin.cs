namespace CBS.NLoan.Data.Entity.DocumentP
{
    public class DocumentJoin : BaseEntity
    {
        public string Id { get; set; }
        public string DocumentId { get; set; }
        public string DocumentPackId { get; set; }
        public virtual Document Document { get; set; }
        public virtual DocumentPack DocumentPack { get; set; }
    }
}
