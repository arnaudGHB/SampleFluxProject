namespace CBS.NLoan.Data.Dto.DocumentP
{
    public class DocumentPackDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<DocumentJoinDto> DocumentJoins { get; set; }
    }
}
