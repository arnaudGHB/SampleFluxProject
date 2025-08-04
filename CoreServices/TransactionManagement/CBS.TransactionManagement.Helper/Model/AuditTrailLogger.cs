namespace CBS.TransactionManagement.Helper
{
    public class AuditTrailLogger
    {
        public string? Action { get; set; } // Action performed (Create, Update, Delete, etc.)
        public string? UserName { get; set; } // User responsible for the action
        public string? MicroServiceName { get; set; }
        public string StringifyObject { get; set; }// Entity affected by the action
        public string? DetailMessage { get; set; }
        public string? Level { get; set; }
        public string IPAddress { get; set; }
        public int StatusCode { get; set; }
        public string CorrolationId { get; set; }
        public AuditTrailLogger(string action, string userName, string microService, string stringifyObject, string detailMessage, string level, int statusCode,string corollationid)
        {
            Action = action;
            UserName = userName;
            MicroServiceName = microService;
            StringifyObject = stringifyObject;
            DetailMessage = detailMessage;
            Level = level;
            StatusCode = statusCode;
            CorrolationId=corollationid;
        }
    }

}
