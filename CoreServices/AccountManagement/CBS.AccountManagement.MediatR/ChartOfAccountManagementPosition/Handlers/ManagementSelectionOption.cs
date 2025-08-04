namespace CBS.AccountManagement.MediatR.Handlers
{
    internal class ManagementSelectionOption
    {
        public string Id { get; set; }
        public object AccountNumber { get; set; }
        public string PositionNumber { get; set; }
        public string Description { get; set; }
        public string GeneralRepresentation { get; set; }
    }
}