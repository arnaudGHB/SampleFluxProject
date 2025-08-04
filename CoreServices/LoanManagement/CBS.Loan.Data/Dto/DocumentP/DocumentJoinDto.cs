namespace CBS.NLoan.Data.Dto.DocumentP
{
    public class DocumentJoinDto
    {
        public string Id { get; set; }
        public string DocumentId { get; set; }
        public string DocumentPackId { get; set; }
        public DocumentDto Document { get; set; }
    }
}
