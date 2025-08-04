namespace CBS.SystemConfiguration.MediatR.Commands
{
    public class TownFile
    {
        public TownFile(string? id, string? name, string? subdivisonId, string? divisionId)
        {
            Id = id;
            Name = name;
            SubdivisonId = subdivisonId;
            DivisionId = divisionId;
        }

        public string? Id { get; }
        public string? Name { get; }
        public string? SubdivisonId { get; }
        public string? DivisionId { get; }
 
    }
}