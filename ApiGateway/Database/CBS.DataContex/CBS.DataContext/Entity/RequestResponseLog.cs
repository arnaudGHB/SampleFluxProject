using MongoDB.Bson;

namespace CBS.Gateway.DataContext
{
    public class RequestResponseLog
    {
        public ObjectId Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string RequestMethod { get; set; }
        public string RequestPath { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestBody { get; set; }
        public int ResponseStatusCode { get; set; }
        public string ResponseHeaders { get; set; }
        public string ResponseBody { get; set; }
        public string CorrelationId { get; set; }
        public string IpAddress { get; set; }
        public string ErrorDetails { get; set; } // New property for error details
        public string SourceHost { get; set; }
        public string DestinationHost { get; set; }
    }
}
